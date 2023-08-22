using Accounts.Data.Entities;
using Accounts.Data.Entities.AccountEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class ApplicationUser : TEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime DateCreated { get; set; }

        public ICollection<UserAccount> Accounts { get; }

        public ApplicationUser(
            string firstName,
            string lastName,
            string email,
            DateTime dateCreated)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DateCreated = dateCreated;
            Accounts = new List<UserAccount>();
        }
    }
}
