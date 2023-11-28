using eCase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace eCase.Web.Helpers
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // Custom property
        public long AllowedRoleId { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            ClaimsIdentity ci = httpContext.User.Identity as ClaimsIdentity;
            var currentUser = eCaseUserManager.LoadUser(ci);

            if (currentUser.IsPerson && AllowedRoleId.Equals(UserGroup.Person))
                return true;

            if (currentUser.IsSystemAdmin && AllowedRoleId.Equals(UserGroup.SystemAdmin))
                return true;

            if (currentUser.IsLawyer && AllowedRoleId.Equals(UserGroup.Lawyer))
                return true;

            if (currentUser.IsCourtAdmin && AllowedRoleId.Equals(UserGroup.CourtAdmin))
                return true;

            if (currentUser.IsSuperAdmin && AllowedRoleId.Equals(UserGroup.SuperAdmin))
                return true;

            return false;
        }
    }
}