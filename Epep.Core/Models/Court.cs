using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.Models
{
    public partial class Court
    {
        public Court()
        {
            this.Cases = new List<Case>();
            this.IncomingDocuments = new List<IncomingDocument>();
            this.Users = new List<User>();
        }

        public long CourtId { get; set; }
        public long CourtTypeId { get; set; }
        [Display(Name="Наименование")]
        public string Name { get; set; }
        public string Code { get; set; }
        [Display(Name = "Активно съдилище")]
        public bool IsActive { get; set; }
        [Display(Name = "Интегрирано в ЕПЕП")]
        public bool IsIntegrated { get; set; }
        [Display(Name = "Разрешено електронно подаване")]
        public bool? ForElectronicDocument { get; set; }

        [Display(Name = "Разрешено електронно плащане")]
        public bool? ForElectronicPayment { get; set; }

        [Display(Name = "Интернет страница")]
        public string Url { get; set; }

        public virtual ICollection<Case> Cases { get; set; }
        public virtual CourtType CourtType { get; set; }
        public virtual ICollection<IncomingDocument> IncomingDocuments { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public class CourtConfiguration : IEntityTypeConfiguration<Court>
    {
        public void Configure(EntityTypeBuilder<Court> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CourtId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(4);

            // Table & Column Mappings
            builder.ToTable("Courts");
            builder.Property(t => t.CourtId).HasColumnName("CourtId");
            builder.Property(t => t.CourtTypeId).HasColumnName("CourtTypeId");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.Code).HasColumnName("Code");
            builder.Property(t => t.IsActive).HasColumnName("IsActive");
            builder.Property(t => t.IsIntegrated).HasColumnName("IsIntegrated");
            builder.Property(t => t.ForElectronicDocument).HasColumnName("ForElectronicDocument");
            builder.Property(t => t.ForElectronicPayment).HasColumnName("ForElectronicPayment");

            // Relationships
            builder.HasOne(t => t.CourtType)
                .WithMany(t => t.Courts)
                .HasForeignKey(d => d.CourtTypeId)
                .IsRequired();
        }
    }
}
