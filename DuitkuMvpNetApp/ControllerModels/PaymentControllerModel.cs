using DuitkuMvpNetApp.Models.Payment;

namespace DuitkuMvpNetApp.ControllerModels
{
    public class PaymentControllerModel
    {
        public required PaymentResponse PaymentResponse { get; set; }
        public double TotalAmount { get; set; }
    }
}