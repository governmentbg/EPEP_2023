using Epep.Core.Constants;
using Epep.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Document
{
    public class ElectronicDocumentVM
    {
        public long Id { get; set; }
        public string ErrorMessage { get; set; }
        public Guid Gid { get; set; }
        [Display(Name = "Съд")]
        public long? CourtId { get; set; }
        [Display(Name = "Съд")]
        public string CourtName { get; set; }
        public Guid? CaseGid { get; set; }
        [Display(Name = "Дело")]
        public string CaseInfo { get; set; }
        public Guid? SideGid { get; set; }
        [Display(Name = "Страна")]
        public string SideInfo { get; set; }

        public int DocumentKind { get; set; }

        [Display(Name = "Вид документ")]
        public long? ElectronicDocumentTypeId { get; set; }

        [Display(Name = "Тарифа")]
        public long? MoneyPricelistId { get; set; }

        [Display(Name = "Материален интерес")]
        [Range(0, 9999999, ErrorMessage = "Невалидна стойност.")]
        public decimal? BaseAmount { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public string SaveMode { get; set; }
        public int StepNo { get; set; }

        public string FileError { get; set; }

        public Guid? DocumentApplyBlobKey { get; set; }

        public ElectronicDocumentSideVM[] Sides { get; set; }

        public ElectronicDocumentVM()
        {
            StepNo = 1;
        }

        public void Sanitize()
        {
            CourtId = CourtId.EmptyToNull();
            ElectronicDocumentTypeId = ElectronicDocumentTypeId.EmptyToNull();
            MoneyPricelistId = MoneyPricelistId.EmptyToNull();
            Description = Description.EmptyToNull();
        }
    }

    public class ElectronicDocumentSideVM : ElectronicDocumentSideListVM
    {
        //public Guid DocumentGid { get; set; }
        //        public Guid SideGid { get; set; }
        [Display(Name = "Роля")]
        public long SideInvolvementKindId { get; set; }
        //public string SideInvolvementKindName { get; set; }
        [Display(Name = "ЕГН/ЛНЧ/ЕИК")]
        public string Uic { get; set; }
        [Display(Name = "Вид лице")]
        public long SubjectTypeId { get; set; }
        [Display(Name = "Собствено име")]
        public string Firstname { get; set; }
        [Display(Name = "Бащино име")]
        public string Secondname { get; set; }
        [Display(Name = "Фамилия")]
        public string Lastname { get; set; }
        [Display(Name = "Наименование")]
        public string EntityName { get; set; }


        public void Sanitize()
        {
            switch (SubjectTypeId)
            {
                case NomenclatureConstants.SubjectTypes.Person:
                    Firstname = Firstname.SaveTrim();
                    Secondname = Secondname.SaveTrim();
                    Lastname = Lastname.SaveTrim();
                    FullName = $"{Firstname} {Secondname} {Lastname}";
                    break;
                default:
                    EntityName = EntityName.SaveTrim();
                    FullName = EntityName;
                    break;
            }
        }
    }

    public class ElectronicDocumentSideListVM
    {
        public Guid DocumentGid { get; set; }
        public Guid SideGid { get; set; }

        public string SideInvolvementKindName { get; set; }

        public string FullName { get; set; }
        public string UIC { get; set; }
    }


}
