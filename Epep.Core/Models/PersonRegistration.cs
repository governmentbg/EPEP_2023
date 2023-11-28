using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
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
        [ForeignKey(nameof(PersonRegistrationId))]
        public virtual User User { get; set; }
    }

    public class PersonRegistrationConfiguration : IEntityTypeConfiguration<PersonRegistration>
    {
        public void Configure(EntityTypeBuilder<PersonRegistration> builder)
        {
            // Primary Key
            builder.HasKey(t => t.PersonRegistrationId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.EGN)
                .HasMaxLength(20);

            builder.Property(t => t.Address)
                .HasMaxLength(1000);

            builder.Property(t => t.Description)
                .HasMaxLength(4000);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("PersonRegistrations");
            builder.Property(t => t.PersonRegistrationId).HasColumnName("PersonRegistrationId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Email).HasColumnName("Email");
            builder.Property(t => t.EGN).HasColumnName("EGN");
            builder.Property(t => t.BirthDate).HasColumnName("BirthDate");
            builder.Property(t => t.Address).HasColumnName("Address");
            builder.Property(t => t.Description).HasColumnName("Description");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.User)
                .WithOne(t => t.PersonRegistration)
                .HasForeignKey<User>(t => t.UserId);
        }
    }
}
