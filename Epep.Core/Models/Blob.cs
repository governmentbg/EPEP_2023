using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class Blob
    {
        public Blob()
        {
            this.PrivateActs = new List<Act>();
            this.PublicActs = new List<Act>();
            this.PrivateMotives = new List<Act>();
            this.PublicMotives = new List<Act>();
            this.PrivateHearings = new List<Hearing>();
            this.PublicHearings = new List<Hearing>();
            this.IncomingDocuments = new List<IncomingDocument>();
            this.OutgoingDocuments = new List<OutgoingDocument>();
            this.ScannedFiles = new List<ScannedFile>();
            this.Summons = new List<Summon>();
            this.Reports = new List<Summon>();
        }

        public Guid Key { get; set; }
        public string FileName { get; set; }
        public long BlobContentLocationId { get; set; }
        public virtual ICollection<Act> PrivateActs { get; set; }
        public virtual ICollection<Act> PublicActs { get; set; }
        public virtual ICollection<Act> PrivateMotives { get; set; }
        public virtual ICollection<Act> PublicMotives { get; set; }
        public virtual BlobContentLocation BlobContentLocation { get; set; }
        public virtual ICollection<Hearing> PrivateHearings { get; set; }
        public virtual ICollection<Hearing> PublicHearings { get; set; }
        public virtual ICollection<IncomingDocument> IncomingDocuments { get; set; }
        public virtual ICollection<OutgoingDocument> OutgoingDocuments { get; set; }
        public virtual ICollection<ScannedFile> ScannedFiles { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }
        public virtual ICollection<Summon> Reports { get; set; }
        public virtual ICollection<Summon> ReportsTimeRead { get; set; }

    }

    public class BlobConfiguration : IEntityTypeConfiguration<Blob>
    {
        public void Configure(EntityTypeBuilder<Blob> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Key);

            // Properties
            builder.Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Blobs");
            builder.Property(t => t.Key).HasColumnName("Key");
            builder.Property(t => t.FileName).HasColumnName("FileName");
            builder.Property(t => t.BlobContentLocationId).HasColumnName("BlobContentLocationId");

            // Relationships
            builder.HasOne(t => t.BlobContentLocation)
                .WithMany(t => t.Blobs)
                .HasForeignKey(d => d.BlobContentLocationId)
                .IsRequired();
        }
    }
}
