using CompanyDirectory.Common.Entities;
using CompanyDirectory.DAL.Repository.Contracts;
using Dapper;
using System.Data;

namespace CompanyDirectory.DAL.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDbConnection _connection;

        public DepartmentRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _connection.QueryAsync<Department>(
                "SELECT * FROM Departments");
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _connection.QueryFirstOrDefaultAsync<Department>(
                "SELECT * FROM Departments WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<int> CreateAsync(Department department)
        {
            return await _connection.QuerySingleAsync<int>(
                @"INSERT INTO Departments (Name) 
                VALUES (@Name);
                SELECT CAST(SCOPE_IDENTITY() as int)",
                department);
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            var affectedRows = await _connection.ExecuteAsync(
                @"UPDATE Departments SET 
                    Name = @Name
                WHERE Id = @Id",
                department);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var affectedRows = await _connection.ExecuteAsync(
                "DELETE FROM Departments WHERE Id = @Id",
                new { Id = id });
            return affectedRows > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _connection.ExecuteScalarAsync<bool>(
                "SELECT 1 FROM Departments WHERE Id = @Id",
                new { Id = id });
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return await _connection.ExecuteScalarAsync<bool>(
                @"SELECT CASE WHEN EXISTS (
                    SELECT 1 FROM Departments 
                    WHERE Name = @Name AND (@ExcludeId IS NULL OR Id != @ExcludeId)
                THEN 0 ELSE 1 END",
                new { Name = name, ExcludeId = excludeId });
        }

        public async Task<int> GetEmployeeCountAsync(int departmentId)
        {
            return await _connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Employees WHERE DepartmentId = @DepartmentId",
                new { DepartmentId = departmentId });
        }
    }
}
