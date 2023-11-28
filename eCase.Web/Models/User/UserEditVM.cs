using eCase.Common.Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eCase.Web.Models.User
{
    public class UserEditVM
    {
        // Display
        public Guid Gid { get; set; }

        public string Username { get; set; }

        public long UserGroupId { get; set; }

        public bool HasCourt
        {
            get
            {
                return this.UserGroupId.Equals(eCase.Domain.Entities.UserGroup.CourtAdmin);
            }
        }

        public IEnumerable<SelectListItem> Courts { get; set; }

        // Edit
        [Required(ErrorMessage = "Полето {0} е задължително.")]
        [StringLength(200, ErrorMessage = "Полето {0} не може да надвишава 200 символа.")]
        [Display(Name = "Име")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string CourtCode { get; set; }
    }
}