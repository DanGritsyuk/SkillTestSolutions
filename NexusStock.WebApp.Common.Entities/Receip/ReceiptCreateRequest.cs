using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebApp.Common.Entities.Receipt
{
    public class ReceiptCreateRequest
    {
        public string Number { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;

        public List<ReceiptItemRequest> Items { get; set; } = new();
    }
}
