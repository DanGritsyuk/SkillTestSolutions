namespace NexusStock.WebApp.Common.Entities.Shipment
{
    public class ShipmentCreateRequest
    {
        public string Number { get; set; }
        public DateTimeOffset Date { get; set; }
        public int ClientId { get; set; }

        public List<ShipmentItemRequest> Items { get; set; } = new();
    }
}
