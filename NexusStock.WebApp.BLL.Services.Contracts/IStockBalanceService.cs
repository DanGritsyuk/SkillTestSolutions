using NexusStock.WebApp.Common.Entities.Balance;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IStockBalanceService
    {
        Task<IEnumerable<StockBalanceResponse>> GetFilteredStockBalancesAsync(IEnumerable<int> resourceIds, IEnumerable<int> unitIds);
    }
}
