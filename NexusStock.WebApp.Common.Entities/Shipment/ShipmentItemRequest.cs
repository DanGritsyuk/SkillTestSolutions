using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebApp.Common.Entities.Shipment
{
    public class ShipmentItemRequest
    {
        [Required(ErrorMessage = "Выберите ресурс")]
        public int ResourceId { get; set; }

        [Required(ErrorMessage = "Выберите единицу измерения")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "Введите количество")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public decimal Quantity { get; set; }
    }
}
