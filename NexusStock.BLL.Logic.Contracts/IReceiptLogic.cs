using NexusStock.Common.Entities;

namespace NexusStock.BLL.Logic.Contracts
{
    public interface IReceiptLogic
    {
        Task CreateReceiptAsync(ReceiptDocument document);
        Task UpdateReceiptAsync(ReceiptDocument document);
        Task DeleteReceiptAsync(int id);
        Task<ReceiptDocument> GetReceiptWithItemsAsync(int id);
        Task<IEnumerable<ReceiptDocument>> GetFilteredReceiptsAsync(
            DateTime? startDate,
            DateTime? endDate,
            IEnumerable<int> documentIds,
            IEnumerable<int> resourceIds,
            IEnumerable<int> unitIds);
    }
}
