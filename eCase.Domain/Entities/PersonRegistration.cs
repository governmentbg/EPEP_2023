using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class PersonRegistration : IAggregateRoot
    {
        public PersonRegistration()
        {
            this.PersonAssignments = new List<PersonAssignment>();
        }

        public long PersonRegistrationId { get; set; }
        public Guid Gid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string EGN { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual ICollection<PersonAssignment> PersonAssignments { get; set; }
        public virtual User User { get; set; }
    }

    public class PersonRegistrationMap : EntityTypeConfiguration<PersonRegistration>
    {
        public PersonRegistrationMap()
        {
            // Primary Key
            this.HasKey(t => t.PersonRegistrationId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.EGN)
                .HasMaxLength(20);

            this.Property(t => t.Address)
                .HasMaxLength(1000);

            this.Property(t => t.Description)
                .HasMaxLength(4000);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("PersonRegistrations");
            this.Property(t => t.PersonRegistrationId).HasColumnName("PersonRegistrationId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.EGN).HasColumnName("EGN");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.User)
                .WithOptional(t => t.PersonRegistration);
        }
    }
}
