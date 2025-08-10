using NexusStock.Common.Entities;
using System.Linq.Expressions;

namespace NexusStock.DAL.Repository.Contracts
{
    public interface IStockBalanceRepository : IRepository<StockBalance>
    {
        Task<IEnumerable<StockBalance>> GetAllWithDetailsAsync();
        Task<IEnumerable<StockBalance>> GetFilteredWithDetailsAsync(Expression<Func<StockBalance, bool>> filter);
    }
}
