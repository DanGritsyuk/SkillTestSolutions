using AutoMapper;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Resource;

namespace NexusStock.WebAPI.Mapping
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {
            CreateMap<Resource, ResourceResponse>();

            CreateMap<ResourceCreateRequest, Resource>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Balances, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiptItems, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentItems, opt => opt.Ignore());

            CreateMap<ResourceUpdateRequest, Resource>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Balances, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiptItems, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentItems, opt => opt.Ignore());
        }
    }
}