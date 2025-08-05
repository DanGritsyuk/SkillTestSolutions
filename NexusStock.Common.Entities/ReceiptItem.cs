namespace NexusStock.Common.Entities
{
    public class ReceiptItem
    {
        public int Id { get; set; }
        public int ReceiptDocumentId { get; set; }
        public int ResourceId { get; set; }
        public int UnitId { get; set; }
        public decimal Quantity { get; set; }

        public ReceiptDocument ReceiptDocument { get; set; }
        public Resource Resource { get; set; }
        public Unit Unit { get; set; }
    }
}
