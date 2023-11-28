namespace Epep.Core.ViewModels.Payment
{
    public class ManageObligationResultVM
    {
        public bool Result { get; set; }

        public bool PaymentChanged { get; set; }
        public string Message { get; set; }

        public string CardFormUrl { get; set; }
    }
}
