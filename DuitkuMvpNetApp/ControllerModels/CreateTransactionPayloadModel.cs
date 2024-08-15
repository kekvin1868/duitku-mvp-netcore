namespace DuitkuMvpNetApp.ControllerModels
{
    public class CreateTransactionPayloadModel
    {
        public string TransactionId { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}