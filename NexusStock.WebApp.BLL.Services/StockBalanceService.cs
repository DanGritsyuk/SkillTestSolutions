using Microsoft.Extensions.Logging;
using NexusStock.WebApp.BLL.Services.Builders;
using NexusStock.WebApp.BLL.Services.Contracts;
using NexusStock.WebApp.Common.Entities.Balance;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services
{
    public class StockBalanceService : IStockBalanceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReceiptsService> _logger;

        public StockBalanceService(HttpClient httpClient,
            ILogger<ReceiptsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<StockBalanceResponse>> GetFilteredStockBalancesAsync(
            IEnumerable<int> resourceIds,
            IEnumerable<int> unitIds)
        {
            try
            {
                var url = new QueryBuilder(_httpClient.BaseAddress!, "stock/get", _logger)
                    .AddListParam("resourceIds", resourceIds)
                    .AddListParam("unitIds", unitIds)
                    .Build();

                return await _httpClient.GetFromJsonAsync<IEnumerable<StockBalanceResponse>>(url)
                    ?? throw new NullReferenceException("Не удалось получить данные о остатках на складе. Ответ от сервера был пустым.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при загрузке складских остатков: {ex.Message}");
                return Enumerable.Empty<StockBalanceResponse>();
            }
        }
    }
}
