using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs.ApiRequests
{
    public class ProductPropertyApiPost
    {
        public string? PropertyId { get; set; }

        public string? Value { get; set; }
    }

    public class CatalogProductApiPost
    {
        public string? ExternalProductId { get; set; }

        public string? SectionId { get; set; }

        public ProductPropertyApiPost[] Properties { get; set; } = new ProductPropertyApiPost[0];

    }
}
