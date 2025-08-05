using Microsoft.EntityFrameworkCore;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;

namespace NexusStock.DAL.Repository
{
    public class StockBalanceRepository : Repository<StockBalance>, IStockBalanceRepository
    {
        public StockBalanceRepository(NexusStockDbContext context) : base(context) { }

        public async Task<IEnumerable<StockBalance>> GetAllWithDetailsAsync()
        {
            return await _context.StockBalances
                .Include(b => b.Resource)
                .Include(b => b.Unit)
                .ToListAsync();
        }
    }
}
