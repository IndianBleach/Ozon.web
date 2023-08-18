using Microsoft.AspNetCore.Mvc;

namespace Authorization.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizeController
    {
        // signin -> {access_token, refresh_token}
        // signout -> {ok, bad}
        // signup -> {access_token, refresh_token}

        // db (mssql)
        
        // docker bridge -> services.accounts (findByUserName)
        // (описать внешний сервис для предоставления сервису авторизации)
    }
}
