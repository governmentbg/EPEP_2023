using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eCase.Web.Models.Lawyer
{
    public class LawyerEditVM
    {
        // Display
        public Guid Gid { get; set; }

        //Edit
        [Required(ErrorMessage = "Полето {0} е задължително.")]
        [StringLength(50, ErrorMessage = "Полето {0} не може да надвишава 50 символа.")]
        [Display(Name = "Номер")]
        public string Number { get; set; }

        [StringLength(200, ErrorMessage = "Полето {0} не може да надвишава 200 символа.")]
        [Display(Name = "Име")]
        public string Name { get; set; }

        public long? LawyerTypeId { get; set; }

        [StringLength(200, ErrorMessage = "Полето {0} не може да надвишава 200 символа.")]
        [Display(Name = "Колегия")]
        public string College { get; set; }

        public IEnumerable<SelectListItem> LawyerTypes { get; set; }
    }
}