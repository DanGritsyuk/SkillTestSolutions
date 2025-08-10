using Microsoft.Extensions.Logging;
using NexusStock.WebApp.BLL.Services.Contracts;
using NexusStock.WebApp.Common.Entities.Client;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services
{
    public class ClientsService : IClientsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ClientsService> _logger;

        public ClientsService(HttpClient httpClient, ILogger<ClientsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ClientResponse>> GetClientsAsync(bool includeArchived = false)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<ClientResponse>>(
                    $"clients/get?includeArchived={includeArchived}")
                    ?? throw new NullReferenceException("Не удалось получить данные о клиентах. Ответ от сервера был пустым.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке клиентов");
                return Enumerable.Empty<ClientResponse>();
            }
        }

        public async Task CreateClientAsync(ClientSaveRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("clients/create", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task UpdateClientAsync(int id, ClientSaveRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"clients/update/{id}", request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new ApplicationException(error);
            }
        }

        public async Task ToggleClientStatusAsync(int id)
        {
            try
            {
                var response = await _httpClient.PatchAsync($"clients/toggle-status/{id}", null);

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
