using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.ViewModels.Regix
{
    public class EntityInfoVM
    {
        [Display(Name = "ЕИК/БУЛСТАТ")]
        public string Uic { get; set; }

        [Display(Name = "Наименование")]
        public string Label { get; set; }

        [Display(Name = "Орган, упражняващ правата на държавата")]
        public string LegalOrgan { get; set; }

        [Display(Name = "Правна форма")]
        public string LegalForm { get; set; }

        [Display(Name = "Седалище и адрес на управление")]
        public string HeadquatersAddress { get; set; }

        [Display(Name = "Предмет на дейност")]
        public string MainActivity { get; set; }

        [Display(Name = "Разпределение на капитала")]
        public string CapitalShares { get; set; }

        [Display(Name = "Размер на участието в капитала")]
        public decimal? CapitalAmmount { get; set; }

        [Display(Name = "Капитал")]
        public decimal? Capital { get; set; }

        [Display(Name = "Счетоводните стандарти, които предприятието прилага")]
        public string AccountStandarts { get; set; }

        [Display(Name = "Членове на органите на управление и контрол")]
        public string LegalOrganManagers { get; set; }


        [Display(Name = "Представляващи")]
        public string Representatives { get; set; }

        [Display(Name = "Начин на представляване")]
        public string RepresentType { get; set; }

        [Display(Name = "Управители")]
        public string Managers { get; set; }
        [Display(Name = "Съдружници")]
        public string CoOwners { get; set; }
    }
}
