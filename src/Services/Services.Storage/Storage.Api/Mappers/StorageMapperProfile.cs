using AutoMapper;
using Common.DTOs.Storage;
using Storage.Data.Entities.Actions;
using Storage.Data.Entities.Products;
using Storage.Data.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Mappers
{
    public class StorageMapperProfile : Profile
    {
        public StorageMapperProfile()
        {
            CreateMap<StorageCell, StorageCellRead>()
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.CellNumber))
                .ForMember(dest => dest.StorageId, src => src.MapFrom(x => x.StorageId));

            CreateMap<StorageActionType, StorageActionRead>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name));

            CreateMap<StorageProduct, StorageProductRead>()
                .ForMember(dest => dest.StorageProductId, opt => opt.MapFrom(x => x.Id))
                .ForMember(dest => dest.MarketplaceProductId, opt => opt.MapFrom(x => x.MarketplaceProductId))
                .ForMember(dest => dest.InStorageId, opt => opt.MapFrom(x => x.StorageId));

            CreateMap<MarketStorage, StorageRead>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));
        }
    }
}
