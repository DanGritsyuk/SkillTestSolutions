using Microsoft.Extensions.Logging;
using NexusStock.WebApp.Common.Entities.Unit;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public class UnitsService : IUnitsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UnitsService> _logger;

        public UnitsService(HttpClient httpClient, ILogger<UnitsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<UnitResponse>> GetUnitsAsync(bool includeArchived = false)
        {
            try
            {
                _logger.LogDebug($"Request URL: {_httpClient.BaseAddress}api/Units/get?includeArchived={includeArchived}");
                return await _httpClient.GetFromJsonAsync<IEnumerable<UnitResponse>>(
                    $"api/units/get?includeArchived={includeArchived}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке единиц измерения");
                return Enumerable.Empty<UnitResponse>();
            }
        }

        public async Task CreateUnitAsync(UnitCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/units/create", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task UpdateUnitAsync(UnitUpdateRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/units/update/{request.Id}", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task ToggleUnitStatusAsync(int id)
        {
            var response = await _httpClient.PatchAsync(
                "api/units/toggle-status",
                JsonContent.Create(new { Id = id })
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }
    }
}
