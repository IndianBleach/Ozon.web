using AutoMapper;
using Common.DTOs.Catalog;
using Common.Repositories;
using Marketplace.Data.Entities.CatalogEntities;
using Marketplace.Data.Entities.PropertyEntities;
using Marketplace.Infrastructure.Mappers;
using Marketplace.Infrastructure.Specifications.Properties;
using Marketplace.Infrastructure.Specifications.Sections;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    //[ApiController]
    [Route("/[controller]")]
    [Produces("application/json")]
    public class SectionsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly ILogger<SectionsController> _logger;

        private readonly IServiceRepository<CategorySection> _sectionRepository;

        private readonly IServiceRepository<SectionProperty> _sectionPropRepository;

        public SectionsController(
            ILogger<SectionsController> logger,
            IServiceRepository<CategorySection> sectionRepository,
            IServiceRepository<SectionProperty> sectionPropertyRepository)
        {
            _sectionRepository = sectionRepository;
            _sectionPropRepository = sectionPropertyRepository;
            _logger = logger;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new CatalogMapProfile(),
                new PropertyMapProfile()
            }));

            _mapper = new Mapper(config);
        }

        [HttpGet("/[controller]")]
        public async Task<IActionResult> GetSections()
        {
            _logger.LogInformation(nameof(GetSections));

            var dtos = _sectionRepository.Find(new AllSectionsIncludeCategorySpec());

            return Ok(_mapper.Map<IEnumerable<CategorySection>, IEnumerable<CatalogSectionRead>>(dtos));
        }

        [HttpPost("/[controller]/properties")]
        public async Task<IActionResult> CreateSectionProperty(
            string section_id,
            string prop_id,
            bool required)
        {
            _logger.LogInformation(nameof(CreateSectionProperty));

            var res = _sectionPropRepository.Create(new SectionProperty(
                sectionId: section_id,
                propertyId: prop_id,
                isRequired: required));

            return Ok(res);
        }

        [HttpPost("/[controller]")]
        public async Task<IActionResult> CreateSection(
            string category_id,
            string name)
        {
            _logger.LogInformation(nameof(CreateSection));

            var res = _sectionRepository.Create(new CategorySection(
                categoryId: category_id,
                name: name));

            _logger.LogWarning($"[sectionId] " + res.Value?.CategoryId);

            return Ok(res);
        }
    }
}
