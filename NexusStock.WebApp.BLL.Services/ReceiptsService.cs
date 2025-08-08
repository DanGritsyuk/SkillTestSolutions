using NexusStock.WebApp.BLL.Services.Contracts;
using NexusStock.WebApp.Common.Entities.Receipt;
using System.Net.Http;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services
{
    public class ReceiptsService : IReceiptsService
    {
        private readonly HttpClient _httpClient;

        public ReceiptsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ReceiptResponse>> GetFilteredReceiptsAsync(ReceiptFilterRequest filter)
        {
            try
            {
                // Сбор параметров запроса
                var queryParams = new Dictionary<string, string>();

                if (filter.StartDate.HasValue)
                    queryParams.Add("startDate", filter.StartDate.Value.ToString("yyyy-MM-dd"));

                if (filter.EndDate.HasValue)
                    queryParams.Add("endDate", filter.EndDate.Value.ToString("yyyy-MM-dd"));

                if (filter.ResourceIds?.Count > 0)
                    queryParams.Add("resourceIds", string.Join(",", filter.ResourceIds));

                if (filter.UnitIds?.Count > 0)
                    queryParams.Add("unitIds", string.Join(",", filter.UnitIds));
                
                var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
                return await _httpClient.GetFromJsonAsync<IEnumerable<ReceiptResponse>>(
                    $"receipts/get?{queryString}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке документов: {ex.Message}");
                return Enumerable.Empty<ReceiptResponse>();
            }
        }

        public async Task CreateReceiptAsync(ReceiptCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("receipts/create", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateReceiptAsync(ReceiptUpdateRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"receipts/update/{request.Id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteReceiptAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"receipts/delete/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
