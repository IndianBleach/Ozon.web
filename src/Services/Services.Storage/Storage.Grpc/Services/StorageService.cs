using Common.DataQueries;
using Common.Grpc.Extensions;
using Common.Repositories;
using GlobalGrpc;
using Grpc.Core;
using Grpc.Protos.Storages;
using Products.Data.Entities.Address;
using Products.Data.Entities.Storage;
using Products.Grpc.GrpcUtils;

namespace Products.Grpc.Services
{
    public class StorageService : StorageServiceGrpc.StorageServiceGrpcBase
    {
        private readonly IServiceRepository<MarketStorage> _storageRepository;

        private readonly IServiceRepository<StorageCell> _storageCellRepository;

        private readonly IServiceRepository<AddressPoint> _addressRepository;

        public StorageService(
            IServiceRepository<AddressPoint> addrRepository,
            IServiceRepository<MarketStorage> storageRepository,
            IServiceRepository<StorageCell> storageCellRepository)
        {
            _addressRepository = addrRepository;
            _storageRepository = storageRepository;
            _storageCellRepository = storageCellRepository;
        }

        public override async Task<QueryStringIdResult> CreateStorageCell(CreateStorageCellRequest request, ServerCallContext context)
        {
            if (_storageCellRepository.Any(x => x.CellNumber.Equals(request.NumberTitle)))
                return GrpcGlobalTools.Failure("cell number already in use");

            StorageCell cell = new StorageCell(
                cellNumber: request.NumberTitle,
                commentary: request.Comment,
                storageId: request.StorageId);

            return _storageCellRepository.Create(cell)
                .FromQueryResult();
        }

        public override async Task<QueryStringIdResult> CreateMarketStorage(CreateMarketStorageRequest request, ServerCallContext context)
        {
            AddressPoint addr = new AddressPoint(
                cityAddr: request.Addr.CityAddr,
                streetAddr: request.Addr.StreetAddr,
                buildingNumberAddr: request.Addr.BuildingAddr);

            QueryResult<AddressPoint> addrResult = _addressRepository.Create(addr);

            if (!addrResult.IsSuccessed)
                return GrpcGlobalTools.Failure(addrResult.StatusMessage);

            MarketStorage store = new MarketStorage(
                addressId: addrResult.Value.Id);

            QueryResult<MarketStorage> storeResult = _storageRepository.Create(store);

            return storeResult.FromQueryResult();
        }
    }
}
