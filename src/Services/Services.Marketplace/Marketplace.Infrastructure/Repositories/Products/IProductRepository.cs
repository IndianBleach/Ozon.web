using Common.DataQueries;
using Common.DTOs.ApiRequests;
using Common.DTOs.Catalog;
using Marketplace.Data.Entities.ProductsEntities;
using Marketplace.Infrastructure.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Repositories.Products
{
    public interface IProductRepository
    {
        //Task<QueryResult> UpdateStockStatus();

        Task<QueryResult<CatalogProductRead>> GetProductDetailAsync(string id);

        Task<QueryResult<string>> CreateProductAsync(CatalogProductApiPost model);
    }
}
