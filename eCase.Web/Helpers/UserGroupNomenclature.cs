using eCase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eCase.Web.Helpers
{
    public class UserGroupNomenclature
    {
        public long Id {get;set;}
        public string Description {get;set;}
        
        public static UserGroupNomenclature SuperAdmin = new UserGroupNomenclature(){Id = UserGroup.SuperAdmin, Description = "Супер администратор"};
        public static UserGroupNomenclature CourtAdmin = new UserGroupNomenclature() { Id = UserGroup.CourtAdmin, Description = "Администратор на съд" };
        public static UserGroupNomenclature Lawyer = new UserGroupNomenclature() { Id = UserGroup.Lawyer, Description = "Адвокат" };
        public static UserGroupNomenclature SystemAdmin = new UserGroupNomenclature() { Id = UserGroup.SystemAdmin, Description = "Системен администратор" };
        public static UserGroupNomenclature Person = new UserGroupNomenclature() { Id = UserGroup.Person, Description = "Физическо лице" };

        public static List<SelectListItem> GetValues()
        {
            return new List<UserGroupNomenclature>()
            {
                SuperAdmin,
                CourtAdmin,
                Lawyer,
                SystemAdmin,
                Person
            }.Select(e => new SelectListItem() {Value = e.Id.ToString(), Text = e.Description }).ToList();
        }
    }
}