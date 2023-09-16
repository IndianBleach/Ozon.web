using Accounts.Data.Entities.AccountEntities;
using Accounts.Data.Entities.RoleEntities;
using Accounts.Infrastructure.Repositories;
using Common.DataQueries;
using Common.DTOs.Accounts;
using Common.Repositories;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Ozon.Common.Logging;

namespace Accounts.RestApi.Controllers
{
    //[ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserAccumulatorService _userAcumService;

        private readonly IServiceRepository<UserRole> _roleRepository;

        private readonly ILogger<AccountsController> _logger;

        public AccountsController(
            IServiceRepository<UserRole> roleRepository,
            IUserAccumulatorService userService,
            ILogger<AccountsController> logger)
        {
            _roleRepository = roleRepository;
            _userAcumService = userService;
            _logger = logger;
        }

        [HttpPost("/roles")]
        public async Task<IActionResult> CreateRole(
            string role_name)
        {
            var query = _roleRepository.Create(new UserRole(
                name: role_name));

            query.LogFromQuery(_logger, nameof(RegisterUser));

            return Ok(query);
        }

        [HttpPost("/register")]
        public async Task<IActionResult> RegisterUser(
            RegisterUserApiPost model)
        {
            QueryResult<UserClaimsJwtToken> query = _userAcumService.RegisterUserIfNotExists(
                model: model);

            query.LogFromQuery(_logger, nameof(RegisterUser));

            if (query.IsSuccessed && query.Value != null)
                return Redirect($"http://localhost:5001/signin?authorize_token={query.Value.ClaimsToken}");

            return BadRequest(query);
        }
    }
}