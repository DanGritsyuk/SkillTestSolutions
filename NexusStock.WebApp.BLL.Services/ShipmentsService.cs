using NexusStock.WebApp.BLL.Services.Contracts;
using NexusStock.WebApp.Common.Entities.Shipment;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services
{
    public class ShipmentsService : IShipmentsService
    {
        private readonly HttpClient _httpClient;

        public ShipmentsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ShipmentResponse>> GetFilteredShipmentsAsync(ShipmentFilterRequest filter)
        {
            try
            {
                var queryParams = new Dictionary<string, string>();

                if (filter.StartDate.HasValue)
                    queryParams.Add("startDate", filter.StartDate.Value.ToString("yyyy-MM-dd"));

                if (filter.EndDate.HasValue)
                    queryParams.Add("endDate", filter.EndDate.Value.ToString("yyyy-MM-dd"));

                if (filter.ClientIds?.Count > 0)
                    queryParams.Add("clientIds", string.Join(",", filter.ClientIds));

                if (filter.ResourceIds?.Count > 0)
                    queryParams.Add("resourceIds", string.Join(",", filter.ResourceIds));

                if (filter.UnitIds?.Count > 0)
                    queryParams.Add("unitIds", string.Join(",", filter.UnitIds));

                //if (filter.IsSigned.HasValue)
                //    queryParams.Add("isSigned", filter.IsSigned.Value.ToString());

                var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
                return await _httpClient.GetFromJsonAsync<IEnumerable<ShipmentResponse>>(
                    $"api/shipments/get?{queryString}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке документов: {ex.Message}");
                return Enumerable.Empty<ShipmentResponse>();
            }
        }

        public async Task CreateShipmentAsync(ShipmentCreateRequest request)
        {
            await _httpClient.PostAsJsonAsync("api/shipments/create", request);
        }

        public async Task UpdateShipmentAsync(ShipmentUpdateRequest request)
        {
            await _httpClient.PutAsJsonAsync($"api/shipments/update/{request.Id}", request);
        }

        public async Task DeleteShipmentAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/shipments/delete/{id}");
        }

        public async Task SignShipmentAsync(int id)
        {
            await _httpClient.PatchAsync("api/shipments/sign",
                JsonContent.Create(new ShipmentSignRequest { Id = id }));
        }

        public async Task RevokeShipmentAsync(int id)
        {
            await _httpClient.PatchAsync("api/shipments/revoke",
                JsonContent.Create(new ShipmentSignRequest { Id = id }));
        }
    }
}
