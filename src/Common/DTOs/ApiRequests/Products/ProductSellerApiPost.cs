using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs.ApiRequests.Products
{
    public class ProductSellerApiPost
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Bank { get; set; }

        public string? Site { get; set; }

        public string? Email { get; set; }

        public string? SpecialCode { get; set; }
    }
}
