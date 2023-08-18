using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Entities.ProductEntities
{


    // chars
    // category
    // percs

    // 

    // delivery info

    // product uploader (user) +
    // photos +
    // seller + 


    // catalog
    // category (percs)
    // subcategory (percs)

    internal class ProductPreviewImage
    { 
        public string MediaSectionId { get; set; }

        public string ImageId { get; set; }

    }

    internal class ProductUploadInfo
    { 
        public string UserAccountId { get; set; }

        public DateTime UploadDate { get; set; }
    }

    internal class ProductSeller
    { 
        public string Name { get; set; }

        public string Description { get; set; }

        public string BannerImagePath { get; set; }

        public string ImagePath { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }



    internal class OzonProduct
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ProductCode { get; set; }
        
        public double BasePrice { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public int DefaultPreviewImageId { get; set; }

        public ICollection<ProductPreviewImage> PreviewImages { get; set; }

        public ProductSeller Seller { get; set; }

        public ProductUploadInfo UploadUserInfo { get; set; }


    }
}
