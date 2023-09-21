using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.HelpServices
{
    public class PasswordHashHelpService
    {
        private readonly string _hashSalt;

        public PasswordHashHelpService(string hashSalt)
            => _hashSalt = hashSalt;

        public string HashPassword(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + _hashSalt);

            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

        public bool Verify(string password, string hash)
            => HashPassword(password).Equals(hash);
    }
}
