using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
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

        [ForeignKey(nameof(LawyerRegistrationId))]
        public virtual User User { get; set; }
    }

    public class LawyerRegistrationConfiguration : IEntityTypeConfiguration<LawyerRegistration>
    {
        public void Configure(EntityTypeBuilder<LawyerRegistration> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LawyerRegistrationId);

            // Properties
            builder.Property(t => t.Description)
                .HasMaxLength(4000);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("LawyerRegistrations");
            builder.Property(t => t.LawyerRegistrationId).HasColumnName("LawyerRegistrationId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.LawyerId).HasColumnName("LawyerId");
            builder.Property(t => t.BirthDate).HasColumnName("BirthDate");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Lawyer)
                .WithMany(t => t.LawyerRegistrations)
                .HasForeignKey(d => d.LawyerId)
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithOne(t => t.LawyerRegistration)
                .HasForeignKey<User>(x => x.UserId);
        }
    }
}
