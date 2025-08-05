using NexusStock.Common.Entities;
using System.Linq.Expressions;

namespace NexusStock.DAL.Repository.Contracts
{
    public interface IShipmentDocumentRepository : IRepository<ShipmentDocument>
    {
        Task<ShipmentDocument> GetWithItemsAndClientAsync(int id);
        Task<IEnumerable<ShipmentDocument>> GetFilteredWithDetailsAsync(Expression<Func<ShipmentDocument, bool>> filter = null);
        Task<bool> IsNumberUniqueAsync(string number, int? excludeId = null);
    }
}
