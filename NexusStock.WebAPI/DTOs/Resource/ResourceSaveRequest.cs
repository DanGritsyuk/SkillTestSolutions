using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Resource
{
    public class ResourceSaveRequest
    {
        [Required(ErrorMessage = "Название ресурса обязательно")]
        [StringLength(100, ErrorMessage = "Максимум 100 символов")]
        public string Name { get; set; }
    }
}
