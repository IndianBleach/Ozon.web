using AutoMapper;
using Common.DTOs.ApiRequests;
using Common.DTOs.Catalog;
using Common.Repositories;
using Confluent.Kafka;
using Marketplace.Data.Entities.ProductsEntities;
using Marketplace.Data.Entities.PropertyEntities;
using Marketplace.Infrastructure.Mappers;
using Marketplace.Infrastructure.Repositories.Products;
using Marketplace.Infrastructure.Specifications.Properties;
using Microsoft.AspNetCore.Mvc;
using Ozon.Bus;
using Ozon.Bus.DTOs.StorageService;

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

        private IProducerFactory _producerFactory;

        public ProductsController(
            IProducerFactory producerFactory,
            ILogger<ProductsController> logger,
            IServiceRepository<CatalogProduct> productsRepository,
            IProductRepository productRepository)
        {
            _producerFactory = producerFactory;

            _productServiceRepository = productsRepository;
            _logger = logger;
            _productRepository = productRepository;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new CatalogMapProfile()
            }));
            _mapper = new Mapper(config);
        }

        // validation
        // elastic
        // cache

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
            var result = await _productRepository.CreateProductAsync(model: model);

            if (result.IsSuccessed && !string.IsNullOrEmpty(result.Value))
            {
                Console.WriteLine("send productRegistry sync");

                ProducerWrapper<string, SyncProductRegistryInfoRequest>? producer = _producerFactory.Get<string, SyncProductRegistryInfoRequest>();
                if (producer != null)
                {
                    _logger.LogInformation($"[syncProductRegistryInfo-req] +msg " + result.Value + " " + model.ExternalProductId);

                    producer.PublishMessage(
                        toTopicAddr: "marketplace-products.syncProductRegistryInfo-req",
                        message: new Message<string, SyncProductRegistryInfoRequest>
                        {
                            Key = Guid.NewGuid().ToString(),
                            Value = new SyncProductRegistryInfoRequest
                            {
                                ExternalProductId = model.ExternalProductId,
                                MarketplaceProductId = result.Value,
                                BusAsnwerChannel = "marketplace-products.syncProductRegistryInfo-answer"
                            }
                        }, (report) => {
                            _logger.LogInformation($"[syncProductRegistryInfo-req] delivery {report.Status}");
                        });
                }
            }

            return Ok(result);
        }
    }
}
