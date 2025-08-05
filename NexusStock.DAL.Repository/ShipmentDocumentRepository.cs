using Microsoft.EntityFrameworkCore;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;
using System.Linq.Expressions;

namespace NexusStock.DAL.Repository
{
    public class ShipmentDocumentRepository : Repository<ShipmentDocument>, IShipmentDocumentRepository
    {

        public ShipmentDocumentRepository(NexusStockDbContext context) : base(context) { }

        public async Task<ShipmentDocument> GetWithItemsAndClientAsync(int id)
        {
            return await _context.ShipmentDocuments
                .Include(sd => sd.Client)
                .Include(sd => sd.Items)
                    .ThenInclude(i => i.Resource)
                .Include(sd => sd.Items)
                    .ThenInclude(i => i.Unit)
                .FirstOrDefaultAsync(sd => sd.Id == id);
        }

        public async Task<IEnumerable<ShipmentDocument>> GetFilteredWithDetailsAsync(
            Expression<Func<ShipmentDocument, bool>> filter = null)
        {
            IQueryable<ShipmentDocument> query = _context.ShipmentDocuments
                .Include(sd => sd.Client)
                .Include(sd => sd.Items)
                    .ThenInclude(i => i.Resource)
                .Include(sd => sd.Items)
                    .ThenInclude(i => i.Unit);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> IsNumberUniqueAsync(string number, int? excludeId = null)
        {
            return await _context.ShipmentDocuments
                .AnyAsync(sd => sd.Number == number && (excludeId == null || sd.Id != excludeId.Value));
        }
    }
}
