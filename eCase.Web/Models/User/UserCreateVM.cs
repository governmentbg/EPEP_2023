using eCase.Common.Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eCase.Web.Models.User
{
    public class UserCreateVM
    {
        public IEnumerable<SelectListItem> Courts { get; set; }

        // Edit
        [Required(ErrorMessage = "Полето {0} е задължително.")]
        [StringLength(200, ErrorMessage = "Полето {0} не може да надвишава 200 символа.")]
        [RegularExpression(@"^[\w\-!#$%&'*+/=?^`{|}~.""]+@([\w]+[.-]?)+[\w]\.[\w]+$", ErrorMessage = "Невалидна електронна поща.")]
        [Display(Name = "Електронна поща")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително.")]
        [StringLength(200, ErrorMessage = "Полето {0} не може да надвишава 200 символа.")]
        [Display(Name = "Име")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Полето {0} е задължително.")]
        [Display(Name = "Съд")]
        public string CourtCode { get; set; }
    }
}