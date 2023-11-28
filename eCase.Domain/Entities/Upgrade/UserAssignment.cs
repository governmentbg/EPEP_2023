using eCase.Domain.Core;
using eCase.Domain.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities.Upgrade
{
    public partial class UserAssignment : IAggregateRoot, IGidRoot, IEventEmitter
    {
        public UserAssignment()
        {
            ((IEventEmitter)this).Events = new List<IDomainEvent>();
        }
        public long UserAssignmentId { get; set; }
        public Guid Gid { get; set; }
        public Guid? LegacyGid { get; set; }
        public long UserRegistrationId { get; set; }
        public long CaseId { get; set; }
        public long SideId { get; set; }
        public int? AssignmentRoleId { get; set; }
        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        [ForeignKey(nameof(UserRegistrationId))]
        public virtual UserRegistration UserRegistration { get; set; }
        [ForeignKey(nameof(CaseId))]
        public virtual Case Case { get; set; }
        [ForeignKey(nameof(SideId))]
        public virtual Side Side { get; set; }
        [ForeignKey(nameof(AssignmentRoleId))]
        public virtual UserAssignmentRole AssignmentRole { get; set; }

        ICollection<IDomainEvent> IEventEmitter.Events { get; set; }

        public void GetCaseAccess(string email, string caseAbbr, string courtName)
        {
            ((IEventEmitter)this).Events.Add(new CaseAccessEvent()
            {
                Email = email,
                CaseAbbr = caseAbbr,
                CourtName = courtName
            });
        }

        public void ChangeCaseAccess(string email, string caseAbbr, string courtName)
        {
            ((IEventEmitter)this).Events.Add(new ChangeCaseAccessEvent()
            {
                Email = email,
                CaseAbbr = caseAbbr,
                CourtName = courtName
            });
        }
    }

    public class UserAssignmentMap : EntityTypeConfiguration<UserAssignment>
    {
        public UserAssignmentMap()
        {
            // Primary Key
            this.HasKey(t => t.UserAssignmentId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();
        }
    }
}
