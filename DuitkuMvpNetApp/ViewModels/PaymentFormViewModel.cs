namespace DuitkuMvpNetApp.ViewModels
{
    public class PaymentFormViewModel
    {
        public string TransactionData { get; set; } = string.Empty;
        public double PaymentFee { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}