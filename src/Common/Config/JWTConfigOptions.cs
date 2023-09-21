using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Config
{
    public class JWTAccountsConfigOptions
    {
        public const string ISSUER = "service.accounts.jwt.issuer";
        public const string AUDINCE = "service.accounts.jwt.audince";
        public const string SECRET_KEY = "jwq0-0d-0-zxm9d0!89218]=-qdan8u1082d-==quijdh-01002-ddkdqqqqqqqqq";
    }

    public class JWTConfigOptions
    {
        public const string ISSUER = "ozon.jwt.issuer";
        public const string AUDINCE = "ozon.jwt.audince";
        public const string SECRET_KEY = "jwq0-0d-0-zxm9d0!89218]=-qdan8u1082d-==quijdh-01-=dpsjdywqdkljasnjh";
        public const int ACCESS_TOKEN_LIFETIME_MINS = 10;
    }
}
