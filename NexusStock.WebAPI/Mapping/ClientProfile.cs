using AutoMapper;
using NexusStock.Common.Entities;
using NexusStock.WebAPI.DTOs.Client;

namespace NexusStock.WebAPI.Mapping
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientResponse>();

            CreateMap<ClientCreateRequest, Client>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Shipments, opt => opt.Ignore());

            CreateMap<ClientUpdateRequest, Client>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Shipments, opt => opt.Ignore());
        }
    }
}