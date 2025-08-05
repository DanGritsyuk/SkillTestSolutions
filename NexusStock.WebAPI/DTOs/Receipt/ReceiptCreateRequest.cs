using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Receipt
{
    public class ReceiptCreateRequest
    {
        [Required(ErrorMessage = "Номер документа обязателен")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Дата документа обязательна")]
        public DateTime Date { get; set; } = DateTime.Today;

        public List<ReceiptItemRequest> Items { get; set; } = new();
    }
}
