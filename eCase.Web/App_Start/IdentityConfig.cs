using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace eCase.Web
{
    public static class ClaimKeys
    {
        public const string UserId = "user/user_id";
        public const string Gid = "user/gid";

        public const string Email = "user/email";
        public const string Name = "user/name";

        public const string Lawyer = "user/lawyer";
        public const string Court = "user/court";
        public const string UserGroup = "user/user_group";
    }

    public class eCaseUser : IdentityUser
    {
        public string UserId { get; set; }
        public string Gid { get; set; }

        public string Name { get; set; }

        public string CourtId { get; set; }
        public string UserGroupId { get; set; }

        public long UserID
        {
            get
            {
                return long.Parse(this.UserId);
            }
        }

        public Guid GID
        {
            get
            {
                return new Guid(this.Gid);
            }
        }

        public long UserGroupID
        {
            get
            {
                return long.Parse(this.UserGroupId);
            }
        }

        public bool IsPerson
        {
            get
            {
                return this.UserGroupID.Equals(eCase.Domain.Entities.UserGroup.Person) || this.UserGroupID.Equals(eCase.Domain.Entities.UserGroup.LawyerAndPerson);
            }
        }

        public bool IsSystemAdmin
        {
            get
            {
                return this.UserGroupID.Equals(eCase.Domain.Entities.UserGroup.SystemAdmin);
            }
        }

        public bool IsLawyer
        {
            get
            {
                return this.UserGroupID.Equals(eCase.Domain.Entities.UserGroup.Lawyer) || this.UserGroupID.Equals(eCase.Domain.Entities.UserGroup.LawyerAndPerson);
            }
        }

        public bool IsCourtAdmin
        {
            get
            {
                return this.UserGroupID.Equals(eCase.Domain.Entities.UserGroup.CourtAdmin);
            }
        }

        public bool IsSuperAdmin
        {
            get
            {
                return this.UserGroupID.Equals(eCase.Domain.Entities.UserGroup.SuperAdmin);
            }
        }

        public virtual Task<ClaimsIdentity> GenerateUserIdentityAsync(eCaseUserManager manager, string authenticationType)
        {
            var userIdentity = new ClaimsIdentity(authenticationType);

            userIdentity.AddClaim(new Claim(ClaimKeys.UserId, this.UserId ?? string.Empty));

            userIdentity.AddClaim(new Claim(ClaimKeys.Email, this.Email));
            userIdentity.AddClaim(new Claim(ClaimKeys.Name, this.Name ?? string.Empty));

            userIdentity.AddClaim(new Claim(ClaimKeys.Court, this.CourtId ?? string.Empty));
            userIdentity.AddClaim(new Claim(ClaimKeys.UserGroup, this.UserGroupId ?? string.Empty));

            return Task.FromResult(userIdentity);
        }
    }

    public class eCaseUserManager : UserManager<eCaseUser>
    {
        public eCaseUserManager(IUserStore<eCaseUser> store) : base(store) { }

        public static eCaseUserManager Create(IdentityFactoryOptions<eCaseUserManager> options, IOwinContext context)
        {
            var manager = new eCaseUserManager(new UserStore<eCaseUser>());
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<eCaseUser>(dataProtectionProvider.Create(Constants.CookieName));
            }

            return manager;
        }

        public static eCaseUser LoadUser(ClaimsIdentity identity)
        {
            return new eCaseUser()
            {
                UserId = identity.FindFirstValue(ClaimKeys.UserId),

                Email = identity.FindFirstValue(ClaimKeys.Email),
                Name = identity.FindFirstValue(ClaimKeys.Name),

                CourtId = identity.FindFirstValue(ClaimKeys.Court),
                UserGroupId = identity.FindFirstValue(ClaimKeys.UserGroup)
            };
        }
    }

    public class SignInManager : SignInManager<eCaseUser, string>
    {
        public SignInManager(eCaseUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        { }

        public static SignInManager Create(IdentityFactoryOptions<SignInManager> options, IOwinContext context)
        {
            return new SignInManager(context.GetUserManager<eCaseUserManager>(), context.Authentication);
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(eCaseUser user)
        {
            return user.GenerateUserIdentityAsync((eCaseUserManager)UserManager, Constants.ProjectName);
        }

        public override async Task SignInAsync(eCaseUser user, bool isPersistent, bool rememberBrowser)
        {
            var userIdentity = await CreateUserIdentityAsync(user);

            AuthenticationManager.SignOut();

            var authenticationProperties = new AuthenticationProperties() { IsPersistent = isPersistent };

            if (rememberBrowser)
            {
                var rememberBrowserIdentity = AuthenticationManager
                    .CreateTwoFactorRememberBrowserIdentity(ConvertIdToString(user.Email));

                AuthenticationManager.SignIn(
                    authenticationProperties,
                    userIdentity,
                    rememberBrowserIdentity);
            }
            else
            {
                AuthenticationManager.SignIn(
                    authenticationProperties,
                    userIdentity);
            }
        }
    }
}
