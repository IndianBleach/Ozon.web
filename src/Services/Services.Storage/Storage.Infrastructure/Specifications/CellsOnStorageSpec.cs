using Common.Specifications;
using Storage.Data.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Specifications
{
    public class CellsOnStorageSpec : BaseSpecification<StorageCell>
    {
        public CellsOnStorageSpec(int storageId) 
            : base(x => x.StorageId.Equals(storageId))
        {}
    }
}
