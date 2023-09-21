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
        }
    }
}
