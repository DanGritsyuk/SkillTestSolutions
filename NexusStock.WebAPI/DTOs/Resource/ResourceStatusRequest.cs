using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Resource
{
    public class ResourceStatusRequest
    {
        [Required(ErrorMessage = "ID обязательно")]
        public int Id { get; set; }
    }
}
