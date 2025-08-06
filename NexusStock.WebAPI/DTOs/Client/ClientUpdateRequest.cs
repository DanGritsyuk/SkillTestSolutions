using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Client
{
    public class ClientUpdateRequest
    {
        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, ErrorMessage = "Максимум 100 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Адрес обязателен")]
        [StringLength(200, ErrorMessage = "Максимум 200 символов")]
        public string Address { get; set; }
    }
}
