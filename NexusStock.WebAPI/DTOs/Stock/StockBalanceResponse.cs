namespace NexusStock.WebAPI.DTOs.Stock
{
    public class StockBalanceResponse
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
    }
}
