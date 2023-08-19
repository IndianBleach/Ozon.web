using Authorization.Api.Services.Jwt.Client;
using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using Redis.OM.Modeling;
using StackExchange.Redis;

namespace Authorization.Api.Controllers
{
    [Document(Prefixes = new[] { "Employee" })]
    public class Employee
    {
        [RedisIdField]
        public string Name { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class AuthorizeController : ControllerBase
    {
        private readonly IJwtClientService _jwtClientService;

        public AuthorizeController(IJwtClientService jwtClientService)
        {
            _jwtClientService = jwtClientService;
        }


        // check user exists
        // create user
        // write access token, refresh token
        // add user-tokens to redis
        // return

        [HttpGet("SignUp")]
        public async Task<IActionResult> RegisterUser(
            string login,
            string password,
            string email)
        {
            return Ok(await _jwtClientService.SignUpAsync(
                userLogin: login,
                password: password,
                email: email));
        }



        [HttpGet("Test2")]
        public async Task<IActionResult> Test2()
        {
            Console.WriteLine("SIGNIN");

            ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect($"ozon-auth-redis:6379");

            var db = _redis.GetDatabase();

            db.StringGetSet("foo", "bar");

            return Ok(db.StringGet("foo"));
        }

        // sign in
        // sign out
        // sign up

        // auth_params(access_time, )
    }
}