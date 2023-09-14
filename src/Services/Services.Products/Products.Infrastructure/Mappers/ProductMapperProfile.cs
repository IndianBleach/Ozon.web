using AutoMapper;
using Common.DTOs.Products;
using Products.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Infrastructure.Mappers
{
    public class ProductMapperProfile : Profile
    {
        public ProductMapperProfile()
        {
            CreateMap<Product, ProductRead>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Title))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(x => x.Id));
        }
    }
}
