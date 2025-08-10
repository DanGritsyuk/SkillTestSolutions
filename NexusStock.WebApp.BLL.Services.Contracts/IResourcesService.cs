using NexusStock.WebApp.Common.Entities.Resources;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IResourcesService
    {
        Task<IEnumerable<ResourceResponse>> GetResourcesAsync(bool includeArchived = false);
        Task CreateResourceAsync(ResourceSaveRequest request);
        Task UpdateResourceAsync(int id, ResourceSaveRequest request);
        Task ToggleResourceStatusAsync(int id);
    }
}
