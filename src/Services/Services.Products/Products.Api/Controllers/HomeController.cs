using AutoMapper;
using Common.DataQueries;
using Common.DTOs.ApiRequests.Products;
using Common.DTOs.Products;
using Common.Repositories;
using Microsoft.AspNetCore.Mvc;
using Products.Data.Entities;
using Products.Infrastructure.Mappers;

namespace Products.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class HomeController : ControllerBase
    {
        private readonly IServiceRepository<Product> _productsRepository;

        private readonly IServiceRepository<ProductSeller> _sellersRepository;

        private readonly ILogger<HomeController> _logger;

        private readonly IMapper _mapper;

        public HomeController(
            IServiceRepository<Product> productsRepository,
            IServiceRepository<ProductSeller> sellersRepository,
            ILogger<HomeController> logger)
        {
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
            ProductApiPost model)
        {
            _logger.LogInformation(nameof(RegisterProduct));

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
            ProductSellerApiPost seller)
        {
            _logger.LogInformation(nameof(CreateSeller));

            var result = _sellersRepository.Create(new ProductSeller(
                name: seller.Title,
                bankAccountNumber: seller.Bank,
                site: seller.Site,
                contactEmail: seller.Email,
                specialNumber: seller.SpecialCode,
                description: seller.Description,
                dateCreated: DateTime.Now));

            return Ok(QueryResult<string>.Successed(result.Value?.Id));
        }

        [HttpGet("/all")]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation(nameof(GetAllProducts));

            var result = _productsRepository.GetAll();

            return Ok(_mapper.Map<IEnumerable<Product>, IEnumerable<ProductRead>>(result));
        }
    }
}