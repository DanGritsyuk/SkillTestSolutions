using CompanyDirectory.BLL.Logic.Contracts;
using CompanyDirectory.Common.Entities;
using CompanyDirectory.DAL.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace CompanyDirectory.BLL.Logic
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly ILogger<DepartmentService> _logger;

        private const int MAX_DEPARTMENT_NAME_LENGHT = 100;

        public DepartmentService(
            IDepartmentRepository repository,
            ILogger<DepartmentService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            _logger.LogInformation("Getting all departments");
            return await _repository.GetAllAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            _logger.LogInformation("Getting department by ID: {DepartmentId}", id);
            return await _repository.GetByIdAsync(id);
        }

        public async Task<int> CreateDepartmentAsync(Department department)
        {
            try
            {
                ValidateDepartment(department);

                _logger.LogInformation("Creating new department: {DepartmentName}", department.Name);
                return await _repository.CreateAsync(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                throw;
            }
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            try
            {
                ValidateDepartment(department);

                _logger.LogInformation("Updating department ID {DepartmentId}", department.Id);
                return await _repository.UpdateAsync(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                throw;
            }
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting department ID {DepartmentId}", id);
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                throw;
            }
        }

        private void ValidateDepartment(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.Name))
            {
                throw new ArgumentException("Department name cannot be empty");
            }

            if (department.Name.Length > MAX_DEPARTMENT_NAME_LENGHT)
            {
                throw new ArgumentException($"Department name cannot exceed {MAX_DEPARTMENT_NAME_LENGHT} characters");
            }
        }
    }
}
