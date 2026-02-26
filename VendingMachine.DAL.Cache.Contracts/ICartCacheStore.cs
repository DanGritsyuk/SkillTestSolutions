using VendingMachine.Common.Entities;

namespace VendingMachine.DAL.Cache.Contracts
{
    public interface ICartCacheStore
    {
        Task<ClientCart?> GetAsync(Guid cartId, CancellationToken cancellationToken = default);
        Task SetAsync(ClientCart cart, TimeSpan? ttl = null, CancellationToken cancellationToken = default);
        Task ClearAsync(Guid cartId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid cartId, CancellationToken cancellationToken = default);
    }
}
