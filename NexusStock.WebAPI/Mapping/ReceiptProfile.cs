using AutoMapper;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Receipt;

namespace NexusStock.WebAPI.Mapping
{
    public class ReceiptProfile : Profile
    {
        public ReceiptProfile()
        {
            CreateMap<ReceiptDocument, ReceiptResponse>();
            CreateMap<ReceiptItem, ReceiptItemResponse>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));

            CreateMap<ReceiptCreateRequest, ReceiptDocument>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));

            CreateMap<ReceiptItemRequest, ReceiptItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiptDocumentId, opt => opt.Ignore())
                .ForMember(dest => dest.ReceiptDocument, opt => opt.Ignore())
                .ForMember(dest => dest.Resource, opt => opt.Ignore())
                .ForMember(dest => dest.Unit, opt => opt.Ignore());

            CreateMap<ReceiptUpdateRequest, ReceiptDocument>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));
        }
    }
}