using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class Person : IAggregateRoot
    {
        public long SubjectId { get; set; }
        public Guid Gid { get; set; }
        public string Firstname { get; set; }
        public string Secondname { get; set; }
        public string Lastname { get; set; }
        public string EGN { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Subject Subject { get; set; }
    }

    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // Primary Key
            builder.HasKey(t => t.SubjectId);

            builder.Property(t => t.SubjectId);
                //.HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            // Properties
            builder.Property(t => t.Firstname)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Secondname)
                .HasMaxLength(100);

            builder.Property(t => t.Lastname)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.EGN)
                .HasMaxLength(20);

            builder.Property(t => t.Address)
                .HasMaxLength(1000);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Persons");
            builder.Property(t => t.SubjectId).HasColumnName("SubjectId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.Firstname).HasColumnName("Firstname");
            builder.Property(t => t.Secondname).HasColumnName("Secondname");
            builder.Property(t => t.Lastname).HasColumnName("Lastname");
            builder.Property(t => t.EGN).HasColumnName("EGN");
            builder.Property(t => t.Address).HasColumnName("Address");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Subject)
                .WithOne(t => t.Person);
        }
    }
}
