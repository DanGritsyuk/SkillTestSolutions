using NexusStock.WebApp.Common.Entities.Client;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IClientsService
    {
        Task<IEnumerable<ClientResponse>> GetClientsAsync(bool includeArchived = false);
        Task CreateClientAsync(ClientCreateRequest request);
        Task UpdateClientAsync(ClientUpdateRequest request);
        Task ToggleClientStatusAsync(int id);
    }
}
