using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
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

    public class PersonMap : EntityTypeConfiguration<Person>
    {
        public PersonMap()
        {
            // Primary Key
            this.HasKey(t => t.SubjectId);

            this.Property(t => t.SubjectId)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            // Properties
            this.Property(t => t.Firstname)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Secondname)
                .HasMaxLength(100);

            this.Property(t => t.Lastname)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.EGN)
                .HasMaxLength(20);

            this.Property(t => t.Address)
                .HasMaxLength(1000);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Persons");
            this.Property(t => t.SubjectId).HasColumnName("SubjectId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.Firstname).HasColumnName("Firstname");
            this.Property(t => t.Secondname).HasColumnName("Secondname");
            this.Property(t => t.Lastname).HasColumnName("Lastname");
            this.Property(t => t.EGN).HasColumnName("EGN");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Subject)
                .WithOptional(t => t.Person);
        }
    }
}
