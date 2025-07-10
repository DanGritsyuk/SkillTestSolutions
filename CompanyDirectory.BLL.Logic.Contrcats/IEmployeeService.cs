using CompanyDirectory.Common.Entities;

namespace CompanyDirectory.BLL.Logic.Contracts
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<IEnumerable<Employee>> GetHighSalaryEmployeesAsync();
        Task<int> DeleteRetiredEmployeesAsync();
        Task<int> RaiseLowSalariesAsync();
    }
}
