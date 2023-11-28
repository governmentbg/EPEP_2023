using Epep.Core.Convertors;
using Epep.Core.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Case
{
    public class CaseVM : CaseApiVM
    {
        public long CourtId { get; set; }

        [JsonIgnore]
        public bool HasElectronicDocuments { get; set; }
        [JsonIgnore]
        public bool HasElectronicPayments { get; set; }

        [JsonIgnore]
        public int? SideLeftSubjectTypeId { get; set; }

        public int SideLeftCount { get; set; }
        [JsonIgnore]
        //public SideVM RightSide { get; set; }
        public int? SideRightSubjectTypeId { get; set; }

        public int SideRightCount { get; set; }

        public short? SystemCode { get; set; }
        public CaseVM()
        {
            ArchiveCase = false;
        }
    }

    public class CaseApiVM
    {
        public Guid Gid { get; set; }

        public string CourtName { get; set; }

        public string CaseKindName { get; set; }
        public int IncommingNumber { get; set; }
        public int RegNumber { get; set; }
        public int RegYear { get; set; }
        public DateTime FormationDate { get; set; }

        //[JsonIgnore]
        //public SideVM LeftSide { get; set; }

        public string SideLeft { get; set; }


        public string SideRight { get; set; }

        public string JudgeReporter { get; set; }
        public string DepartmentName { get; set; }
        public string PanelName { get; set; }
        public bool FocusCase { get; set; }
        public bool ArchiveCase { get; set; }

        public bool HasAccess { get; set; }

        public bool? RestrictedAccess { get; set; }

    }

    public class FilterCaseVM
    {
        public bool MyCasesOnly { get; set; }
        public Guid? CaseGid { get; set; }
        [Display(Name = "Съд")]
        public long? CourtId { get; set; }
        [Display(Name = "Вид дело")]
        public long? CaseKindId { get; set; }
        [Display(Name = "Номер дело")]
        public int? RegNumber { get; set; }
        [Display(Name = "Година")]
        public int? RegYear { get; set; }

        [Display(Name = "Номер на свързано дело")]
        public int? PrevNumber { get; set; }
        [Display(Name = "Година")]
        public int? PrevYear { get; set; }
        [Display(Name = "Входящ номер")]
        public int? DocNumber { get; set; }

        [Display(Name = "Вид акт")]
        public long? ActKindId { get; set; }
        [Display(Name = "Номер акт")]
        public int? ActNumber { get; set; }
        [Display(Name = "Година")]
        public int? ActYear { get; set; }

        [Display(Name = "Страна (ЕГН/ЛНЧ/ЕИК/БУЛСТАТ)")]
        public string SideUic { get; set; }


        [Display(Name = "Дела с мое участие по идентификатор")]
        public bool CheckMeInSides { get; set; }

        [Display(Name = "Дела без участие на юрист")]
        public bool NoOrgUserCases { get; set; }
        public bool FocusCasesOnly { get; set; }
        public bool ArchiveCasesOnly { get; set; }
        public bool LastCasesOnly { get; set; }

        public virtual void Sanitize()
        {
            CourtId = CourtId.EmptyToNull();
            CaseKindId = CaseKindId.EmptyToNull();
            RegNumber = RegNumber.EmptyToNull();
            RegYear = RegYear.EmptyToNull();
            PrevNumber = PrevNumber.EmptyToNull();
            PrevYear = PrevYear.EmptyToNull();
            DocNumber = DocNumber.EmptyToNull();
            ActKindId = ActKindId.EmptyToNull();
            ActNumber = ActNumber.EmptyToNull();
            ActYear = ActYear.EmptyToNull();
            SideUic = SideUic.EmptyToNull();
        }


    }

    public class FilterHearingVM : FilterCaseVM
    {
        [Display(Name = "Дата на заседание от")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateFrom { get; set; }
        [Display(Name = "Дата на заседание до")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateTo { get; set; }
    }

    public class FilterSummonVM : FilterCaseVM
    {
        public override void Sanitize()
        {
            base.Sanitize();

            SummonKind = SummonKind.EmptyToNull();
            Number = Number.EmptyToNull();
        }

        [Display(Name = "Вид")]
        public string SummonKind { get; set; }

        [Display(Name = "Дата на съобщението от")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateFrom { get; set; }
        [Display(Name = "Дата на съобщението до")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateTo { get; set; }
        [Display(Name = "Номер")]
        public string Number { get; set; }

        [Display(Name = "Само непрочетени")]
        public bool NotReadOnly { get; set; }
    }

    public class FilterLawyerViewVM : FilterCaseVM
    {
        [Display(Name = "Дата на достъп от")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateFrom { get; set; }
        [Display(Name = "Дата на достъп до")]
        [JsonConverter(typeof(BgDateConvertor))]
        public DateTime? DateTo { get; set; }

        public Guid LawyerGid { get; set; }
    }
}
