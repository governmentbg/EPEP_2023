using Epep.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Case
{
    public class OrganizationUserVM
    {
        public Guid? CaseGid { get; set; }
        public Guid? OrganizationUserGid { get; set; }
        public int UserType { get; set; }
        public string UserTypeName
        {
            get
            {
                switch (UserType)
                {
                    case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                        return "представляващ";
                    default:
                        return "юрист";
                }
            }
        }
        public bool IsRepresentative
        {
            get
            {
                return UserType == NomenclatureConstants.UserTypes.OrganizationRepresentative;
            }
        }
        public string FullName { get; set; }

        [Display(Name = "Юрист")]
        public Guid UserGid { get; set; }

        [Display(Name = "Получател на известия")]
        public bool NotificateUser { get; set; }

        [Display(Name = "Активен запис")]
        public bool IsActive { get; set; }

        public bool IsComfirmed { get; set; }
        public bool IsActivated { get; set; }

        public OrganizationUserVM()
        {
            IsActive = true;
        }
    }

    public class OrganizationUserListVM
    {
        public Guid Gid { get; set; }
        public int UserType { get; set; }
        public bool IsRepresentative
        {
            get
            {
                return UserType == NomenclatureConstants.UserTypes.OrganizationRepresentative;
            }
        }

        public string FullName { get; set; }
        public string Email { get; set; }

        public bool NotificateUser { get; set; }

        public bool IsActive { get; set; }

        public bool IsComfirmed { get; set; }
        public bool IsActivated { get; set; }

    }
}
