using Microsoft.Extensions.Logging;
using NexusStock.WebApp.BLL.Services.Builders;
using NexusStock.WebApp.BLL.Services.Contracts;
using NexusStock.WebApp.Common.Entities.Receipt;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services
{
    public class ReceiptsService : IReceiptsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReceiptsService> _logger;

        public ReceiptsService(HttpClient httpClient,
            ILogger<ReceiptsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ReceiptResponse>> GetFilteredReceiptsAsync(ReceiptFilterRequest filter)
        {
            const string FORMAT_TIME = "yyyy-MM-ddTHH:mm:ss.fffzzz";

            try
            {
                var url = new QueryBuilder(_httpClient.BaseAddress!, "receipts/get", _logger)
                    .WithNameNormalization()
                    .AddDateParam("startDate", filter.StartDate, FORMAT_TIME)
                    .AddDateParam("endDate", filter.EndDate, FORMAT_TIME)
                    .AddListParam("documentIds", filter.DocumentIds)
                    .AddListParam("resourceIds", filter.ResourceIds)
                    .AddListParam("unitIds", filter.UnitIds)
                    .Build();

                return await _httpClient.GetFromJsonAsync<IEnumerable<ReceiptResponse>>(url)
                    ?? throw new NullReferenceException("Не удалось получить данные о поступлениях");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке документов");
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
