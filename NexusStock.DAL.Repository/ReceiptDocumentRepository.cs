using Microsoft.EntityFrameworkCore;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

namespace NexusStock.DAL.Repository
{
    public class ReceiptDocumentRepository : Repository<ReceiptDocument>, IReceiptDocumentRepository
    {
        public ReceiptDocumentRepository(NexusStockDbContext context) : base(context) { }

        public async Task<ReceiptDocument> GetWithItemsAsync(int id)
        {
            return await _context.ReceiptDocuments
                .Include(rd => rd.Items)
                .ThenInclude(i => i.Resource)
                .Include(rd => rd.Items)
                .ThenInclude(i => i.Unit)
                .FirstOrDefaultAsync(rd => rd.Id == id);
        }

        public async Task<IEnumerable<ReceiptDocument>> GetByPeriodAsync(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _context.ReceiptDocuments
                .Where(rd => rd.Date >= startDate && rd.Date <= endDate)
                .Include(rd => rd.Items)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocument>> GetFilteredWithDetailsAsync(
            Expression<Func<ReceiptDocument, bool>> filter = null)
        {
            IQueryable<ReceiptDocument> query = _context.ReceiptDocuments
                .Include(rd => rd.Items)
                    .ThenInclude(i => i.Resource)
                .Include(rd => rd.Items)
                    .ThenInclude(i => i.Unit);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> IsNumberUniqueAsync(string number, int? excludeId = null)
        {
            return await _context.ReceiptDocuments
                .Where(rd => rd.Number == number && (excludeId == null || rd.Id != excludeId.Value))
                .AnyAsync();
        }
    }
}
