namespace DuitkuMvpNetApp.Models.Payment
{
    public class PaymentObject
    {
        public required string PaymentMethod { get; set; }
        public required string PaymentName { get; set; }
        public required string PaymentImage { get; set; }
        public int TotalFee { get; set; }
    }
}