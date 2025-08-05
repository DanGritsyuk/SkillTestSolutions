using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Shipment
{
    public class ShipmentSignRequest
    {
        [Required(ErrorMessage = "ID документа обязательно")]
        public int Id { get; set; }
    }
}
