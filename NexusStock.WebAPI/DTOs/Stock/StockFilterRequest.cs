namespace NexusStock.WebAPI.DTOs.Stock
{
    public class StockFilterRequest
    {
        public List<int> ResourceIds { get; set; } = new List<int>();
        public List<int> UnitIds { get; set; } = new List<int>();
    }
}
