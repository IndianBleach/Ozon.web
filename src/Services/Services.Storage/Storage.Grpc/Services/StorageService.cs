using Common.DataQueries;
using Common.Grpc.Extensions;
using Common.Repositories;
using GlobalGrpc;
using Grpc.Core;
using Grpc.Protos.Storages;
using Storage.Data.Entities.Actions;
using Storage.Data.Entities.Address;
using Storage.Data.Entities.Employees;
using Storage.Data.Entities.Products;
using Storage.Data.Entities.Storage;
using Storage.Grpc.Extensions;
using Storage.Grpc.GrpcUtils;
using Storage.Grpc.Kafka;
using Storage.Infrastructure.Specifications;
using System;

#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод

namespace Storage.Grpc.Services
{
    public class StorageService : StorageServiceGrpc.StorageServiceGrpcBase
    {
        private readonly IServiceRepository<MarketStorage> _storageRepository;

        private readonly IServiceRepository<StorageCell> _storageCellRepository;

        private readonly IServiceRepository<AddressPoint> _addressRepository;

        private readonly IServiceRepository<StorageEmployee> _employeeRepository;

        private readonly IServiceRepository<StorageActionType> _actionRepository;

        private readonly IServiceRepository<StorageProduct> _productsRepository;

        public StorageService(
            IServiceRepository<AddressPoint> addrRepository,
            IServiceRepository<MarketStorage> storageRepository,
            IServiceRepository<StorageCell> storageCellRepository,
            IServiceRepository<StorageEmployee> employeeRepository,
            IServiceRepository<StorageActionType> actionRepisitory,
            IServiceRepository<StorageProduct> productsRepository,
            IServiceProvider provider)
        {
            CancellationToken token = new CancellationToken();

            var consumerService = provider.GetHostedService<BackgroundConsumerService>();

            Task.Run(async () =>
            {
                if (consumerService.ExecuteTask.IsCompleted)
                {
                    Console.WriteLine("[storage-grpc] start consumer backsrv");
                    await consumerService.StartAsync(token);
                }
            }).Wait();

            Console.WriteLine("[storage-grpc] ctor being");

            _productsRepository = productsRepository;
            _actionRepository = actionRepisitory;
            _employeeRepository = employeeRepository;
            _addressRepository = addrRepository;
            _storageRepository = storageRepository;
            _storageCellRepository = storageCellRepository;
        }

        public override async Task<GetStorageCellsResponse> GetStorageCells(GetStorageCellsRequest request, ServerCallContext context)
        {
            var list = _storageCellRepository.Find(new CellsOnStorageSpec(
                storageId: request.StorageId));

            GetStorageCellsResponse resp = new GetStorageCellsResponse();

            foreach (var item in list)
            {
                resp.Cells.Add(new StorageCellRead()
                {
                    CellId = item.Id,
                    CellNumber = item.CellNumber
                });
            }

            return resp;
        }

        public override async Task<GetStorageProductsResponse> GetStorageProducts(GetStorageProductsRequest request, ServerCallContext context)
        {
            IEnumerable<StorageProduct> list = _productsRepository.Find(new ProductsOnStorageSpec(
                onStorageId: request.StorageId));

            GetStorageProductsResponse resp = new GetStorageProductsResponse();

            resp.StorageId = request.StorageId;

            foreach (var item in list)
            {
                resp.Products.Add(new StorageProductRead
                {
                    ExternalProductId = item.ExternalProductId,
                    InStorageProductId = item.Id
                });
            }

            return resp;
        }

        public override async Task<QueryIntIdResult> AddStorageProduct(AddStorageProductRequest request, ServerCallContext context)
        {
            for (int i = 0; i < request.IncomeCount; i++)
            {
                StorageProduct prod = new StorageProduct(
                    externalProductId: request.ExternalProductId,
                    dateAdded: DateTime.Now,
                    storageId: request.StorageId);

                _productsRepository.Create(prod);
            }

            return new QueryIntIdResult
            {
                SuccessValueId = request.StorageId
            };
        }

        public override async Task<QueryIntIdResult> CreateStorageAction(CreateStorageActionRequest request, ServerCallContext context)
        {
            StorageActionType action = new StorageActionType(
                name: request.ActionName,
                dateCreated: DateTime.Now);

            return _actionRepository.Create(action)
                .FromQueryResult();
        }

        public override async Task<GetAllStorageEmployeesResponse> GetAllStorageEmployees(GetAllStorageEmployeesRequest request, ServerCallContext context)
        {
            GetAllStorageEmployeesResponse resp = new GetAllStorageEmployeesResponse();

            foreach (StorageEmployee item in _employeeRepository.Find(new StorageEmployeesSpec(request.StorageId)))
            {
                resp.Employees.Add(new StorageEmployeeRead()
                {
                    StorageId = item.StorageId,
                    UserAccountId = item.ExternalUserAccountId,
                    StorageEmployeeId = item.Id
                });
            }

            return resp;
        }

        public override async Task<GetAllStorageActionsResponse> GetAllStorageActions(GetAllStorageActionsRequest request, ServerCallContext context)
        {
            GetAllStorageActionsResponse resp = new GetAllStorageActionsResponse();

            foreach (StorageActionType item in _actionRepository.GetAll())
            {
                resp.Actions.Add(new StorageActionTypeRead()
                {
                    ActionId = item.Id,
                    Name = item.Name,
                });
            }

            return resp;
        }

        public override async Task<QueryIntIdResult> CreateStorageEmployee(CreateStorageEmployeeRequest request, ServerCallContext context)
        {
            if (!_storageRepository.Any(x => x.Id == request.StorageId))
                return GrpcGlobalTools.FailureIntId("storage not found");

            StorageEmployee emp = new StorageEmployee(
                externalUserAccountId: request.UserAccountId,
                storageId: request.StorageId,
                dateCreated: DateTime.Now);

            return _employeeRepository.Create(emp)
                .FromQueryResult();
        }

        public override async Task<GetAllStoragesResponse> GetAllStorages(GetAllStoragesRequest request, ServerCallContext context)
#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        {
            GetAllStoragesResponse resp = new GetAllStoragesResponse();

            foreach (MarketStorage item in _storageRepository.GetAll())
            {
                resp.Storages.Add(new MarketStorageShortRead()
                {
                    StorageId = item.Id
                });
            }

            return resp;
        }

        public override async Task<QueryIntIdResult> CreateStorageCell(CreateStorageCellRequest request, ServerCallContext context)
        {
            if (_storageCellRepository.Any(x => x.CellNumber.Equals(request.NumberTitle)))
                return GrpcGlobalTools.FailureIntId("cell number already in use");

            StorageCell cell = new StorageCell(
                cellNumber: request.NumberTitle,
                commentary: request.Comment,
                storageId: request.StorageId);

            return _storageCellRepository.Create(cell)
                .FromQueryResult();
        }

        public override async Task<QueryIntIdResult> CreateMarketStorage(CreateMarketStorageRequest request, ServerCallContext context)
        {
            AddressPoint addr = new AddressPoint(
                cityAddr: request.Addr.CityAddr,
                streetAddr: request.Addr.StreetAddr,
                buildingNumberAddr: request.Addr.BuildingAddr);

            QueryResult<AddressPoint> addrResult = _addressRepository.Create(addr);

            if (!addrResult.IsSuccessed)
                return GrpcGlobalTools.FailureIntId(addrResult.StatusMessage);

            MarketStorage store = new MarketStorage(
                addressId: addrResult.Value.Id);

            QueryResult<MarketStorage> storeResult = _storageRepository.Create(store);

            return storeResult.FromQueryResult();
        }
    }
}
