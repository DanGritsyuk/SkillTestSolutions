using NexusStock.WebApp.Common.Entities.Resources;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IResourcesService
    {
        Task<IEnumerable<ResourceResponse>> GetResourcesAsync(bool includeArchived = false);
        Task CreateResourceAsync(ResourceCreateRequest request);
        Task UpdateResourceAsync(ResourceUpdateRequest request);
        Task ToggleResourceStatusAsync(int id);
    }
}
