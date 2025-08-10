using Microsoft.EntityFrameworkCore;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

namespace NexusStock.DAL.Repository
{
    public class StockBalanceRepository : Repository<StockBalance>, IStockBalanceRepository
    {
        public StockBalanceRepository(NexusStockDbContext context) : base(context) { }

        public async Task<IEnumerable<StockBalance>> GetAllWithDetailsAsync()
        {
            return await GetBaseQuery()
                .ToListAsync();
        }

        public async Task<IEnumerable<StockBalance>> GetFilteredWithDetailsAsync(Expression<Func<StockBalance, bool>> filter)
        {
            return await GetBaseQuery()
                .Where(filter)
                .AsNoTracking()
                .ToListAsync();
        }

        private IQueryable<StockBalance> GetBaseQuery()
        {
            return _context.StockBalances
                .Include(b => b.Resource)
                .Include(b => b.Unit)
                .AsNoTracking();
        }
    }
}
