using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Receipt
{
    public class ReceiptUpdateRequest
    {
        [Required(ErrorMessage = "ID обязательно")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Номер документа обязателен")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Дата документа обязательна")]
        public DateTime Date { get; set; }

        public List<ReceiptItemRequest> Items { get; set; } = new();
    }
}
