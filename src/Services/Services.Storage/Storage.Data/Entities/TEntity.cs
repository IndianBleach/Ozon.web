using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Data.Entities
{
    [PrimaryKey(nameof(Id))]
    public class TEntity : TServiceEntity
    {
        public int Id { get; set; }
    }

    public abstract class TServiceEntity
    { }
}
