using NexusStock.WebApp.Common.Entities.Client;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IClientsService
    {
        Task<IEnumerable<ClientResponse>> GetClientsAsync(bool includeArchived = false);
        Task CreateClientAsync(ClientSaveRequest request);
        Task UpdateClientAsync(int id, ClientSaveRequest request);
        Task ToggleClientStatusAsync(int id);
    }
}
