using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebAPI.DTOs.Unit
{
    public class UnitResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
