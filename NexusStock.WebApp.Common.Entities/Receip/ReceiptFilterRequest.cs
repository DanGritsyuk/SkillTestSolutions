namespace NexusStock.WebApp.Common.Entities.Receipt
{
    public class ReceiptFilterRequest
    {
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public List<int> DocumentIds { get; set; } = new();
        public List<int> ResourceIds { get; set; } = new();
        public List<int> UnitIds { get; set; } = new();
    }
}
