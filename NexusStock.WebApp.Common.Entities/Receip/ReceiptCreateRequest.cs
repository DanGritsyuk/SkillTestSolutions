using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebApp.Common.Entities.Receipt
{
    public class ReceiptCreateRequest
    {
        public string Number { get; set; }
        public DateTimeOffset Date { get; set; }

        public List<ReceiptItemRequest> Items { get; set; } = new();
    }
}
