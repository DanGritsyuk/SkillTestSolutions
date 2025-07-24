using VendingMachine.Common.Entities;

namespace VendingMachine.DAL.Repository.Contracts
{
    public interface IDrinksRepository
    {
        Task<Drink> GetDrinkAsync(int id);
        Task<IEnumerable<Drink>> GetAllByBrandAsync(int brandId);
        IAsyncEnumerable<Drink> GetAllAsync();
        Task CreateDrink(Drink drink);
        Task EditDrink(Drink drink);
        Task RemoveDrink(Drink drink);
        Task InsertOrUpdateRangeAsync(IEnumerable<Drink> drinks);
    }
}
