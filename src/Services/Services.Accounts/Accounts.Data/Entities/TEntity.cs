using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Data.Entities
{
    [PrimaryKey(nameof(Id))]
    public class TEntity
    {
        public string Id { get; } = Guid.NewGuid().ToString();
    }
}
