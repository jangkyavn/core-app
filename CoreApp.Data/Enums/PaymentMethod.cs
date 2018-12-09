using System.ComponentModel;

namespace CoreApp.Data.Enums
{
    public enum PaymentMethod
    {
        [Description("Thanh toán khi giao hàng")]
        CashOnDelivery,
        [Description("Ngân hàng trực tuyến")]
        OnlineBanking,
        [Description("Cổng thanh toán")]
        PaymentGateway,
        [Description("Thẻ Visa")]
        Visa,
        [Description("Thẻ Master")]
        MasterCard,
        [Description("PayPal")]
        PayPal,
        [Description("ATM")]
        Atm
    }
}
