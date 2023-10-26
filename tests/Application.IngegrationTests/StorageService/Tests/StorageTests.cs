using Common.DataQueries;
using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Storage.Data.Entities.Actions;
using Storage.Data.Entities.Storage;
using Storage.Infrastructure.DTOs;
using Storage.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Application.IngegrationTests.StorageService.Tests
{
    public class StorageTests : BaseIntegrationTest
    {
        private readonly IStorageService _storageService;

        public StorageTests(StorageServiceIntegrationTestApiFactory factory)
            : base(factory)
        {
            var scope = factory.Services.CreateScope();
            _storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();
        }

        #region Upd-1

        [Fact]
        public async Task CreateStorageActionType_InputIsInvalid_ReturnsFail()
        {
            // arrange
            StorageActionTypeApiCreate model1 = new StorageActionTypeApiCreate
            {
                Name = null
            };

            StorageActionTypeApiCreate model2 = new StorageActionTypeApiCreate
            {
                Name = string.Empty
            };

            // act
            ApiResponseRead<StorageActionType> postAction1 = await ApiPostAsync<StorageActionType, StorageActionTypeApiCreate>(
                uri: "/actions",
                body: model1);

            ApiResponseRead<StorageActionType> postAction2 = await ApiPostAsync<StorageActionType, StorageActionTypeApiCreate>(
                uri: "/actions",
                body: model2);

            // asset
            Assert.False(postAction1.IsSuccessed);
            Assert.Null(postAction1.Value);
            Assert.False(StorageServiceDbContext.StorageActionTypes.Any(x => x.Name == model1.Name));

            Assert.False(postAction2.IsSuccessed);
            Assert.Null(postAction2.Value);
            Assert.False(StorageServiceDbContext.StorageActionTypes.Any(x => x.Name == model2.Name));
        }

        [Fact]
        public async Task CreateStorageActionType_InputIsValid_ReturnsTrue()
        {
            // arrange
            StorageActionTypeApiCreate model1 = new StorageActionTypeApiCreate
            {
                Name = "put-item"
            };

            StorageActionTypeApiCreate model2 = new StorageActionTypeApiCreate
            {
                Name = "get-item"
            };

            // act
            ApiResponseRead<StorageActionType> postAction1 = await ApiPostAsync<StorageActionType, StorageActionTypeApiCreate>(
                uri: "/actions",
                body: model1);

            ApiResponseRead<StorageActionType> postAction2 = await ApiPostAsync<StorageActionType, StorageActionTypeApiCreate>(
                uri: "/actions",
                body: model2);

            // asset
            Assert.True(postAction1.IsSuccessed);
            Assert.NotNull(postAction1.Value);
            Assert.True(StorageServiceDbContext.StorageActionTypes.Any(x => x.Name.Equals(model1.Name)));

            Assert.True(postAction2.IsSuccessed);
            Assert.NotNull(postAction2.Value);
            Assert.True(StorageServiceDbContext.StorageActionTypes.Any(x => x.Name.Equals(model2.Name)));
        }

        [Fact]
        public async Task CreateStorageCell_InputIsInvalid_ReturnsFail()
        {
            // arrange
            StorageApiCreate storageModel = new StorageApiCreate()
            {
                AddrBuilding = "test-buuilding-number",
                AddrCity = "test-city",
                AddrStreet = "test-street"
            };

            StorageCellApiCreate cellModel = new StorageCellApiCreate
            {
                Comment = null,
                Name = string.Empty
            };

            // act
            ApiResponseRead<MarketStorage> postStorage = await ApiPostAsync<MarketStorage, StorageApiCreate>(
                uri: "/",
                body: storageModel);

            ApiResponseRead<StorageCell> postStorageCell = await ApiPostAsync<StorageCell, StorageCellApiCreate>(
                uri: string.Format("/{0}/cells", postStorage.Value.Id),
                body: cellModel);

            // assert
            Assert.False(postStorageCell.IsSuccessed);
            Assert.Null(postStorageCell.Value);
            Assert.False(StorageServiceDbContext.StorageCells.Any(cell => cell.CellNumber == string.Empty));
        }

        [Fact]
        public async Task CreateStorageCell_InputIsValid_ReturnsTrue()
        {
            // arrange
            StorageApiCreate storageModel = new StorageApiCreate()
            {
                AddrBuilding = "test-buuilding-number",
                AddrCity = "test-city",
                AddrStreet = "test-street"
            };

            StorageCellApiCreate cellModel = new StorageCellApiCreate
            {
                Comment = string.Empty,
                Name = "test-name"
            };

            // act
            ApiResponseRead<MarketStorage> postStorage = await ApiPostAsync<MarketStorage, StorageApiCreate>(
                uri: "/",
                body: storageModel);

            ApiResponseRead<StorageCell> postStorageCell = await ApiPostAsync<StorageCell, StorageCellApiCreate>(
                uri: string.Format("/{0}/cells", postStorage.Value.Id),
                body: cellModel);

            // assert
            Assert.True(postStorageCell.IsSuccessed);
            Assert.NotNull(postStorageCell.Value);
            Assert.True(StorageServiceDbContext.StorageCells.Any(cell => cell.Id == postStorageCell.Value.Id));
        }

        [Fact]
        public async Task CreateStorage_InputIsInvalid_ReturnsFail()
        {
            // arrange
            StorageApiCreate model = new StorageApiCreate()
            {
                AddrBuilding = string.Empty,
                AddrCity = "test-city1",
                AddrStreet = "test-street1"
            };

            // act
            ApiResponseRead<MarketStorage> postFailStorage = await ApiPostAsync<MarketStorage, StorageApiCreate>(
                uri: "/",
                body: model);

            // assert
            Assert.False(postFailStorage.IsSuccessed);
            Assert.Null(postFailStorage.Value);
            Assert.False(StorageServiceDbContext.MarketStorage
                .Include(x => x.Address)
                .Any(x => x.Address.CityAddr == model.AddrCity && x.Address.StreetAddr == model.AddrStreet));
        }

        [Fact]
        public async Task CreateStorage_InputIsValid_ReturnsTrue()
        {
            // arrange
            StorageApiCreate model = new StorageApiCreate()
            {
                AddrBuilding = "test-buuilding-number",
                AddrCity = "test-city",
                AddrStreet = "test-street"
            };

            // act
            ApiResponseRead<MarketStorage> postStorage = await ApiPostAsync<MarketStorage, StorageApiCreate>(
                uri: "/",
                body: model);

            // assert
            Assert.True(postStorage.IsSuccessed);
            Assert.NotNull(postStorage.Value);
            Assert.Empty(postStorage.Errors);
            Assert.True(StorageServiceDbContext.MarketStorage.Any(x => x.Id == postStorage.Value.Id));
        }

        #endregion
    }
}
