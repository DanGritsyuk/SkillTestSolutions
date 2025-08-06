using Microsoft.EntityFrameworkCore;
using NexusStock.Common.Entities;
using NexusStock.DAL.Repository.Contracts;

namespace NexusStock.DAL.Repository
{
    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        public ResourceRepository(NexusStockDbContext context) : base(context) { }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return !await _dbSet
                .Where(r => r.Name == name && (excludeId == null || r.Id != excludeId.Value))
                .AnyAsync();
        }

        public async Task<bool> IsUsedInDocumentsAsync(int resourceId)
        {
            return await _context.ReceiptItems.AnyAsync(ri => ri.ResourceId == resourceId)
                || await _context.ShipmentItems.AnyAsync(si => si.ResourceId == resourceId);
        }

        public async Task<bool> IsUsedInReceiptItemsAsync(int resourceId)
        {
            return await _context.ReceiptItems
                .AnyAsync(ri => ri.ResourceId == resourceId);
        }

        public async Task<bool> IsUsedInShipmentItemsAsync(int resourceId)
        {
            return await _context.ShipmentItems
                .AnyAsync(si => si.ResourceId == resourceId);
        }
    }
}
