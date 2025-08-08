namespace NexusStock.WebApp.Common.Entities.Shipment
{
    public class ShipmentFilterRequest
    {
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public List<int> ClientIds { get; set; }
        public List<int> ResourceIds { get; set; }
        public List<int> UnitIds { get; set; }
    }
}
