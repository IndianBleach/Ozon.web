using AutoMapper;
using Common.DTOs.Catalog;
using Common.Repositories;
using Marketplace.Data.Entities.CatalogEntities;
using Marketplace.Data.Entities.PropertyEntities;
using Marketplace.Infrastructure.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly ILogger<CategoriesController> _logger;

        private readonly IServiceRepository<CatalogCategory> _categoryRepository;

        private readonly IServiceRepository<CategoryProperty> _categoryPropertyRepository;

        public CategoriesController(
            ILogger<CategoriesController> logger,
            IServiceRepository<CatalogCategory> categoryRepository,
            IServiceRepository<CategoryProperty> categoryPropRepository)
        {
            _logger = logger;
            _categoryPropertyRepository = categoryPropRepository;
            _categoryRepository = categoryRepository;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new CatalogMapProfile(),
                new PropertyMapProfile()
            }));

            _mapper = new Mapper(config);
        }

        [HttpGet("/[controller]")]
        public async Task<IActionResult> CreateCategory()
        {
            _logger.LogInformation(nameof(CreateCategory));

            var res = _categoryRepository.GetAll();

            return Ok(_mapper.Map<IEnumerable<CatalogCategory>, IEnumerable<CatalogCategoryRead>>(res));
        }

        [HttpPost("/[controller]/properties")]
        public async Task<IActionResult> CreateCategoryProperty(
            string category_id,
            string prop_id,
            bool required)
        {
            _logger.LogInformation(nameof(CreateCategoryProperty));

            var res = _categoryPropertyRepository.Create(new CategoryProperty(
                categoryId: category_id,
                propertyId: prop_id,
                isRequired: required));

            return Ok(res);
        }

        [HttpPost("/[controller]")]
        public async Task<IActionResult> CreateCategory(
            string catalog_id,
            string name)
        {
            _logger.LogInformation(nameof(CreateCategory));

            var res = _categoryRepository.Create(new CatalogCategory(
                catalogId: catalog_id,
                name: name));

            return Ok(res);
        }
    }
}
