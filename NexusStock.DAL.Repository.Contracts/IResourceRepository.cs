using NexusStock.Common.Entities;

namespace NexusStock.DAL.Repository.Contracts
{
    public interface IResourceRepository : IRepository<Resource>
    {
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<bool> IsUsedInDocumentsAsync(int resourceId);
        Task<bool> IsUsedInReceiptItemsAsync(int resourceId);
        Task<bool> IsUsedInShipmentItemsAsync(int resourceId);
    }
}
