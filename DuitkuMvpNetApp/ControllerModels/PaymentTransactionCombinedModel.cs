namespace DuitkuMvpNetApp.ControllerModels
{
    public class PaymentTransactionCombinedModel
    {
        public PaymentControllerModel PaymentCombinedModel { get; set; }
        public TransactionControllerModel TransactionCombinedModel { get; set; }
        public string TransactionId { get; set; }
    }
}