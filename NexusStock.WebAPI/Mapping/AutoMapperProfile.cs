using AutoMapper;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Client;
using NexusStock.WebAPI.DTOs.Receipt;
using NexusStock.WebAPI.DTOs.Resource;
using NexusStock.WebAPI.DTOs.Shipment;
using NexusStock.WebAPI.DTOs.Stock;
using NexusStock.WebAPI.DTOs.Unit;

namespace NexusStock.WebAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Клиенты
            CreateMap<Client, ClientResponse>();
            CreateMap<ClientCreateRequest, Client>();
            CreateMap<ClientUpdateRequest, Client>();

            // Ресурсы
            CreateMap<Resource, ResourceResponse>();
            CreateMap<ResourceCreateRequest, Resource>();
            CreateMap<ResourceUpdateRequest, Resource>();

            // Единицы измерения
            CreateMap<Unit, UnitResponse>();
            CreateMap<UnitCreateRequest, Unit>();
            CreateMap<UnitUpdateRequest, Unit>();

            // Поступления
            CreateMap<ReceiptDocument, ReceiptResponse>();
            CreateMap<ReceiptItem, ReceiptItemResponse>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));

            CreateMap<ReceiptCreateRequest, ReceiptDocument>();
            CreateMap<ReceiptUpdateRequest, ReceiptDocument>();
            CreateMap<ReceiptItemRequest, ReceiptItem>();

            // Отгрузки
            CreateMap<ShipmentDocument, ShipmentResponse>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name));

            CreateMap<ShipmentItem, ShipmentItemResponse>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));

            CreateMap<ShipmentCreateRequest, ShipmentDocument>();
            CreateMap<ShipmentUpdateRequest, ShipmentDocument>();
            CreateMap<ShipmentItemRequest, ShipmentItem>();

            // Склад
            CreateMap<StockBalance, StockBalanceResponse>()
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));
        }
    }
}
