using AutoMapper;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Stock;

namespace NexusStock.WebAPI.Mapping
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<StockBalance, StockBalanceResponse>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));
        }
    }
}
