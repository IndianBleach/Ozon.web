using AutoMapper;
using Common.DTOs.Catalog;
using Marketplace.Data.Entities.PropertyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Mappers
{
    public class PropertyMapProfile : Profile
    {
        public PropertyMapProfile()
        {
            CreateMap<CatalogProperty, CatalogPropertyRead>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));
        }
    }
}
