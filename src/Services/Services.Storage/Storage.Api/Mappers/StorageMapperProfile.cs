using AutoMapper;
using Common.DTOs.Storage;
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
            CreateMap<MarketStorage, StorageRead>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.Id));
        }
    }
}
