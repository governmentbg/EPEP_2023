using eCase.Domain.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCase.Domain.Entities.Upgrade
{
    [Table("UserOrganizationCases")]
    public class UserOrganizationCase : IGidRoot
    {
        [Key]
        public long UserOrganizationCaseId { get; set; }
        public Guid Gid { get; set; }
        public long OrganizationUserId { get; set; }
        public long UserRegistrationId { get; set; }
        public long CaseId { get; set; }
        public bool IsActive { get; set; }
        public bool NotificateUser { get; set; }

        public long UserWrtId { get; set; }
        public DateTime DateWrt { get; set; }

        [ForeignKey(nameof(OrganizationUserId))]
        public virtual UserRegistration OrganizationUser { get; set; }
        [ForeignKey(nameof(UserRegistrationId))]
        public virtual UserRegistration UserRegistration { get; set; }
        [ForeignKey(nameof(UserWrtId))]
        public virtual UserRegistration UserWrt { get; set; }
        [ForeignKey(nameof(CaseId))]
        public virtual Case Case { get; set; }
    }
}
