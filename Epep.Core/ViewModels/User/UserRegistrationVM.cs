using Epep.Core.Constants;
using Epep.Core.Extensions;
using Epep.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.User
{
    public class UserRegistrationVM
    {
        public Guid? Gid { get; set; }

        [Display(Name = "Вид лице")]
        public int UserType { get; set; }

        [Display(Name = "Съд")]
        public long? CourtId { get; set; }

        [Display(Name = "Съд")]
        public string CourtName { get; set; }

        [Display(Name = "Вашето име")]
        [RegularExpression(NomenclatureConstants.CyliricLetersAndNumbers, ErrorMessage = "Невалидна стойност.")]
        public string FullName { get; set; }

        [Display(Name = "Наименование на дружество или организация")]
        [RegularExpression(NomenclatureConstants.CyliricLetersAndNumbers, ErrorMessage = "Невалидна стойност.")]
        public string OrgFullName { get; set; }

        [Display(Name = "Електронна поща")]
        [RegularExpression(NomenclatureConstants.EmailRegexPattern, ErrorMessage = "Невалидна стойност.")]
        public string Email { get; set; }

        [Display(Name = "Електронна поща за известяване")]
        [RegularExpression(NomenclatureConstants.EmailRegexPattern, ErrorMessage = "Невалидна стойност.")]
        public string NotificationEmail { get; set; }

        public string OrgFileError { get; set; }

        [Display(Name = "ЕГН/ЛНЧ")]
        public string EGN { get; set; }

        //[Display(Name = "Адвокатски номер")]
        //public string LawyerNumber { get; set; }

        [Display(Name = "ЕИК/БУЛСТАТ")]
        public string UIC { get; set; }


        [Display(Name = "Имена на представляващия юридическото лице")]
        [RegularExpression(NomenclatureConstants.CyliricLetersAndNumbers, ErrorMessage = "Невалидна стойност.")]
        public string RepresentativeFullName { get; set; }

        [Display(Name = "Електронна поща")]
        [RegularExpression(NomenclatureConstants.EmailRegexPattern, ErrorMessage = "Невалидна стойност.")]
        public string RepresentativeEmail { get; set; }

        [Display(Name = "ЕГН")]
        public string RepresentativeEGN { get; set; }

        [Display(Name = "Електронна поща за известяване")]
        [RegularExpression(NomenclatureConstants.EmailRegexPattern, ErrorMessage = "Невалидна стойност.")]
        public string RepresentativeNotificationEmail { get; set; }

        [Display(Name = "Активен потребител")]
        public bool IsActive { get; set; }

        [Display(Name = "Активен профил")]
        public bool IsComfirmed { get; set; }

        public string reCaptchaToken { get; set; }

        public string UserTypeName { get; set; }

        public long? ExistingOrganizationId { get; set; }

        public string RegCertificateInfo { get; set; }


        public void Sanitize()
        {
            if (CourtId <= 0)
            {
                CourtId = null;
            }
            EGN = EGN.EmptyToNull();
            UIC = UIC.EmptyToNull();
            Email = Email.EmptyToNull();
            NotificationEmail = NotificationEmail.EmptyToNull();
            FullName = FullName.EmptyToNull();
            OrgFullName = OrgFullName.EmptyToNull();
            RepresentativeFullName = RepresentativeFullName.EmptyToNull();
            RepresentativeEGN = RepresentativeEGN.EmptyToNull();
            RepresentativeEmail = RepresentativeEmail.EmptyToNull();
        }

        public UserRegistration MapToEntity()
        {
            var result = new UserRegistration()
            {
                Gid = this.Gid ??  Guid.NewGuid(),
                UserTypeId = this.UserType,
                UserName = Guid.NewGuid().ToString().ToLower().Replace("-", ""),
                FullName = (string.IsNullOrEmpty(this.OrgFullName)) ? this.FullName : this.OrgFullName,
                EGN = this.EGN,
                IsComfirmedEGN = false,
                UIC = this.UIC,
                IsComfirmedUIC = false,
                Email = this.Email,
                NotificationEmail = this.NotificationEmail,
                EmailConfirmed = false,
                CourtId = (UserType == NomenclatureConstants.UserTypes.CourtAdmin) ? this.CourtId : (long?)null,
                IsActive = true,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };

            return result;
        }
        public UserRegistration MapRepresentativeToEntity(long organizationUserId)
        {
            var result = new UserRegistration()
            {
                Gid = Guid.NewGuid(),
                UserTypeId = NomenclatureConstants.UserTypes.OrganizationRepresentative,
                UserName = Guid.NewGuid().ToString().ToLower().Replace("-", ""),
                FullName = this.RepresentativeFullName,
                EGN = this.RepresentativeEGN,
                IsComfirmedEGN = false,
                UIC = this.UIC,
                OrganizationUserId = organizationUserId,
                IsComfirmedUIC = false,
                Email = this.RepresentativeEmail,
                NotificationEmail = this.RepresentativeNotificationEmail,
                EmailConfirmed = false,
                IsActive = true,
                RegCertificateInfo = this.RegCertificateInfo,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };
            return result;
        }
    }
}
