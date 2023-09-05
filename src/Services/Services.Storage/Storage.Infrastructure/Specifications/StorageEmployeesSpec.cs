using Common.Specifications;
using Storage.Data.Entities.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Specifications
{
    public class StorageEmployeesSpec : BaseSpecification<StorageEmployee>
    {
        public StorageEmployeesSpec(int storageId)
            : base(x => x.StorageId.Equals(storageId))
        {}
    }
}
