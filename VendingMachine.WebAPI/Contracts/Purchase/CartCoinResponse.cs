namespace VendingMachine.WebAPI.Contracts.Purchase
{
    public sealed class CartCoinResponse
    {
        public int Denomination { get; set; }
        public int Quantity { get; set; }
    }
}
