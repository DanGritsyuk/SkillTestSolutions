using NexusStock.WebApp.BLL.Services.Contracts;
using NexusStock.WebApp.Common.Entities.Balance;
using System.Net.Http.Json;

namespace NexusStock.WebApp.BLL.Services
{
    public class StockBalanceService : IStockBalanceService
    {
        private readonly HttpClient _httpClient;

        public StockBalanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<StockBalanceResponse>> GetFilteredStockBalancesAsync(
            IEnumerable<int> resourceIds,
            IEnumerable<int> unitIds)
        {
            try
            {
                var queryParams = new Dictionary<string, string>();

                if (resourceIds?.Count() > 0)
                    queryParams.Add("resourceIds", string.Join(",", resourceIds));

                if (unitIds?.Count() > 0)
                    queryParams.Add("unitIds", string.Join(",", unitIds));

                var queryString = new FormUrlEncodedContent(queryParams).ReadAsStringAsync();
                return await _httpClient.GetFromJsonAsync<IEnumerable<StockBalanceResponse>>(
                    $"api/stock?{queryString}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке складских остатков: {ex.Message}");
                return Enumerable.Empty<StockBalanceResponse>();
            }
        }
    }
}
