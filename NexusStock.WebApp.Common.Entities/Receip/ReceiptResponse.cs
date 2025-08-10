namespace NexusStock.WebApp.Common.Entities.Receipt
{
    public class ReceiptResponse
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
        public List<ReceiptItemResponse> Items { get; set; }
    }
}
