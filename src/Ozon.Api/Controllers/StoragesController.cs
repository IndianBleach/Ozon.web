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

        [HttpPost("/[controller]/{storage_id}/cells")]
        [Consumes("application/json")]
        [Authorize]
        public async Task<IActionResult> CreateStorageCell(
            string storage_id,
            string comment,
            string number_title)
        {
            var storageClient = new StorageServiceGrpc.StorageServiceGrpcClient(_grpcChannel);

            QueryStringIdResult query = await storageClient.CreateStorageCellAsync(new CreateStorageCellRequest
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

            QueryStringIdResult query = await storageClient.CreateMarketStorageAsync(new CreateMarketStorageRequest
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
