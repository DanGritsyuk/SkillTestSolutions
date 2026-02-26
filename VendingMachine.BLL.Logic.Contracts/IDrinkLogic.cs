using VendingMachine.Common.Entities;

namespace VendingMachine.BLL.Logic.Contracts
{
    public interface IDrinkLogic
    {
        IAsyncEnumerable<Drink> GetAllAsync();
        Task<Drink?> GetByIdAsync(int id);
        Task<IEnumerable<Drink>> GetAllByBrandAsync(int brandId);
        Task AddAsync(Drink drink);
        Task UpdateAsync(Drink drink);
        Task DeleteAsync(int id);
        Task BulkUpsertAsync(IEnumerable<Drink> drinks);
    }
}
