using App.Metrics;
using AutoMapper;
using Azure.Core;
using Common.DataQueries;
using Common.DTOs.Storage;
using Common.Grpc.Extensions;
using Common.Repositories;
using Confluent.Kafka;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Ozon.Bus;
using Ozon.Bus.DTOs.StorageService;
using Storage.Api.Kafka.Services;
using Storage.Data.Entities.Actions;
using Storage.Data.Entities.Address;
using Storage.Data.Entities.Employees;
using Storage.Data.Entities.Products;
using Storage.Data.Entities.Storage;
using Storage.Infrastructure.Mappers;
using Storage.Infrastructure.Specifications;
using System.Threading;

namespace Storage.Api.Controllers
{
    //[ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class StoragesController : ControllerBase
    {
        private IProducerFactory _producerFactory;

        private IMapper _mapper;

        private IMetrics _metrics;

        private readonly ILogger<StoragesController> _logger;

        private readonly IServiceRepository<MarketStorage> _storageRepository;

        private readonly IServiceRepository<StorageCell> _storageCellRepository;

        private readonly IServiceRepository<AddressPoint> _addressRepository;

        private readonly IServiceRepository<StorageEmployee> _employeeRepository;

        private readonly IServiceRepository<StorageActionType> _actionTypesRepository;

        private IServiceRepository<StorageProduct> _productsRepository;

        public StoragesController(
            IMetrics metrics,
            IProducerFactory producerFactory,
            ILogger<StoragesController> logger,
            IServiceRepository<MarketStorage> storageRepository,
            IServiceRepository<StorageCell> cellRepository,
            IServiceRepository<AddressPoint> addrRepository,
            IServiceRepository<StorageEmployee> employeeRepository,
            IServiceRepository<StorageActionType> actionTypesRepository,
            IServiceRepository<StorageProduct> productsRepository)
        {
            _metrics = metrics;

            _producerFactory = producerFactory;

            _productsRepository = productsRepository;
            _storageCellRepository = cellRepository;
            _addressRepository = addrRepository;
            _employeeRepository = employeeRepository;
            _actionTypesRepository = actionTypesRepository;
            _storageRepository = storageRepository;

            _logger = logger;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new StorageMapperProfile()
            }));

            _mapper = new Mapper(config);
        }

        [HttpPost("/actions")]
        public async Task<IActionResult> CreateActionType(
           string actionName)
        {
            StorageActionType action = new StorageActionType(
                name: actionName,
                dateCreated: DateTime.Now);

            return Ok(_actionTypesRepository.Create(action));
        }


        [HttpPost("/{storage_id:int}/employees")]
        public async Task<IActionResult> CreateStorageEmployee(
           int storage_id,
           string user_account_id)
        {
            StorageEmployee emp = new StorageEmployee(
                externalUserAccountId: user_account_id,
                storageId: storage_id,
                dateCreated: DateTime.Now);

            return Ok(_employeeRepository.Create(emp));
        }

        [HttpPost("/{storage_id:int}/cells")]
        public async Task<IActionResult> CreateStorageCell(
           int storage_id,
           string title,
           string comment)
        {
            StorageCell cell = new StorageCell(
                 cellNumber: title,
                 commentary: comment,
                 storageId: storage_id);

            var result = _storageCellRepository.Create(cell);

            return Ok(result);
        }

        [HttpPost("/")]
        public async Task<IActionResult> CreateStorage(
            string addrCity,
            string addrStreet,
            string addrBuilding)
        {
            QueryResult<AddressPoint> addrResult = _addressRepository.Create(new AddressPoint(
                cityAddr: addrCity,
                streetAddr: addrStreet,
                buildingNumberAddr: addrBuilding));

            if (!addrResult.IsSuccessed)
                return Ok(addrResult);

            MarketStorage store = new MarketStorage(
                addressId: addrResult.Value.Id);

            QueryResult<MarketStorage> storeResult = _storageRepository.Create(store);

            if (storeResult.IsSuccessed)
            {
                var producer = _producerFactory.Get<string, AddStorageMessage>();

                if (producer != null)
                {
                    _logger.LogInformation("+msg to [storage-marketplace.addMarketplaceStorage] storageId: " + storeResult.Value.Id);

                    producer.PublishMessage(
                        toTopicAddr: "storage-marketplace.addMarketplaceStorage",
                        message: new Message<string, AddStorageMessage>
                        {
                            Key = Guid.NewGuid().ToString(),
                            Value = new AddStorageMessage
                            {
                                ExternalStorageId = storeResult.Value.Id,
                                BuildingNumberAddr = addrBuilding,
                                CityAddr = addrCity,
                                StreetAddr = addrStreet
                            }
                        },
                        handler: (report) =>
                        {
                            _logger.LogInformation($"msg[storage-marketplace.addMarketplaceStorage] report: {report.Error.Reason} {report.Status.ToString()}");
                        });
                }
                else _logger.LogCritical("[StoragesController] not found producer (AddStorageMessage)");
            }

            return Ok(storeResult);
        }


        [HttpGet("/actions")]
        public async Task<IActionResult> GetAllStorageActions()
        {
            var dtos = _actionTypesRepository.GetAll();

            return Ok(_actionTypesRepository.GetAll());
        }

        [HttpGet("/{storage_id:int}/employees")]
        public async Task<IActionResult> GetAllStorageEmployees(int storage_id)
        {
            var dtos = _employeeRepository.Find(new StorageEmployeesSpec(storage_id));

            return Ok(dtos);
        }

        [HttpGet("/")]
        public async Task<IActionResult> GetAllStorages()
        {
            var dtos = _storageRepository.GetAll();

            return Ok(_mapper.Map<IEnumerable<MarketStorage>, IEnumerable<StorageRead>>(dtos));
        }


        [HttpGet("/{storage_id:int}/products")]
        public async Task<IActionResult> GetStorageProducts(int storage_id)
        {
            var dtos = _productsRepository.Find(new ProductsOnStorageSpec(
                onStorageId: storage_id));

            return Ok(_mapper.Map<IEnumerable<StorageProduct>, StorageProductRead[]>(dtos));
        }

        [HttpGet("/{storage_id:int}/cells")]
        public async Task<IActionResult> GetAllStorageCells(int storage_id)
        {
            var dtos = _storageCellRepository.Find(new CellsOnStorageSpec(
                storageId: storage_id));

            return Ok(_mapper.Map<IEnumerable<StorageCell>, StorageCellRead[]>(dtos));
        }
    }
}