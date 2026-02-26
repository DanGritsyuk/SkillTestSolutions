namespace VendingMachine.WebAPI.Contracts.Purchase
{
    public sealed class CreateCartRequest
    {
        public int? TtlMinutes { get; set; }
    }
}
