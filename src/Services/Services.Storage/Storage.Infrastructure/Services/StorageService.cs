using AutoMapper;
using Common.DataQueries;
using Common.DTOs.Storage;
using Common.Repositories;
using Storage.Data.Context;
using Storage.Data.Entities.Actions;
using Storage.Data.Entities.Address;
using Storage.Data.Entities.Products;
using Storage.Data.Entities.Storage;
using Storage.Infrastructure.DTOs;
using Storage.Infrastructure.Mappers;
using Storage.Infrastructure.Repositories;
using Storage.Infrastructure.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Services
{
    public class StorageService : IStorageService
    {
        private readonly IServiceRepository<StorageProduct> _productRepository;

        private readonly IServiceRepository<StorageCell> _cellsRepository;

        private readonly IServiceRepository<StorageActionType> _actionsRepository;

        private readonly IServiceRepository<MarketStorage> _storagesRepository;

        private readonly IServiceRepository<AddressPoint> _addrRepository;


        private readonly IServiceAsyncRepository<StorageCell> _cellsAsyncRepository;

        private readonly IServiceAsyncRepository<MarketStorage> _storageAsyncRepository;

        private readonly IServiceAsyncRepository<StorageActionType> _actionsAsyncRepository;


        private readonly IMapper _mapper;

        public StorageService(
            IServiceAsyncRepository<StorageActionType> actionsAsyncRepository,
            IServiceAsyncRepository<StorageCell> cellsAsyncRepository,
            IServiceAsyncRepository<MarketStorage> storageAsyncRepository,
            IServiceRepository<AddressPoint> addrRepository,
            IServiceRepository<StorageCell> cellsRepository,
            IServiceRepository<StorageProduct> productRepository,
            IServiceRepository<StorageActionType> actionsRepository,
            IServiceRepository<MarketStorage> storagesRepository)
        {
            _storageAsyncRepository = storageAsyncRepository;
            _cellsAsyncRepository = cellsAsyncRepository;
            _actionsAsyncRepository = actionsAsyncRepository;

            _addrRepository = addrRepository;
            _storagesRepository = storagesRepository;
            _actionsRepository = actionsRepository;
            _cellsRepository = cellsRepository;
            _productRepository = productRepository;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new StorageMapperProfile()
            }));

            _mapper = new Mapper(config);
        }





        #region Upd-1

        public async Task<QueryResult<StorageActionType>> CreateStorageActionTypeAsync(StorageActionTypeApiCreate model)
        {
            return await _actionsAsyncRepository.CreateAsync(new StorageActionType(
                name: model.Name,
                dateCreated: DateTime.Now));
        }

        public async Task<QueryResult<StorageCell>> CreateStorageCellAsync(int storageId, StorageCellApiCreate model)
        {
            StorageCell cell = new StorageCell(
                 cellNumber: model.Name,
                 commentary: model.Comment,
                 storageId: storageId);

            return await _cellsAsyncRepository.CreateAsync(cell);
        }

        public async Task<QueryResult<MarketStorage>> CreateStorageAsync(StorageApiCreate model)
        {
            using (var trans = _addrRepository.NewTransaction())
            {
                QueryResult<AddressPoint> addrResult = _addrRepository.Create(new AddressPoint(
                    cityAddr: model.AddrCity,
                    streetAddr: model.AddrStreet,
                    buildingNumberAddr: model.AddrBuilding));

                if (!addrResult.IsSuccessed || addrResult.Value == null)
                    return QueryResult<MarketStorage>.Failure("error with create address");

                MarketStorage store = new MarketStorage(
                    addressId: addrResult.Value.Id);

                var result = await _storageAsyncRepository.CreateAsync(store);

                if (result.Value == null && !result.IsSuccessed)
                    trans.Rollback();
                else await trans.CommitAsync();

                return result;
            }
        }

        #endregion

        public StorageRead[] GetAllStorages()
        {
            var dtos = _storagesRepository.GetAll();

            return _mapper.Map<IEnumerable<MarketStorage>, StorageRead[]>(dtos);
        }

        public StorageCellRead[] GetStorageCells(int storageId)
        {
            var dtos = _cellsRepository.Find(new CellsOnStorageSpec(
                storageId: storageId));

            return _mapper.Map<IEnumerable<StorageCell>, StorageCellRead[]>(dtos);
        }

        public StorageProductRead[] GetStorageProducts(int storageId)
        {
            var dtos = _productRepository.Find(new ProductsOnStorageSpec(
                onStorageId: storageId));

            return _mapper.Map<IEnumerable<StorageProduct>, StorageProductRead[]>(dtos);
        }
    }
}
