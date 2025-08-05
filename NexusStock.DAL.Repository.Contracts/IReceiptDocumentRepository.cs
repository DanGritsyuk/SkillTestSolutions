using NexusStock.Common.Entities;
using System.Linq.Expressions;

namespace NexusStock.DAL.Repository.Contracts
{
    public interface IReceiptDocumentRepository : IRepository<ReceiptDocument>
    {
        Task<ReceiptDocument> GetWithItemsAsync(int id);
        Task<IEnumerable<ReceiptDocument>> GetByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ReceiptDocument>> GetFilteredWithDetailsAsync(Expression<Func<ReceiptDocument, bool>> filter = null);
        Task<bool> IsNumberUniqueAsync(string number, int? excludeId = null);
    }
}
