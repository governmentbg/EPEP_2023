using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    [Table("MoneyObligations")]
    public class MoneyObligation : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        public Guid Gid { get; set; }
        public int AttachmentType { get; set; }
        public long ParentId { get; set; }

        public long? UserRegistrationId { get; set; }

        public long MoneyObligationTypeId { get; set; }

        public string ParentDescription { get; set; }
        public string Description { get; set; }
        public string LegalBase { get; set; }
        public string ClientCode { get; set; }

        public long MoneyCurrencyId { get; set; }
        [Precision(18,2)]
        public decimal MoneyAmount { get; set; }

        public DateTime? PaymentDate { get; set; }
        public DateTime? FailDate { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }

        [ForeignKey(nameof(UserRegistrationId))]
        public virtual UserRegistration UserRegistration { get; set; }

        [ForeignKey(nameof(MoneyObligationTypeId))]
        public virtual MoneyObligationType ObligationType { get; set; }

        [ForeignKey(nameof(MoneyCurrencyId))]
        public virtual MoneyCurrency Currency { get; set; }
    }

    public class MoneyObligationConfiguration : IEntityTypeConfiguration<MoneyObligation>
    {
        public void Configure(EntityTypeBuilder<MoneyObligation> builder)
        {
            // Primary Key
            // builder.HasKey(t => t.AttachedDocumentId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            //// Table & Column Mappings
            //builder.ToTable("AttachedDocuments");
            //builder.Property(t => t.AttachedDocumentId).HasColumnName("AttachedDocumentId");
            //builder.Property(t => t.Gid).HasColumnName("Gid");
            //builder.Property(t => t.AttachmentType).HasColumnName("AttachmentType");
            //builder.Property(t => t.ParentId).HasColumnName("ParentId");
            //builder.Property(t => t.FileTitle).HasColumnName("FileTitle");
            //builder.Property(t => t.FileName).HasColumnName("FileName");
            //builder.Property(t => t.BlobKey).HasColumnName("BlobKey");
            //builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            //builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            //builder.Property(t => t.Version).HasColumnName("Version");

            //// Relationships
            //builder.HasOne(t => t.AttachedBlob)
            //    .WithMany()
            //    .HasForeignKey(d => d.BlobKey)
            //    .IsRequired();
        }
    }
}
