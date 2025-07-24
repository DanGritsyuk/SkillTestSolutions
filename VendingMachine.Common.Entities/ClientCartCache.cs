using System.Runtime.Serialization;

namespace VendingMachine.Common.Entities
{
    public sealed class ClientCart
    {
        public ClientCart()
        {
            Items = new();
            ClientMoney = 0m;
        }

        [DataMember(Name = "Items")]
        public List<Drink> Items { get; set; }

        [DataMember(Name = "ClientMoney")]
        public decimal ClientMoney { get; set; }
    }
}
