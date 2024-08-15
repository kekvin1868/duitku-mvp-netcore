namespace DuitkuMvpNetApp.ControllerModels
{
    public class CallbackModel
    {
        public string merchantCode { get; set; }
        public int amount { get; set; }
        public string merchantOrderId { get; set; }
        public string paymentCode { get; set; }
        public string resultCode { get; set; }
        public string merchantUserId { get; set; }
        public string reference { get; set; }
        public string signature { get; set; }
    }
}