using VendingMachine.Common.Entities;
using VendingMachine.Common.Entities.Enums;

namespace VendingMachine.BLL.Logic.Contracts
{
    public interface IPurchaseLogic
    {
        Task<ClientCart> CreateCartAsync(TimeSpan? ttl = null, CancellationToken cancellationToken = default);
        Task<ClientCart> GetCartAsync(Guid cartId, CancellationToken cancellationToken = default);
        Task<ClientCart> AddCoinAsync(Guid cartId, CoinDenomination denomination, TimeSpan? ttl = null, CancellationToken cancellationToken = default);
        Task<ClientCart> AddDrinkAsync(Guid cartId, int drinkId, int quantity = 1, TimeSpan? ttl = null, CancellationToken cancellationToken = default);
        Task CancelAsync(Guid cartId, CancellationToken cancellationToken = default);
        Task<PurchaseCheckoutResult> CheckoutAsync(Guid cartId, CancellationToken cancellationToken = default);
    }
}
