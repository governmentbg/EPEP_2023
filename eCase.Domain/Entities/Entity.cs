using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Entity : IAggregateRoot
    {
        public long SubjectId { get; set; }
        public Guid Gid { get; set; }
        public string Name { get; set; }
        public string Bulstat { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Subject Subject { get; set; }
    }

    public class EntityMap : EntityTypeConfiguration<Entity>
    {
        public EntityMap()
        {
            // Primary Key
            this.HasKey(t => t.SubjectId);

            this.Property(t => t.SubjectId)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.Bulstat)
                .HasMaxLength(20);

            this.Property(t => t.Address)
                .HasMaxLength(500);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Entities");
            this.Property(t => t.SubjectId).HasColumnName("SubjectId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Bulstat).HasColumnName("Bulstat");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Subject)
                .WithOptional(t => t.Entity);
        }
    }
}
