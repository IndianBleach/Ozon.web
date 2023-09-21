using AutoMapper;
using Ozon.Bus.DTOs.StorageService;
using Storage.Data.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Api.Mappers.KafkaBus
{
    public class MarketplaceProductToStorageProductProfile : Profile
    {
        public MarketplaceProductToStorageProductProfile()
        {
            CreateMap<MarketplaceProductStorageRegistrationRead, StorageProduct>()
                .ForCtorParam("marketplaceProductId", src => src.MapFrom(x => x.MarketplaceProductId))
                .ForCtorParam("storageId", src => src.MapFrom(x => x.StorageId))
                .ForCtorParam("dateAdded", src => src.MapFrom(_ => DateTime.Now));
        }
    }
}
