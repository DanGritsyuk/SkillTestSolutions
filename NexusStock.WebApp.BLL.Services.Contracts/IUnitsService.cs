using NexusStock.WebApp.Common.Entities.Unit;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IUnitsService
    {
        Task<IEnumerable<UnitResponse>> GetUnitsAsync(bool includeArchived = false);
        Task CreateUnitAsync(UnitSaveRequest request);
        Task UpdateUnitAsync(int id, UnitSaveRequest request);
        Task ToggleUnitStatusAsync(int id);
    }
}
