using CompanyDirectory.BLL.Logic.Contracts;
using CompanyDirectory.Common.Entities;
using CompanyDirectory.DAL.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace CompanyDirectory.BLL.Logic
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<EmployeeService> _logger;

        private const int RETIREMENT_AGE = 70;
        private const decimal HIGTH_SALARY_THRESHOLD = 10000m;
        private const decimal MINIMUM_SALARY = 15000m;

        public EmployeeService(
            IEmployeeRepository repository,
            ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<Employee>> GetHighSalaryEmployeesAsync()
        {
            var employees = await _repository.GetAllAsync();
            return employees.Where(e => e.Salary > HIGTH_SALARY_THRESHOLD);
        }

        public async Task<int> DeleteRetiredEmployeesAsync()
        {
            var employees = await _repository.GetAllAsync();
            var retired = employees.Where(e => e.GetAge() > RETIREMENT_AGE).ToList();

            foreach (var employee in retired)
            {
                await _repository.DeleteAsync(employee.Id);
            }

            _logger.LogInformation("Deleted {Count} retired employees", retired.Count);
            return retired.Count;
        }

        public async Task<int> RaiseLowSalariesAsync()
        {
            var employees = await _repository.GetAllAsync();
            var underpaid = employees.Where(e => e.Salary < MINIMUM_SALARY).ToList();

            foreach (var employee in underpaid)
            {
                employee.Salary = MINIMUM_SALARY;
                await _repository.UpdateAsync(employee);
            }

            _logger.LogInformation("Raised salaries for {Count} employees", underpaid.Count);
            return underpaid.Count;
        }
    }
}
