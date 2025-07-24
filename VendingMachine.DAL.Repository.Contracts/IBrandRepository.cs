using VendingMachine.Common.Entities;

namespace VendingMachine.DAL.Repository.Contracts
{
    public interface IBrandRepository
    {
        Task<Brand?> GetByIdAsync(int id);
        Task<IEnumerable<Brand>> GetAllAsync();
        Task AddAsync(Brand brand);
        Task UpdateAsync(Brand brand);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}
