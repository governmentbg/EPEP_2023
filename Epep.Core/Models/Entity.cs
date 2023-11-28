using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class EntityConfiguration : IEntityTypeConfiguration<Entity>
    {
        public void Configure(EntityTypeBuilder<Entity> builder)
        {
            // Primary Key
            builder.HasKey(t => t.SubjectId);

            builder.Property(t => t.SubjectId);
                //.HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.Bulstat)
                .HasMaxLength(20);

            builder.Property(t => t.Address)
                .HasMaxLength(500);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Entities");
            builder.Property(t => t.SubjectId).HasColumnName("SubjectId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Bulstat).HasColumnName("Bulstat");
            builder.Property(t => t.Address).HasColumnName("Address");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Subject)
                .WithOne(t => t.Entity);
        }
    }
}
