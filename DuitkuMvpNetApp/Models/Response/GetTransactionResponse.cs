namespace DuitkuMvpNetApp.Models.Response
{
    public class GetTransactionResponse
    {
        public string MerchantCode { get; set; }
        public string Reference { get; set; }
        public string PaymentUrl { get; set; }
        public string VaNumber { get; set; }
        public string Amount { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }
}