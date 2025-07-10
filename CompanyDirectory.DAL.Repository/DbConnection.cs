using CompanyDirectory.DAL.Repository.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CompanyDirectory.DAL.Repository
{
    public class DbConnection : IDBConnection, IDisposable
    {
        private readonly string _connectionString;
        private readonly ILogger<DbConnection> _logger;

        public DbConnection(string connectionString, ILogger<DbConnection> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken ct = default)
        {
            var connection = new SqlConnection(_connectionString);

            try
            {
                await connection.OpenAsync(ct);
                _logger.LogDebug($"Created async SQL connection [{connection.ClientConnectionId}]");
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create async SQL connection");
                await connection.DisposeAsync();
                throw;
            }
        }

        public void Dispose() => _logger.LogInformation("Connection factory disposed");
    }
}
