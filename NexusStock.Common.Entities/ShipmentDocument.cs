namespace NexusStock.Common.Entities
{
    public class ShipmentDocument
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public bool IsSigned { get; set; }

        public Client Client { get; set; }
        public ICollection<ShipmentItem> Items { get; set; }
    }
}
