using NexusStock.WebApp.Common.Entities.Unit;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IUnitsService
    {
        Task<IEnumerable<UnitResponse>> GetUnitsAsync(bool includeArchived = false);
        Task CreateUnitAsync(UnitCreateRequest request);
        Task UpdateUnitAsync(UnitUpdateRequest request);
        Task ToggleUnitStatusAsync(int id);
    }
}
