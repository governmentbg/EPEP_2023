using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class LawyerRegistration : IAggregateRoot
    {
        public long LawyerRegistrationId { get; set; }
        public Guid Gid { get; set; }
        public long LawyerId { get; set; }
        public DateTime BirthDate { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Lawyer Lawyer { get; set; }
 //       public long? UserId { get; set; } // TODO check why is this here?????
        public virtual User User { get; set; }
    }

    public class LawyerRegistrationMap : EntityTypeConfiguration<LawyerRegistration>
    {
        public LawyerRegistrationMap()
        {
            // Primary Key
            this.HasKey(t => t.LawyerRegistrationId);

            // Properties
            this.Property(t => t.Description)
                .HasMaxLength(4000);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("LawyerRegistrations");
            this.Property(t => t.LawyerRegistrationId).HasColumnName("LawyerRegistrationId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.LawyerId).HasColumnName("LawyerId");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Lawyer)
                .WithMany(t => t.LawyerRegistrations)
                .HasForeignKey(d => d.LawyerId);
            this.HasRequired(t => t.User)
                .WithOptional(t => t.LawyerRegistration);
        }
    }
}
