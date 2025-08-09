using Microsoft.Extensions.Logging;
using NexusStock.WebApp.BLL.Services.Builders;
using NexusStock.WebApp.BLL.Services.Contracts;
using NexusStock.WebApp.Common.Entities.Shipment;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services
{
    public class ShipmentsService : IShipmentsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ShipmentsService> _logger;

        public ShipmentsService(HttpClient httpClient, ILogger<ShipmentsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

        }

        public async Task<IEnumerable<ShipmentResponse>> GetFilteredShipmentsAsync(ShipmentFilterRequest filter)
        {
            try
            {
                var builder = new QueryBuilder(_httpClient.BaseAddress!, "shipments/get", _logger)
                    .WithNameNormalization()
                    .AddDateParam("startDate", filter.StartDate)
                    .AddDateParam("endDate", filter.EndDate)
                    .AddListParam("clientIds", filter.ClientIds)
                    .AddListParam("resourceIds", filter.ResourceIds)
                    .AddListParam("unitIds", filter.UnitIds);

                //if (filter.IsSigned.HasValue)
                //{
                //    builder.AddBoolParam("isSigned", filter.IsSigned.Value);
                //}

                var url = builder.Build();

                return await _httpClient.GetFromJsonAsync<IEnumerable<ShipmentResponse>>(url)
                    ?? throw new NullReferenceException("Не удалось получить данные об отгрузках");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке отгрузок");
                return Enumerable.Empty<ShipmentResponse>();
            }
        }

        public async Task CreateShipmentAsync(ShipmentCreateRequest request)
        {
            await _httpClient.PostAsJsonAsync("shipments/create", request);
        }

        public async Task UpdateShipmentAsync(ShipmentUpdateRequest request)
        {
            await _httpClient.PutAsJsonAsync($"shipments/update/{request.Id}", request);
        }

        public async Task DeleteShipmentAsync(int id)
        {
            await _httpClient.DeleteAsync($"shipments/delete/{id}");
        }

        public async Task SignShipmentAsync(int id)
        {
            await _httpClient.PatchAsync("shipments/sign",
                JsonContent.Create(new ShipmentSignRequest { Id = id }));
        }

        public async Task RevokeShipmentAsync(int id)
        {
            await _httpClient.PatchAsync("shipments/revoke",
                JsonContent.Create(new ShipmentSignRequest { Id = id }));
        }
    }
}
