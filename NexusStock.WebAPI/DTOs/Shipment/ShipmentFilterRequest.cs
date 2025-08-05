namespace NexusStock.WebAPI.DTOs.Shipment
{
    public class ShipmentFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> ClientIds { get; set; } = new();
        public List<int> ResourceIds { get; set; } = new();
        public List<int> UnitIds { get; set; } = new();
    }
}
