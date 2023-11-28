using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
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

        [Display(Name = "Статус")] 
        public long? LawyerStateId { get; set; }

        [Display(Name = "Вид")]
        public long? LawyerTypeId { get; set; }
        [Display(Name = "Имена")]
        public string Name { get; set; }
        [Display(Name = "Номер")]
        public string Number { get; set; }
        [Display(Name = "Идентификатор")]
        public string Uic { get; set; }
        [Display(Name = "Колегия")]
        public string College { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual ICollection<LawyerAssignment> LawyerAssignments { get; set; }
        public virtual ICollection<LawyerRegistration> LawyerRegistrations { get; set; }
        public virtual LawyerType LawyerType { get; set; }

        public virtual LawyerState LawyerState { get; set; }
    }

    public class LawyerConfiguration : IEntityTypeConfiguration<Lawyer>
    {
        public void Configure(EntityTypeBuilder<Lawyer> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LawyerId);

            // Properties
            builder.Property(t => t.Name)
                .HasMaxLength(200);

            builder.Property(t => t.Number)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.College)
                .HasMaxLength(200);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Lawyers");
            builder.Property(t => t.LawyerId).HasColumnName("LawyerId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.LawyerTypeId).HasColumnName("LawyerTypeId");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Number).HasColumnName("Number");
            builder.Property(t => t.Uic).HasColumnName("Uic");
            builder.Property(t => t.College).HasColumnName("College");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.LawyerType)
                .WithMany(t => t.Lawyers)
                .HasForeignKey(d => d.LawyerTypeId);

            builder.HasOne(t => t.LawyerState)
                .WithMany(t => t.Lawyers)
                .HasForeignKey(d => d.LawyerStateId);
        }
    }
}
