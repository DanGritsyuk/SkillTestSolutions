using VendingMachine.Common.Entities;

namespace VendingMachine.BLL.Logic.Contracts
{
    public sealed class PurchaseCheckoutResult
    {
        public Guid CartId { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal OrderAmount { get; init; }
        public decimal ChangeAmount { get; init; }
        public IReadOnlyCollection<Drink> PurchasedItems { get; init; } = Array.Empty<Drink>();
        public IReadOnlyCollection<Coin> ChangeCoins { get; init; } = Array.Empty<Coin>();
    }
}
