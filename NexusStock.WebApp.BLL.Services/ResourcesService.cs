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
                    $"api/resources/get?includeArchived={includeArchived}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке ресурсов");
                return Enumerable.Empty<ResourceResponse>();
            }
        }

        public async Task CreateResourceAsync(ResourceCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/resources/create", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task UpdateResourceAsync(ResourceUpdateRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/resources/update/{request.Id}", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task ToggleResourceStatusAsync(int id)
        {
            var response = await _httpClient.PatchAsync(
                "api/resources/toggle-status",
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
