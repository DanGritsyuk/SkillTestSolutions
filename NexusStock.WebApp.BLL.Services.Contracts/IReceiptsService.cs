using NexusStock.WebApp.Common.Entities.Receipt;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IReceiptsService
    {
        Task<IEnumerable<ReceiptResponse>> GetFilteredReceiptsAsync(ReceiptFilterRequest filter);
        Task CreateReceiptAsync(ReceiptSaveRequest request);
        Task UpdateReceiptAsync(int id, ReceiptSaveRequest request);
        Task DeleteReceiptAsync(int id);
    }
}
