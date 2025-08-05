namespace NexusStock.WebAPI.DTOs.Receipt
{
    public class ReceiptResponse
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public List<ReceiptItemResponse> Items { get; set; } = new();

        public decimal TotalQuantity => Items.Sum(i => i.Quantity);
    }
}
