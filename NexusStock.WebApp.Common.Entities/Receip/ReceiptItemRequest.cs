using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebApp.Common.Entities.Receipt
{
    public class ReceiptItemRequest
    {
        public int ResourceId { get; set; }
        public int UnitId { get; set; }
        public decimal Quantity { get; set; }
    }
}
