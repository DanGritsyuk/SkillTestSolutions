using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Resource
{
    public class ResourceUpdateRequest
    {
        [Required(ErrorMessage = "ID обязательно")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название ресурса обязательно")]
        [StringLength(100, ErrorMessage = "Максимум 100 символов")]
        public string Name { get; set; }
    }
}
