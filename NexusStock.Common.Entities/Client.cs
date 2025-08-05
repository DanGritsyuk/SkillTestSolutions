namespace NexusStock.Common.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ShipmentDocument> Shipments { get; set; }
    }
}
