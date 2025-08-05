namespace NexusStock.Common.Entities
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<StockBalance> Balances { get; set; } = new HashSet<StockBalance>();
        public ICollection<ReceiptItem> ReceiptItems { get; set; } = new HashSet<ReceiptItem>();
        public ICollection<ShipmentItem> ShipmentItems { get; set; } = new HashSet<ShipmentItem>();
    }
}
