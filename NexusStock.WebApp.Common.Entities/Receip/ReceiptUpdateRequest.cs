using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebApp.Common.Entities.Receipt
{
    public class ReceiptUpdateRequest
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTimeOffset Date { get; set; }
        public List<ReceiptItemRequest> Items { get; set; } = new();
    }
}
