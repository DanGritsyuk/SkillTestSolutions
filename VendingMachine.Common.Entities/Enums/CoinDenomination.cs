using System.ComponentModel;

namespace VendingMachine.Common.Entities.Enums
{
    public enum CoinDenomination
    {
        [Description(description: "1 ruble")]
        One = 1,

        [Description(description: "2 ruble")]
        Two = 2,

        [Description(description: "5 ruble")]
        Five = 5,

        [Description(description: "10 ruble")]
        Ten = 10
    }
}
