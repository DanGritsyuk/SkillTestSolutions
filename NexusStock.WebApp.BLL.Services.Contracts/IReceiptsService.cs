using NexusStock.WebApp.Common.Entities.Receipt;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IReceiptsService
    {
        Task<IEnumerable<ReceiptResponse>> GetFilteredReceiptsAsync(ReceiptFilterRequest filter);
        Task CreateReceiptAsync(ReceiptCreateRequest request);
        Task UpdateReceiptAsync(ReceiptUpdateRequest request);
        Task DeleteReceiptAsync(int id);
    }
}
