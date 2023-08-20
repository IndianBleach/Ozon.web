using Grpc.Authorization;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Ozon.Api.DTOs.Authorization;

namespace Ozon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizeController : ControllerBase
    {
        private readonly ILogger<AuthorizeController> _logger;

        private GrpcChannel _authorizeChannel;

        public AuthorizeController(ILogger<AuthorizeController> logger)
        {
            _logger = logger;
            _authorizeChannel = GrpcChannel.ForAddress("https://localhost:5001");
        }

        [HttpGet(Name = "SignUp")]
        public async Task<IActionResult> RegisterUser([FromBody] AuthorizeUserPut authorizeUserModel)
        {
            var client = new AuthorizationGrpcService.AuthorizationGrpcServiceClient(_authorizeChannel);

            //client.JWTSignIn(new JwtSignInRequest
            //{ 
            //    Name = authorizeUserModel.UserName,
            //    UserAccountId = authorizeUserModel.
            //})

            return Ok();
        }


        [HttpGet(Name = "SignIn")]
        public async Task<IActionResult> SignInUser([FromBody]AuthorizeUserPut authorizeUserModel)
        {
            var client = new AuthorizationGrpcService.AuthorizationGrpcServiceClient(_authorizeChannel);

            //client.JWTSignIn(new JwtSignInRequest
            //{ 
            //    Name = authorizeUserModel.UserName,
            //    UserAccountId = authorizeUserModel.
            //})

            return Ok();
        }
    }
}