using Epep.Core.Constants;
using Epep.Core.Data;
using Epep.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Principal;

namespace Epep.Web.Extensions
{
    public class ApplicationClaimsPrincipalFactory : UserClaimsPrincipalFactory<UserRegistration>
    {
        private readonly IRepository repo;
        public ApplicationClaimsPrincipalFactory(
           UserManager<UserRegistration> userManager,
           IOptions<IdentityOptions> options,
           IRepository _repo) : base(userManager, options)
        {
            repo = _repo;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(UserRegistration user)
        {
            ClaimsIdentity myIdentity = new ClaimsIdentity("custom");
            myIdentity.AddClaim(new Claim(ClaimTypes.Sid, user.Id.ToString()));
            if (!string.IsNullOrEmpty(user.CertNo))
            {
                myIdentity.AddClaim(new Claim(CustomClaimTypes.IdStampit.CertificateNumber, user.CertNo));
            }
            if (user.OrganizationUserId.HasValue)
            {
                myIdentity.AddClaim(new Claim(ClaimTypes.PrimarySid, user.OrganizationUserId.ToString()));
            }
            switch (user.UserTypeId)
            {
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                case NomenclatureConstants.UserTypes.OrganizationUser:
                    if (user.OrganizationUserId > 0)
                    {
                        var _organization = await repo.GetByIdAsync<UserRegistration>(user.OrganizationUserId);
                        myIdentity.AddClaim(new Claim(CustomClaimTypes.OrganizationUic, _organization.UIC));
                        myIdentity.AddClaim(new Claim(CustomClaimTypes.OrganizationName, _organization.FullName));
                    }
                    break;
                case NomenclatureConstants.UserTypes.CourtAdmin:
                    myIdentity.AddClaim(new Claim(CustomClaimTypes.Court, (user.CourtId ?? 0).ToString()));
                    break;
                default:
                    break;
            }
            myIdentity.AddClaim(new Claim(ClaimTypes.Name, user.FullName));
            switch (user.LoginUserType)
            {
                case NomenclatureConstants.UserTypes.Person:
                case NomenclatureConstants.UserTypes.Administrator:
                case NomenclatureConstants.UserTypes.Organization:
                    myIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.EGN));
                    break;
                case NomenclatureConstants.UserTypes.Registration:
                    if (!string.IsNullOrEmpty(user.UIC))
                    {
                        myIdentity.AddClaim(new Claim(CustomClaimTypes.OrganizationUic, user.UIC));
                    }
                    if (!string.IsNullOrEmpty(user.RegCertificateInfo))
                    {
                        myIdentity.AddClaim(new Claim(CustomClaimTypes.RegCertInfo, user.RegCertificateInfo));
                    }
                    myIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.EGN));
                    myIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                    break;
                case NomenclatureConstants.UserTypes.Lawyer:
                    var lawyerNumber = await repo.AllReadonly<Lawyer>().Where(x => x.Uic == user.EGN).Select(x => x.Number).FirstOrDefaultAsync();
                    myIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, lawyerNumber));
                    user.UserTypeId = NomenclatureConstants.UserTypes.Lawyer;
                    break;
            }
            myIdentity.AddClaim(new Claim(CustomClaimTypes.UserType, user.UserTypeId.ToString()));

            GenericPrincipal myPrincipal = new GenericPrincipal(myIdentity, null);

            return await Task.FromResult(new ClaimsPrincipal(myPrincipal));
        }
    }

    public static class IdentityExtensions
    {
        public static string GetClaim(this ClaimsPrincipal identity, string claimName)
        {
            if (identity == null)
            {
                return string.Empty;
            }
            return identity.Claims.Where(c => c.Type == claimName).Select(x => x.Value).FirstOrDefault();
        }
    }
}
