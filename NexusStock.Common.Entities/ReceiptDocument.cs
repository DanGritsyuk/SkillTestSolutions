namespace NexusStock.Common.Entities
{
    public class ReceiptDocument
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }

        public ICollection<ReceiptItem> Items { get; set; }
    }
}
