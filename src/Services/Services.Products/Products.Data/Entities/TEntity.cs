using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Entities
{
    [PrimaryKey(nameof(Id))]
    public class TEntity : TServiceEntity
    {
        public string Id { get; } = Guid.NewGuid().ToString();
    }

    public abstract class TServiceEntity
    { }
}
