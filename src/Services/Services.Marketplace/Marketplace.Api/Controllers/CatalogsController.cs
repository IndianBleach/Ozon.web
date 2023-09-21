using AutoMapper;
using Common.DTOs.Catalog;
using Common.Repositories;
using Common.Specifications;
using Marketplace.Data.Entities.CatalogEntities;
using Marketplace.Data.Entities.ProductsEntities;
using Marketplace.Data.Entities.PropertyEntities;
using Marketplace.Infrastructure.Mappers;
using Marketplace.Infrastructure.Specifications.Properties;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Marketplace.Api.Controllers
{
    //[ApiController]
    [Route("/")]
    [Produces("application/json")]
    public class CatalogsController : ControllerBase
    {
        private readonly ILogger<CatalogsController> _logger;

        private readonly IServiceRepository<Catalog> _catalogRepository;

        private readonly IServiceRepository<CatalogProperty> _propertyRepository;

        private readonly IServiceRepository<CatalogPropertyUnit> _propertyUnitRepository;

        private readonly IServiceRepository<SectionProperty> _sectionPropertyRepository;

        private readonly IServiceRepository<CategoryProperty> _categoryPropertyRepository;

        private readonly Mapper _mapper;

        public CatalogsController(
            IServiceRepository<CatalogProperty> propertyRepository,
            IServiceRepository<Catalog> catalogRepository,
            IServiceRepository<CatalogPropertyUnit> propertyUnitRepository,
            IServiceRepository<SectionProperty> sectionPropertyRepository,
            IServiceRepository<CategoryProperty> categoryPropertyRepository,
            ILogger<CatalogsController> logger)
        {
            _sectionPropertyRepository = sectionPropertyRepository;
            _categoryPropertyRepository = categoryPropertyRepository;
            _propertyRepository = propertyRepository;
            _catalogRepository = catalogRepository;
            _propertyUnitRepository = propertyUnitRepository;
            _logger = logger;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> { 
                new CatalogMapProfile(),
                new PropertyMapProfile()
            }));

            _mapper = new Mapper(config);
        }




        [HttpGet("/")]
        public async Task<IActionResult> GetCatalogs()
        {
            _logger.LogInformation(nameof(GetCatalogs));

            var dtos = _catalogRepository.GetAll();

            return Ok(_mapper.Map<IEnumerable<Catalog>, IEnumerable<CatalogRead>>(dtos));
        }


        [HttpGet("/properties/units")]
        public async Task<IActionResult> GetPropertyUnits()
        {
            _logger.LogInformation(nameof(GetPropertyUnits));

            var reuslt = _propertyUnitRepository.GetAll();

            return Ok(reuslt);
        }

        [HttpGet("/properties")]
        public async Task<IActionResult> GetProperties(
           string? section_id,
           string? category_id)
        {
            _logger.LogInformation(nameof(GetProperties));

            IEnumerable<CatalogProperty?> props;

            if (!string.IsNullOrEmpty(section_id) &&
                !string.IsNullOrEmpty(category_id))
            {
                props = Enumerable.Concat<CatalogProperty?>(
                    first: _categoryPropertyRepository.Find(
                        specification: new PropertiesOfCategorySpec(
                            categoryId: category_id,
                            onlyRequired: true))
                        .Select(x => x.Property),
                    second: _sectionPropertyRepository.Find(
                        specification: new PropertiesOfSectionSpec(
                            sectionId: section_id,
                            onlyRequired: true))
                        .Select(x => x.Property));

                props = props.Where(x => x != null);
            }
            else if (!string.IsNullOrEmpty(section_id))
            {
                props = _sectionPropertyRepository.Find(
                    specification: new PropertiesOfSectionSpec(
                        sectionId: section_id,
                        onlyRequired: true))
                    .Select(x => x.Property);
            }
            else if (!string.IsNullOrEmpty(category_id))
            {
                props = _categoryPropertyRepository.Find(
                    specification: new PropertiesOfCategorySpec(
                        categoryId: category_id,
                        onlyRequired: true))
                    .Select(x => x.Property);
            }
            else props = _propertyRepository.GetAll();

            return Ok(_mapper.Map<IEnumerable<CatalogProperty>, IEnumerable<CatalogPropertyRead>>(props));
        }

        [HttpPost("/")]
        public async Task<IActionResult> CreateCatalog(
            string name)
        {
            _logger.LogInformation(nameof(CreateCatalog));

            var res = _catalogRepository.Create(new Catalog(name));

            return Ok(res);
        }

        [HttpPost("/properties/units")]
        public async Task<IActionResult> CreatePropertyUnit(
            string name)
        {
            _logger.LogInformation(nameof(CreatePropertyUnit));

            var res = _propertyUnitRepository.Create(new CatalogPropertyUnit(
                name: name));

            return Ok(res);
        }

        [HttpPost("/properties")]
        public async Task<IActionResult> CreateProperty(
            string name,
            string unit_id,
            string desc)
        {
            _logger.LogInformation(nameof(CreateProperty));

            var res = _propertyRepository.Create(new CatalogProperty(
                name: name,
                unitId: unit_id,
                description: desc));

            return Ok(res);
        }
    }
}