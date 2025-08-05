using NexusStock.Common.Entities;

namespace NexusStock.DAL.Repository.Contracts
{
    public interface IStockBalanceRepository : IRepository<StockBalance>
    {
        Task<IEnumerable<StockBalance>> GetAllWithDetailsAsync();
    }
}
