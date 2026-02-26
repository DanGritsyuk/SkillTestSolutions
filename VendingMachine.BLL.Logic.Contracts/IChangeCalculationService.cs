using VendingMachine.Common.Entities;

namespace VendingMachine.BLL.Logic.Contracts
{
    public interface IChangeCalculationService
    {
        IReadOnlyCollection<Coin> CalculateChange(decimal changeAmount, IEnumerable<Coin> availableCoins);
    }
}
