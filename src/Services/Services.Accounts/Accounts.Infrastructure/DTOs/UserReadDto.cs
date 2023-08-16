using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class UserReadDto
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public UserReadDto(string userId, string firstName, string secondName)
        {
            UserId = userId;
            FirstName = firstName;
            SecondName = secondName;
        }
    }
}
