using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Client
{
    public class ClientStatusRequest
    {
        [Required(ErrorMessage = "ID обязательно")]
        public int Id { get; set; }
    }
}
