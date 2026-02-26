namespace VendingMachine.WebAPI.Contracts.Purchase
{
    public sealed class AddDrinkRequest
    {
        public int DrinkId { get; set; }
        public int Quantity { get; set; } = 1;
        public int? TtlMinutes { get; set; }
    }
}
