using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Entities
{
    public class ProductSeller : TEntity
    {
        public string Name { get; set; }

        public string BankAccountNumber { get; set; }

        public string Site { get; set; }

        public string ContactEmail { get; set; }

        public string SpecialNumber { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public ICollection<Product> Products { get; set; }

        public ProductSeller(
            string name,
            string bankAccountNumber,
            string site,
            string contactEmail,
            string specialNumber,
            string description,
            DateTime dateCreated)
        {
            Name = name;
            BankAccountNumber = bankAccountNumber;
            Site = site;
            ContactEmail = contactEmail;
            SpecialNumber = specialNumber;
            Description = description;
            DateCreated = dateCreated;
            Products = new List<Product>();
        }
    }
}
