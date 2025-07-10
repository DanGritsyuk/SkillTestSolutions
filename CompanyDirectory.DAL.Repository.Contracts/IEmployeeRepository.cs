using CompanyDirectory.Common.Entities;

namespace CompanyDirectory.DAL.Repository.Contracts
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct = default);
        Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(Employee entity);
        Task<bool> DeleteAsync(int id);
    }
}
