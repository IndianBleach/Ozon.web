using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs.ApiRequests.Products
{
    public class ProductApiPost
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? SellerId { get; set; }

        public double DefaultPrice { get; set; }
    }
}
