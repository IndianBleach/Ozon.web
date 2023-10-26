using Common.DataQueries;
using Common.DTOs.Storage;
using Storage.Data.Entities.Actions;
using Storage.Data.Entities.Storage;
using Storage.Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Services
{
    public interface IStorageService
    {
        StorageRead[] GetAllStorages();

        StorageCellRead[] GetStorageCells(int storageId);

        StorageProductRead[] GetStorageProducts(int storageId);



        #region Upd-1

        Task<QueryResult<StorageActionType>> CreateStorageActionTypeAsync(StorageActionTypeApiCreate model);

        Task<QueryResult<StorageCell>> CreateStorageCellAsync(int storageId, StorageCellApiCreate model);

        Task<QueryResult<MarketStorage>> CreateStorageAsync(StorageApiCreate model);

        #endregion
    }
}
