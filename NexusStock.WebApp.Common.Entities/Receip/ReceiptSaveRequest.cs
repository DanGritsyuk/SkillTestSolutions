namespace NexusStock.WebApp.Common.Entities.Receipt
{
    public class ReceiptSaveRequest
    {
        public string Number { get; set; }
        public DateTimeOffset Date { get; set; }

        public List<ReceiptItemRequest> Items { get; set; } = new();
    }
}
