using Newtonsoft.Json;

namespace DuitkuMvpNetApp.Models.Payment
{
    public class PaymentResponse
    {
        [JsonProperty("paymentFee")]
        public required List<PaymentObject> PaymentFee { get; set; }
    }
}