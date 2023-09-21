using AutoMapper;
using Common.DTOs.ApiRequests;
using Common.DTOs.Catalog;
using Common.Repositories;
using Marketplace.Api.Kafka.Producers;
using Marketplace.Data.Entities.ProductsEntities;
using Marketplace.Data.Entities.PropertyEntities;
using Marketplace.Infrastructure.Mappers;
using Marketplace.Infrastructure.Repositories.Products;
using Marketplace.Infrastructure.Specifications.Properties;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    //[ApiController]
    [Route("/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        private readonly IServiceRepository<CatalogProduct> _productServiceRepository;

        private readonly ILogger<ProductsController> _logger;

        private readonly IMapper _mapper;

        private IProductRegistryProducer _producer;

        public ProductsController(
            IProductRegistryProducer producer,
            ILogger<ProductsController> logger,
            IServiceRepository<CatalogProduct> productsRepository,
            IProductRepository productRepository)
        {
            _productServiceRepository = productsRepository;
            _logger = logger;
            _productRepository = productRepository;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new CatalogMapProfile()
            }));

            _producer = producer;

            _mapper = new Mapper(config);
        }


        // validation
        // elastic
        // cache

        // get detail product

        // catalog/products/123
        // catalog/products (проды с вариантами)

        [HttpGet("/[controller]/")]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation(nameof(GetAllProducts));

            var result = _productServiceRepository.GetAll();

            return Ok(_mapper.Map<IEnumerable<CatalogProduct>, IEnumerable<CatalogProductShortRead>>(result));
        }


        [HttpGet("/[controller]/{product_id}")]
        public async Task<IActionResult> GetProductDetail(
            string product_id)
        {
            _logger.LogInformation(nameof(GetProductDetail), product_id);

            var result = await _productRepository.GetProductDetailAsync(product_id);

            return Ok(result);
        }


        [HttpPost("/[controller]/")]
        public async Task<IActionResult> CreateProduct(
            [FromForm] CatalogProductApiPost model)
        {
            //_logger.LogWarning($"{nameof(CreateProduct)} {model.ExternalProductId} {model.Properties.Length}");

            var result = await _productRepository.CreateProductAsync(model: model);

            if (result.IsSuccessed && !string.IsNullOrEmpty(result.Value))
            {
                Console.WriteLine("send productRegistry sync");

                _producer.UpdateProductRegistryInfo(
                    productId: model.ExternalProductId,
                    result.Value);
            }

            return Ok(result);
        }
    }
}
