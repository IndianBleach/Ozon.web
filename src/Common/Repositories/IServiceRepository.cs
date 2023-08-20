using Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public interface IServiceRepository<T>
    {
        T? FirstOrDefault(ISpecification<T> spec);

        IEnumerable<T> FindBy(ISpecification<T> specification);
    }
}
