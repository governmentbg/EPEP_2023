using Epep.Core.Convertors;
using Epep.Core.Extensions;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Payment;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Epep.Core.ViewModels.Document
{
    public class DocumentListVM
    {
        public long Id { get; set; }
        public Guid Gid { get; set; }
        [Display(Name = "Съд")]
        public string CourtName { get; set; }

        [JsonIgnore]
        public long? IncomingDocumentId { get; set; }

        [JsonIgnore]
        public long? CaseId { get; set; }

        public int DocumentKind { get; set; }
        [Display(Name = "Вид документ")]
        public string DocumentTypeName { get; set; }
        [Display(Name = "Номер документ")]
        public string NumberApply { get; set; }
        public string PricelistName { get; set; }
        public DateTime ModifyDate { get; set; }
        [Display(Name = "Дата документ")]
        public DateTime? DateApply { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime? DateCourtAccept { get; set; }
        public DocumentItemInfoVM DocInfo { get; set; }
        public DocumentItemInfoVM CaseInfo { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        public Guid? DocumentApplyBlobKey { get; set; }
        public Guid? TimeApplyBlobKey { get; set; }
        [Display(Name = "Материален интерес")]
        public decimal? BaseAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        public string Currency { get; set; }

        [Display(Name = "Статус")]
        public string StatusDescription
        {
            get
            {
                if (DateCourtAccept != null)
                {
                    return "Входиран в съд";
                }
                if (DatePaid != null)
                {
                    if (TaxAmount > 0.001M)
                    {
                        return "Платен документ, чака входиране в съд";
                    }
                    else
                    {
                        return "Подаден документ, чака входиране в съд";
                    }
                }
                if (DateApply != null && TaxAmount > 0.001M)
                {
                    return "Подписан документ, чака плащане";
                }
                if (DateApply != null)
                {
                    return "Подписан документ";
                }
                return "Нов документ";
            }
        }

        public string PaymentUrl { get; set; }

        public bool HasDocumentFiles
        {
            get
            {
                return DocumentApplyBlobKey.HasValue || TimeApplyBlobKey.HasValue;
            }
        }

        public string ButtonName
        {
            get
            {
                if (DateApply == null)
                {
                    return "Продължи";
                }
                if (DatePaid == null)
                {
                    return "Плати";
                }
                return "Преглед";
            }
        }
        public ElectronicDocumentSideListVM[] Sides { get; set; }
        public FileListVM[] Files { get; set; }
        public ObligationVM[] Obligations { get; set; }
    }

    public class DocumentItemInfoVM
    {
        public Guid? Gid { get; set; }
        public int? Number { get; set; }
        public string NumberText { get; set; }
        public DateTime? Date { get; set; }
        public int? Year { get; set; }
        public string TypeName { get; set; }
    }

    public class FilterDocumentListVM
    {
        [Display(Name = "Съд")]
        public long? CourtId { get; set; }
        [Display(Name = "Вид")]
        public long? ElectronicDocumentTypeId { get; set; }
        [Display(Name = "Регистрационен номер")]
        public string NumberApply { get; set; }
        [Display(Name = "Дата на регистриране от")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateApplyFrom { get; set; }
        [Display(Name = "Дата на регистриране до")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateApplyTo { get; set; }

        [Display(Name = "Номер дело")]
        public int? CaseNumber { get; set; }
        [Display(Name = "Година")]
        public int? CaseYear { get; set; }



        public void Sanitize()
        {
            CourtId = CourtId.EmptyToNull();
            ElectronicDocumentTypeId = ElectronicDocumentTypeId.EmptyToNull();
            NumberApply = NumberApply.EmptyToNull();

            CaseNumber = CaseNumber.EmptyToNull();
            CaseYear = CaseYear.EmptyToNull();
        }
    }

}
