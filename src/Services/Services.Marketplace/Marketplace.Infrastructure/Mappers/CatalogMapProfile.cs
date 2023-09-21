using AutoMapper;
using Common.DTOs.Catalog;
using Google.Protobuf.Compiler;
using Marketplace.Data.Entities.CatalogEntities;
using Marketplace.Data.Entities.ProductsEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Mappers
{
    public class CatalogMapProfile : Profile
    {
        public CatalogMapProfile()
        {
            CreateMap<CatalogProduct, CatalogProductShortRead>()
                .ForMember(dest => dest.MarketplaceProductId, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.ExternalProductId, opt => opt.MapFrom(x => x.ExternalBaseProductId));

            CreateMap<Catalog, CatalogRead>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));

            CreateMap<CatalogCategory, CatalogCategoryRead>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));

            CreateMap<CategorySection, CatalogSectionRead>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(dest => dest.InCategoryId, opt => opt.MapFrom(x => x.Category != null ? x.Category.Id : string.Empty));
        }
    }
}
