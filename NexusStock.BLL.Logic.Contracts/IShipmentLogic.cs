using NexusStock.Common.Entities;

namespace NexusStock.BLL.Logic.Contracts
{
    public interface IShipmentLogic
    {
        Task CreateShipmentAsync(ShipmentDocument shipment);
        Task UpdateShipmentAsync(ShipmentDocument shipment);
        Task DeleteShipmentAsync(int id);
        Task<ShipmentDocument> GetShipmentWithItemsAsync(int id);
        Task SignShipmentAsync(int id);
        Task RevokeShipmentAsync(int id);
        Task<IEnumerable<ShipmentDocument>> GetFilteredShipmentsAsync(
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            IEnumerable<int> clientIds,
            IEnumerable<int> resourceIds,
            IEnumerable<int> unitIds);
    }
}
