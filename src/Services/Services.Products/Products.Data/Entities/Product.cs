using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Entities
{
    public class Product : TEntity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public double DefaultPrice { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        //public ICollection<ProductPreviewImage> PreviewImages { get; set; }

        public string SellerId { get; set; }

        public ProductSeller? Seller { get; set; }

        public Product(
            string title,
            string description,
            double defaultPrice,
            DateTime dateCreated,
            DateTime dateUpdated,
            string sellerId)
        {
            Title = title;
            Description = description;
            DefaultPrice = defaultPrice;
            DateCreated = dateCreated;
            DateUpdated = dateUpdated;
            SellerId = sellerId;
        }
    }
}
