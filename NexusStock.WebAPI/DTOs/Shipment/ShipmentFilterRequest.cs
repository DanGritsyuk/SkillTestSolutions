namespace NexusStock.WebAPI.DTOs.Shipment
{
    public class ShipmentFilterRequest
    {
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public List<int> ClientIds { get; set; } = new();
        public List<int> ResourceIds { get; set; } = new();
        public List<int> UnitIds { get; set; } = new();
        public bool? IsSigned {  get; set; }
    }
}
