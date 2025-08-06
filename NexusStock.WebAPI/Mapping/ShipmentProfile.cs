using AutoMapper;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Shipment;

namespace NexusStock.WebAPI.Mapping
{
    public class ShipmentProfile : Profile
    {
        public ShipmentProfile()
        {
            CreateMap<ShipmentDocument, ShipmentResponse>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name));

            CreateMap<ShipmentItem, ShipmentItemResponse>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));

            CreateMap<ShipmentCreateRequest, ShipmentDocument>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsSigned, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.Client, opt => opt.Ignore());

            CreateMap<ShipmentItemRequest, ShipmentItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentDocumentId, opt => opt.Ignore())
                .ForMember(dest => dest.ShipmentDocument, opt => opt.Ignore())
                .ForMember(dest => dest.Resource, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore());

            CreateMap<ShipmentUpdateRequest, ShipmentDocument>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsSigned, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore());
        }
    }
}