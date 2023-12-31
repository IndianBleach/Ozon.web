﻿using Common.DataQueries;
using Common.DTOs.ApiRequests;
using Common.DTOs.Catalog;
using Common.Repositories;
using Marketplace.Data.Context;
using Marketplace.Data.Entities.ProductsEntities;
using Marketplace.Data.Entities.Storages;
using Marketplace.Infrastructure.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationContext _dbContext;

        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(
            ApplicationContext dbContext,
            ILogger<ProductRepository> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<QueryResult<string>> CreateProductAsync(CatalogProductApiPost model)
        {
            _logger.LogInformation(nameof(CreateProductAsync));

            // check all props
            // validate all values
            // check externalProductId

            _logger.LogWarning("[count product props] " + model.Properties.Length);

            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                if (!_dbContext.CategorySections.Any(x => x.Id == model.SectionId))
                    throw new Exception("section doesn't exists");

                for (int i = 0; i < model.Properties.Length; i++)
                    if (!_dbContext.CatalogProperties.Any(x => x.Id == model.Properties[i].PropertyId))
                        throw new Exception("prop doesn't exists: " + model.Properties[i].PropertyId);

                var product = new CatalogProduct(
                    externalBaseProductId: model.ExternalProductId,
                    categorySectionId: model.SectionId,
                    marketplaceSellerId: null,
                    title: model.Title,
                    price: null,
                    description: null,
                    catalogStatus: ProductCatalogStatuses.ON_REGISTRATION,
                    dateAdded: DateTime.Now);

                await _dbContext.CatalogProducts.AddAsync(product);

                List<ProductPropertyValue> propValues = new List<ProductPropertyValue>();
                for (int i = 0; i < model.Properties.Length; i++)
                    propValues.Add(new ProductPropertyValue(
                        catalogProductId: product.Id,
                        propertyId: model.Properties[i].PropertyId,
                        value: model.Properties[i].Value));

                await _dbContext.ProductPropertyValues.AddRangeAsync(propValues);

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return QueryResult<string>.Successed(product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);

                return QueryResult<string>.Failure(ex.Message);
            }
        }

        public async Task<QueryResult<CatalogProductRead>> GetProductDetailAsync(string id)
        {
            CatalogProduct? findProduct = await _dbContext.CatalogProducts
                .Include(x => x.CategorySection)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.Catalog)
                .Include(x => x.AvailableInStorages)
                .ThenInclude(x => x.InStorage)
                .ThenInclude(x => x.StorageAddr)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (findProduct == null)
                return QueryResult<CatalogProductRead>.Failure("product not found");

            var variants = _dbContext.ProductPropertyValues
                .Include(x => x.CatalogProduct)
                .Include(x => x.Property)
                .Where(x => x.CatalogProduct != null && x.CatalogProduct.ExternalBaseProductId == findProduct.ExternalBaseProductId)
                .ToArray();

            CatalogProductVariantRead[] vars = new CatalogProductVariantRead[0];
            for (int i = 0; i < variants.Length; i++)
            {
                vars = variants.Select(x => new CatalogProductVariantRead
                {
                    ProductId = x.CatalogProductId,
                    PropertyValue = new CatalogPropertyValueRead
                    {
                        Property = new CatalogPropertyRead
                        {
                            Id = x.Property?.Id,
                            Name = x.Property?.Name,
                        },
                        Value = x.Value
                    }
                })
                    .ToArray();
            }

            ProductCatalogStorageInfo[] stores = findProduct.AvailableInStorages
                .Select(x => new ProductCatalogStorageInfo(
                    x.InStorageId,
                    $"{x.InStorage?.StorageAddr?.City} {x.InStorage?.StorageAddr?.Street} {x.InStorage?.StorageAddr?.BuildingNumber}",
                    stockStatusName: x.StockStatus == ProductStorageStockStatuses.IN_STOCK ? "В наличии" : "Нет в наличии",
                    countNow: x.CountNow))
                .ToArray();
            
            CatalogProductRead dto = new CatalogProductRead
            {
                Id = findProduct.Id,
                SummaryStorages = stores,
                Catalog = new CatalogRead
                {
                    Id = findProduct.CategorySection?.Category?.Catalog?.Id ?? string.Empty,
                    Name = findProduct.CategorySection?.Category?.Catalog?.Name ?? string.Empty
                },
                Category = new CatalogCategoryRead
                {
                    Id = findProduct.CategorySection?.Category?.Id ?? string.Empty,
                    Name = findProduct.CategorySection?.Category?.Name ?? string.Empty
                },
                Description = findProduct.Description ?? string.Empty,
                Name = findProduct.Title ?? string.Empty,
                Price = $"{findProduct.Price} Руб." ?? string.Empty,
                Section = new CatalogSectionRead
                {
                    Id = findProduct.CategorySectionId,
                    InCategoryId = findProduct.CategorySection?.CategoryId ?? string.Empty,
                    Name = findProduct.CategorySection?.Name ?? string.Empty,
                },
                Variants = vars
            };

            return QueryResult<CatalogProductRead>.Successed(dto);
        }
    }
}
