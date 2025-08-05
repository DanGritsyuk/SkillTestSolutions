using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Unit
{
    public class UnitStatusRequest
    {
        [Required(ErrorMessage = "ID обязательно")]
        public int Id { get; set; }
    }
}
