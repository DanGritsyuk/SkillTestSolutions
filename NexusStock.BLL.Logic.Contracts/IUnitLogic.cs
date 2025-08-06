using NexusStock.Common.Entities;

namespace NexusStock.BLL.Logic.Contracts
{
    public interface IUnitLogic
    {
        Task<IEnumerable<Unit>> GetAllUnitsAsync();
        Task<IEnumerable<Unit>> GetAllActiveUnitsAsync(); 
        Task<IEnumerable<Unit>> GetAllArchiveUnitsAsync();
        Task<Unit> GetUnitByIdAsync(int id);
        Task CreateUnitAsync(Unit unit);
        Task UpdateUnitAsync(Unit unit);
        Task ToggleUnitStatusAsync(int id);
    }
}
