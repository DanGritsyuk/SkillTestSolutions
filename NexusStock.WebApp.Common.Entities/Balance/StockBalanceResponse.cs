namespace NexusStock.WebApp.Common.Entities.Balance
{
    public class StockBalanceResponse
    {
        public int Id { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
}
