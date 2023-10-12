using AutoMapper;
using Common.DataQueries;
using Common.DTOs.ApiRequests.Products;
using Common.DTOs.Products;
using Common.Repositories;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Ozon.Bus;
using Ozon.Bus.DTOs.ProductsRegistry;
using Products.Data.Entities;
using Products.Infrastructure.Mappers;

namespace Products.Api.Controllers
{
    //[ApiController]
    [Route("[controller]")]
    //[Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IServiceRepository<Product> _productsRepository;

        private readonly IServiceRepository<ProductSeller> _sellersRepository;

        private readonly ILogger<ProductsController> _logger;

        private readonly IMapper _mapper;

        private readonly IProducerFactory _producerFactory;

        public ProductsController(
            IProducerFactory producerFactory,
            IServiceRepository<Product> productsRepository,
            IServiceRepository<ProductSeller> sellersRepository,
            ILogger<ProductsController> logger)
        {
            _producerFactory = producerFactory;
            _sellersRepository = sellersRepository;
            _productsRepository = productsRepository;
            _logger = logger;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new ProductMapperProfile()
            }));

            _mapper = new Mapper(config);
        }

        // /products/ -> /
        // /products/sellers -> /sellers/..


        [HttpPost("/")]
        public async Task<IActionResult> RegisterProduct(
            [FromForm]ProductApiPost model)
        {
            _logger.LogWarning($"{nameof(RegisterProduct)} {model.Title} {model.SellerId}");

            Product product = new Product(
                title: model.Title,
                description: model.Description,
                defaultPrice: model.DefaultPrice,
                dateCreated: DateTime.Now,
                dateUpdated: DateTime.Now,
                sellerId: model.SellerId);

            QueryResult<Product> prodResult = _productsRepository.Create(product);

            return Ok(prodResult);
        }

        [HttpPost("/sellers")]
        public async Task<IActionResult> CreateSeller(
            [FromForm]ProductSellerApiPost seller)
        {
            _logger.LogInformation($"{nameof(CreateSeller)} {seller.Title} {seller.SpecialCode}");

            var result = _sellersRepository.Create(new ProductSeller(
                name: seller.Title,
                bankAccountNumber: seller.Bank,
                site: seller.Site,
                contactEmail: seller.Email,
                specialNumber: seller.SpecialCode,
                description: seller.Description,
                dateCreated: DateTime.Now));

            if (result.IsSuccessed)
            {
                ProducerWrapper<string, ProductRegistryMarketplaceSeller>? producer = _producerFactory.Get<string, ProductRegistryMarketplaceSeller>();

                if (producer != null)
                {
                    producer.PublishMessage(
                        toTopicAddr: "products-marketplace.addMarketplaceSeller",
                        message: new Message<string, ProductRegistryMarketplaceSeller>
                        {
                            Key = Guid.NewGuid().ToString(),
                            Value = new ProductRegistryMarketplaceSeller
                            {
                                Description = seller.Description,
                                Email = seller.Email,
                                ExternalSellerId = result.Value.Id,
                                Name = seller.Title,
                                Site = seller.Site
                            }
                        },
                        handler: (report) =>
                        {
                            _logger.LogInformation($"msg[products-marketplace.addMarketplaceSeller] report: {report.Error.Reason} {report.Status.ToString()}");
                        });
                }
                else _logger.LogCritical("producer not found [ProductRegistryMarketplaceSeller]");
            }

            return Ok(QueryResult<string>.Successed(result.Value?.Id));
        }

        [HttpGet("/sellers")]
        public async Task<IActionResult> GetAllSellers()
        {
            _logger.LogInformation(nameof(GetAllSellers));

            var result = _sellersRepository.GetAll();

            return Ok(result);
        }

        [HttpGet("/")]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation(nameof(GetAllProducts));

            var result = _productsRepository.GetAll();

            return Ok(_mapper.Map<IEnumerable<Product>, IEnumerable<ProductRead>>(result));
        }
    }
}