namespace NexusStock.WebApp.Common.Entities.Shipment
{
    public class ShipmentItemResponse
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
    }
}
