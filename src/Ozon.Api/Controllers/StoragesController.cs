using GlobalGrpc;
using Grpc.Net.Client;
using Grpc.Protos.Storages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ozon.Api.Controllers
{
    // api/storages/
    // api/storages/{}/cells
    // api/storages/summary?page
    // api/storages/{}/summary
    // api/storages/{}/cells/{}/summary

    [ApiController]
    [Route("[controller]")]
    public class StoragesController : ControllerBase
    {
        private GrpcChannel _grpcChannel;

        private IConfiguration _config;

        public StoragesController(IConfiguration config)
        {
            _config = config;
            _grpcChannel = GrpcChannel.ForAddress(_config["Services:Storage:GrpcConnectionString"]);
        }

        [HttpGet("/[controller]/{storage_id:int}/products")]
        public async Task<IActionResult> GetStorageProducts(
            int storage_id)
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            var result = await storageClient.GetStorageProductsAsync(new GetStorageProductsRequest
            { 
                StorageId = storage_id
            });

            return Ok(result);
        }

        [HttpGet("/[controller]/{storage_id:int}/cells")]
        public async Task<IActionResult> GetStorageCells(
            int storage_id)
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            var result = await storageClient.GetStorageCellsAsync(new GetStorageCellsRequest
            { 
                StorageId = storage_id
            });

            return Ok(result);
        }


        [HttpPost("/[controller]/{storage_id:int}/products")]
        public async Task<IActionResult> CreateStorageProducts(
            int storage_id,
            string external_product_id,
            int income_count)
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            var result = await storageClient.AddStorageProductAsync(new AddStorageProductRequest
            { 
                ExternalProductId = external_product_id,
                IncomeCount = income_count,
                StorageId = storage_id
            });

            return Ok(result);
        }


        [HttpPost("/[controller]/{storage_id:int}/employees")]
        public async Task<IActionResult> CreateStorageEmployee(
            int storage_id,
            string user_account_id)
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            var result = await storageClient.CreateStorageEmployeeAsync(new CreateStorageEmployeeRequest
            { 
                StorageId = storage_id,
                UserAccountId = user_account_id
            });

            return Ok(result);
        }

        [HttpPost("/[controller]/actions/")]
        public async Task<IActionResult> CreateStorageAction(string action_name)
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            var result = await storageClient.CreateStorageActionAsync(new CreateStorageActionRequest
            { 
                ActionName = action_name
            });

            return Ok(result);
        }

        [HttpGet("/[controller]/actions/all")]
        public async Task<IActionResult> GetAllStorageActions()
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            GetAllStorageActionsResponse result = await storageClient.GetAllStorageActionsAsync(new GetAllStorageActionsRequest());

            return Ok(result);
        }

        [HttpGet("/[controller]/{storage_id:int}/employees")]
        public async Task<IActionResult> GetAllStorageEmployees(int storage_id) 
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            var result = await storageClient.GetAllStorageEmployeesAsync(new GetAllStorageEmployeesRequest()
            { 
                StorageId = storage_id
            });

            return Ok(result);
        }


        [HttpGet("/[controller]/all")]
        public async Task<IActionResult> GetAllStorages()
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            return Ok(storageClient.GetAllStorages(new GetAllStoragesRequest()));
        }

        [HttpPost("/[controller]/{storage_id}/cells")]
        [Consumes("application/json")]
        //[Authorize]
        public async Task<IActionResult> CreateStorageCell(
            int storage_id,
            string comment,
            string number_title)
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            QueryIntIdResult query = await storageClient.CreateStorageCellAsync(new CreateStorageCellRequest
            {
                Comment = comment,
                StorageId = storage_id,
                NumberTitle = number_title
            });

            return Ok(query);
        }

        [HttpPost("/[controller]")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateStorage(
            string building_addr,
            string street_addr,
            string city_addr)
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            QueryIntIdResult query = await storageClient.CreateMarketStorageAsync(new CreateMarketStorageRequest
            {
                Addr = new CreateAddrRequest
                {
                    BuildingAddr = building_addr,
                    CityAddr = city_addr,
                    StreetAddr = street_addr
                }
            });

            return Ok(query);
        }
    }
}
