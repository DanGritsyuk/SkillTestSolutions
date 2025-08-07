namespace NexusStock.WebApp.Common.Entities.Shipment
{
    public class ShipmentFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> ClientIds { get; set; }
        public List<int> ResourceIds { get; set; }
        public List<int> UnitIds { get; set; }
    }
}
