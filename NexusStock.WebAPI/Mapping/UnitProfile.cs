using AutoMapper;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Unit;

namespace NexusStock.WebAPI.Mapping
{
    public class UnitProfile : Profile
    {
        public UnitProfile()
        {
            CreateMap<Unit, UnitResponse>();

            CreateMap<UnitCreateRequest, Unit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Balances, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiptItems, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentItems, opt => opt.Ignore());

            CreateMap<UnitUpdateRequest, Unit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Balances, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiptItems, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentItems, opt => opt.Ignore());
        }
    }
}