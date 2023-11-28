namespace Epep.Core.ViewModels.Payment
{
    public class PaymentVM
    {
        public long Id { get; set; }
        public Guid Gid { get; set; }
        public string PaymentCode { get; set; }

        public string ParentDescription { get; set; }
        public string Description { get; set; }
        public string LegalBase { get; set; }

        public string MoneyCurrency { get; set; }
        public decimal MoneyAmount { get; set; }

        public bool IsPaid { get; set; }
        public bool IsFailed { get; set; }
    }
}
