using NexusStock.Common.Entities;

namespace NexusStock.BLL.Logic.Contracts
{
    public interface IStockLogic
    {
        Task<IEnumerable<StockBalance>> GetFilteredStockBalancesAsync(
            IEnumerable<int> resourceIds,
            IEnumerable<int> unitIds);
    }
}
