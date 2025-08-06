using NexusStock.Common.Entities;

namespace NexusStock.BLL.Logic.Contracts
{
    public interface IClientLogic
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
        Task<IEnumerable<Client>> GetAllActiveClientsAsync();
        Task<IEnumerable<Client>> GetAllArchiveClientsAsync();
        Task<Client> GetClientByIdAsync(int id);
        Task CreateClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task ToggleClientStatusAsync(int id);
    }
}
