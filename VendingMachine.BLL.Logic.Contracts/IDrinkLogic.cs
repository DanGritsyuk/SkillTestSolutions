using VendingMachine.Common.Entities;

namespace VendingMachine.BLL.Logic.Contracts
{
    public interface IDrinkLogic
    {
        IAsyncEnumerable<Drink>  GetAllDrinksAsync();
        Task<Drink?> GetDrinkByIdAsync(int id);
        Task<IEnumerable<Drink>> GetAllByBrandAsync(int brandId);
    }

}
