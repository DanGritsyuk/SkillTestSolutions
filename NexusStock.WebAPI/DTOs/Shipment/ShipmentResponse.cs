namespace NexusStock.WebAPI.DTOs.Shipment
{
    public class ShipmentResponse
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime Date { get; set; }
        public bool IsSigned { get; set; }
        public List<ShipmentItemResponse> Items { get; set; } = new();

        public decimal TotalQuantity => Items.Sum(i => i.Quantity);
    }
}
