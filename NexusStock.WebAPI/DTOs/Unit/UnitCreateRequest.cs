using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Unit
{
    public class UnitCreateRequest
    {
        [Required(ErrorMessage = "Название единицы измерения обязательно")]
        [StringLength(50, ErrorMessage = "Максимум 50 символов")]
        public string Name { get; set; }
    }
}
