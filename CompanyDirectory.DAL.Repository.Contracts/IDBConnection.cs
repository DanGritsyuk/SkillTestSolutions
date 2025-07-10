using System.Data;

namespace CompanyDirectory.DAL.Repository.Contracts
{
    public interface IDBConnection
    {
        Task<IDbConnection> CreateConnectionAsync(CancellationToken ct = default);
    }
}
