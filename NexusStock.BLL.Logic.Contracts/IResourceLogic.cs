using NexusStock.Common.Entities;

namespace NexusStock.BLL.Logic.Contracts
{
    public interface IResourceLogic
    {
        Task<IEnumerable<Resource>> GetAllResourcesAsync();
        Task<IEnumerable<Resource>> GetAllActiveResourcesAsync();
        Task<Resource> GetResourceByIdAsync(int id);
        Task CreateResourceAsync(Resource resource);
        Task UpdateResourceAsync(Resource resource);
        Task ToggleResourceStatusAsync(int id);
    }
}
