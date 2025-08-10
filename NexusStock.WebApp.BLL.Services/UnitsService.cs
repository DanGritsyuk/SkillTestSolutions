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
                return await _httpClient.GetFromJsonAsync<IEnumerable<UnitResponse>>($"units/get?includeArchived={includeArchived}")
                    ?? throw new NullReferenceException("Не удалось получить данные об единицах измерения. Ответ от сервера был пустым."); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке единиц измерения");
                return Enumerable.Empty<UnitResponse>();
            }
        }

        public async Task CreateUnitAsync(UnitSaveRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("units/create", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task UpdateUnitAsync(int id, UnitSaveRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"units/update/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task ToggleUnitStatusAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"units/toggle-status/{id}", null);

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                throw;
            }
        }
    }
}
