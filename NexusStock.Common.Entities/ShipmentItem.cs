namespace NexusStock.Common.Entities
{
    public class ShipmentItem
    {
        public int Id { get; set; }
        public int ShipmentDocumentId { get; set; }
        public int ResourceId { get; set; }
        public int UnitId { get; set; }
        public decimal Quantity { get; set; }

        public ShipmentDocument ShipmentDocument { get; set; }
        public Resource Resource { get; set; }
        public Unit Unit { get; set; }
    }
}
