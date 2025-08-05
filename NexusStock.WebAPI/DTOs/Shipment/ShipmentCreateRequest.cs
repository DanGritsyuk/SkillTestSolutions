using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Shipment
{
    public class ShipmentCreateRequest
    {
        [Required(ErrorMessage = "Номер документа обязателен")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Дата документа обязательна")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Клиент обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите клиента")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Должен быть хотя бы один товар")]
        [MinLength(1, ErrorMessage = "Добавьте хотя бы один товар")]
        public List<ShipmentItemRequest> Items { get; set; } = new();
    }
}
