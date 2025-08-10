using NexusStock.WebApp.Common.Entities.Shipment;

namespace NexusStock.WebApp.BLL.Services.Contracts
{
    public interface IShipmentsService
    {
        Task<IEnumerable<ShipmentResponse>> GetFilteredShipmentsAsync(ShipmentFilterRequest filter);
        Task CreateShipmentAsync(ShipmentSaveRequest request);
        Task UpdateShipmentAsync(int id, ShipmentSaveRequest request);
        Task DeleteShipmentAsync(int id);
        Task SignShipmentAsync(int id);
        Task RevokeShipmentAsync(int id);
    }
}
