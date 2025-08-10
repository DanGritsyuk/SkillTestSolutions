using Microsoft.Extensions.Logging;
using NexusStock.WebApp.BLL.Services.Contracts;
using NexusStock.WebApp.Common.Entities.Resources;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services
{
    public class ResourcesService : IResourcesService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ResourcesService> _logger;

        public ResourcesService(HttpClient httpClient, ILogger<ResourcesService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ResourceResponse>> GetResourcesAsync(bool includeArchived = false)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<ResourceResponse>>(
                    $"resources/get?includeArchived={includeArchived}")
                    ?? throw new NullReferenceException("Не удалось получить данные о клиентах. Ответ от сервера был пустым.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке ресурсов");
                return Enumerable.Empty<ResourceResponse>();
            }
        }

        public async Task CreateResourceAsync(ResourceSaveRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("resources/create", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task UpdateResourceAsync(int id, ResourceSaveRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"resources/update/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task ToggleResourceStatusAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"resources/toggle-status/{id}", null);

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
