using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.Sellers
{
    public class MarketplaceSeller : TEntity
    {
        public string ExternalSellerId { get; set; }

        public string Title { get; set; }

        public bool Verified { get; set; }

        public bool IsPopular { get; set; }

        public DateTime Created { get; set; }

        public string Description { get; set; }

        public string Site { get; set; }

        public string Email { get; set; }

        public MarketplaceSeller(
            string externalSellerId,
            string title,
            bool verified,
            bool isPopular,
            DateTime created,
            string description,
            string site,
            string email)
        {
            Created = created;
            ExternalSellerId = externalSellerId;
            Title = title;
            Verified = verified;
            Description = description;
            Site = site;
            IsPopular = isPopular;
            Email = email;
        }
    }
}
