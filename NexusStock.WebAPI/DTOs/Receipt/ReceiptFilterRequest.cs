namespace NexusStock.WebAPI.DTOs.Receipt
{
    public class ReceiptFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> DocumentIds { get; set; } = new();
        public List<int> ResourceIds { get; set; } = new();
        public List<int> UnitIds { get; set; } = new();
    }
}
