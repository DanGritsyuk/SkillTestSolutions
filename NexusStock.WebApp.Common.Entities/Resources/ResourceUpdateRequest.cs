using System.ComponentModel.DataAnnotations;

namespace NexusStock.WebApp.Common.Entities.Resources
{
    public class ResourceUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
