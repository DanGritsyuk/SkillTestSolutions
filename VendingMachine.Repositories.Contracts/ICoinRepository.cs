using VendingMachine.Common.Entities;
using VendingMachine.Common.Entities.Enums;

namespace VendingMachine.DAL.Repository.Contracts
{
    public interface ICoinRepository
    {
        Task<Coin?> GetByIdAsync(int itemId);
        Task<Coin?> GetByDenominationAsync(CoinDenomination denomination);
        Task<IEnumerable<Coin>> GetAllAsync();
        Task AddAsync(Coin coin);
        Task UpdateAsync(Coin coin);
        Task DeleteAsync(int itemId);
    }
}
