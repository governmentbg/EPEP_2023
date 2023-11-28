using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;
using eCase.Domain.Events;

namespace eCase.Domain.Entities
{
    public partial class PersonAssignment : IAggregateRoot, IEventEmitter
    {
        public PersonAssignment()
        {
            ((IEventEmitter)this).Events = new List<IDomainEvent>();
        }

        public long PersonAssignmentId { get; set; }
        public Guid Gid { get; set; }
        public long PersonRegistrationId { get; set; }
        public long? SideId { get; set; }
        public DateTime Date { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual PersonRegistration PersonRegistration { get; set; }
        public virtual Side Side { get; set; }

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

        public void DenyCaseAccess(string email, string caseAbbr, string courtName)
        {
            ((IEventEmitter)this).Events.Add(new ChangeCaseAccessEvent()
            {
                Email = email,
                CaseAbbr = caseAbbr,
                CourtName = courtName
            });
        }
    }

    public class PersonAssignmentMap : EntityTypeConfiguration<PersonAssignment>
    {
        public PersonAssignmentMap()
        {
            // Primary Key
            this.HasKey(t => t.PersonAssignmentId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("PersonAssignments");
            this.Property(t => t.PersonAssignmentId).HasColumnName("PersonAssignmentId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.PersonRegistrationId).HasColumnName("PersonRegistrationId");
            this.Property(t => t.SideId).HasColumnName("SideId");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.PersonRegistration)
                .WithMany(t => t.PersonAssignments)
                .HasForeignKey(d => d.PersonRegistrationId);
            this.HasOptional(t => t.Side)
                .WithMany(t => t.PersonAssignments)
                .HasForeignKey(d => d.SideId);
        }
    }
}
