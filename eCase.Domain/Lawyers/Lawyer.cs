using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Lawyer : IAggregateRoot
    {
        public Lawyer()
        {
            this.LawyerAssignments = new List<LawyerAssignment>();
            this.LawyerRegistrations = new List<LawyerRegistration>();
        }

        public long LawyerId { get; set; }
        public Guid Gid { get; set; }
        public long? LawyerTypeId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string College { get; set; }
        public string Uic { get; set; }
        public long? LawyerStateId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual ICollection<LawyerAssignment> LawyerAssignments { get; set; }
        public virtual ICollection<LawyerRegistration> LawyerRegistrations { get; set; }
        public virtual LawyerType LawyerType { get; set; }
    }

    public class LawyerMap : EntityTypeConfiguration<Lawyer>
    {
        public LawyerMap()
        {
            // Primary Key
            this.HasKey(t => t.LawyerId);

            // Properties
            this.Property(t => t.Name)
                .HasMaxLength(200);

            this.Property(t => t.Number)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.College)
                .HasMaxLength(200);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Lawyers");
            this.Property(t => t.LawyerId).HasColumnName("LawyerId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.LawyerTypeId).HasColumnName("LawyerTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Number).HasColumnName("Number");
            this.Property(t => t.Uic).HasColumnName("Uic");
            this.Property(t => t.LawyerStateId).HasColumnName("LawyerStateId");
            this.Property(t => t.College).HasColumnName("College");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasOptional(t => t.LawyerType)
                .WithMany(t => t.Lawyers)
                .HasForeignKey(d => d.LawyerTypeId);
        }
    }
}
