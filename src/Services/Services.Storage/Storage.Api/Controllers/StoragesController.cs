using App.Metrics;
using AutoMapper;
using Azure.Core;
using Common.DataQueries;
using Common.DTOs.Storage;
using Common.Grpc.Extensions;
using Common.Repositories;
using Confluent.Kafka;
using FluentValidation;
using FluentValidation.Results;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Ozon.Bus;
using Ozon.Bus.DTOs.StorageService;
using Storage.Api.Kafka.Services;
using Storage.Api.Validations;
using Storage.Data.Entities.Actions;
using Storage.Data.Entities.Address;
using Storage.Data.Entities.Employees;
using Storage.Data.Entities.Products;
using Storage.Data.Entities.Storage;
using Storage.Infrastructure.DTOs;
using Storage.Infrastructure.Mappers;
using Storage.Infrastructure.Services;
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

        private IMetrics _metrics;

        private readonly ILogger<StoragesController> _logger;

        private readonly IServiceRepository<StorageEmployee> _employeeRepository;

        private readonly IServiceRepository<StorageActionType> _actionTypesRepository;

        private readonly IStorageService _storageService;
        public StoragesController(
            IMetrics metrics,
            IStorageService storageService,
            IProducerFactory producerFactory,
            ILogger<StoragesController> logger,
            IServiceRepository<StorageEmployee> employeeRepository,
            IServiceRepository<StorageActionType> actionTypesRepository)
        {
            _storageService = storageService;

            _metrics = metrics;

            _producerFactory = producerFactory;

            _employeeRepository = employeeRepository;
            _actionTypesRepository = actionTypesRepository;

            _logger = logger;
        }

        #region Upd-1

        [HttpPost("/")]
        public async Task<IActionResult> CreateStorage([FromBody]StorageApiCreate model)
        {
            ValidationResult validationResult = await ValidationRegistry.CreateStorageValidator
                .ValidateAsync(model);

            if (!validationResult.IsValid)
                return BadRequest(ApiResponseRead<MarketStorage>.Failure(validationResult.Errors
                    .Select(er => er.ErrorMessage)));

            QueryResult<MarketStorage> createStorageQuery = await _storageService.CreateStorageAsync(model);

            if (createStorageQuery.Value == null || !createStorageQuery.IsSuccessed)
                return Ok(createStorageQuery.ToApiResponse());

            var producer = _producerFactory.Get<string, AddStorageMessage>();

            if (producer != null)
            {
                _logger.LogInformation("+msg to [storage-marketplace.addMarketplaceStorage] storageId: " + createStorageQuery.Value.Id);

                producer.PublishMessage(
                    toTopicAddr: "storage-marketplace.addMarketplaceStorage",
                    message: new Message<string, AddStorageMessage>
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = new AddStorageMessage
                        {
                            ExternalStorageId = createStorageQuery.Value.Id,
                            BuildingNumberAddr = model.AddrBuilding,
                            CityAddr = model.AddrCity,
                            StreetAddr = model.AddrStreet
                        }
                    },
                    handler: (report) =>
                    {
                        _logger.LogInformation($"msg[storage-marketplace.addMarketplaceStorage] report: {report.Error.Reason} {report.Status.ToString()}");
                    });
            }
            else _logger.LogCritical("[StoragesController] not found producer (AddStorageMessage)");

            return Ok(createStorageQuery.ToApiResponse());
        }

        [HttpPost("/{storage_id:int}/cells")]
        public async Task<IActionResult> CreateStorageCell(
           [FromRoute]int storage_id,
           [FromBody] StorageCellApiCreate model)
        {
            ValidationResult validationResult = await ValidationRegistry.CreateStorageCellValidator
                .ValidateAsync(model);

            if (!validationResult.IsValid)
                return BadRequest(ApiResponseRead<StorageCell>.Failure(validationResult.Errors
                    .Select(er => er.ErrorMessage)));

            return Ok(_storageService.CreateStorageCellAsync(storage_id, model)
                .Result
                .ToApiResponse());
        }

        [HttpPost("/actions")]
        public async Task<IActionResult> CreateActionType(
           [FromBody]StorageActionTypeApiCreate model)
        {
            ValidationResult validationResult = await ValidationRegistry.CreateActionTypeValidator
                .ValidateAsync(model);

            if (!validationResult.IsValid)
                return BadRequest(ApiResponseRead<StorageActionType>.Failure(validationResult.Errors
                    .Select(er => er.ErrorMessage)));

            return Ok(_storageService.CreateStorageActionTypeAsync(model)
                .Result
                .ToApiResponse());
        }



        #endregion




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

        



        


        #region Old update

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
            return Ok(_storageService.GetAllStorages());
        }

        #endregion


        [HttpGet("/{storage_id:int}/products")]
        public async Task<IActionResult> GetStorageProducts(int storage_id)
        {
            var dtos = _storageService.GetStorageProducts(storage_id);

            return Ok(dtos);
        }

        [HttpGet("/{storage_id:int}/cells")]
        public async Task<IActionResult> GetAllStorageCells(int storage_id)
        {
            var dtos = _storageService.GetStorageCells(storage_id);

            return Ok(dtos);
        }
    }
}