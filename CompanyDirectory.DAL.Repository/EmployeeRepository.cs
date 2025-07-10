using CompanyDirectory.Common.Entities;
using Dapper;
using System.Data;
using CompanyDirectory.DAL.Repository.Contracts;

namespace CompanyDirectory.DAL.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnection _connection;

        public EmployeeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(CancellationToken ct = default)
        {
            return await _connection.QueryAsync<Employee>(
                new CommandDefinition(
                    "SELECT * FROM Employees",
                    cancellationToken: ct));
        }

        public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _connection.QueryFirstOrDefaultAsync<Employee>(
                new CommandDefinition(
                    "SELECT * FROM Employees WHERE Id = @Id",
                    parameters: new { Id = id },
                    cancellationToken: ct));
        }

        public async Task<bool> UpdateAsync(Employee entity)
        {
            var affectedRows = await _connection.ExecuteAsync(
                @"UPDATE Employees SET 
                FullName = @FullName,
                BirthDate = @BirthDate,
                HireDate = @HireDate,
                Salary = @Salary,
                DepartmentId = @DepartmentId
            WHERE Id = @Id",
                entity);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var affectedRows = await _connection.ExecuteAsync(
                "DELETE FROM Employees WHERE Id = @Id",
                new { Id = id });
            return affectedRows > 0;
        }
    }
}
