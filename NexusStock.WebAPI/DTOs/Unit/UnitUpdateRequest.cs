using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Unit
{
    public class UnitUpdateRequest
    {
        [Required(ErrorMessage = "ID обязательно")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название единицы измерения обязательно")]
        [StringLength(50, ErrorMessage = "Максимум 50 символов")]
        public string Name { get; set; }
    }
}
