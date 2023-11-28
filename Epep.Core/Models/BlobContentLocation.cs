using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class BlobContentLocation
    {
        public BlobContentLocation()
        {
            this.Blobs = new List<Blob>();
        }

        public long BlobContentLocationId { get; set; }
        public long BlobContentId { get; set; }
        public string ContentDbConnectionStringName { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
        public virtual ICollection<Blob> Blobs { get; set; }
    }

    public class BlobContentLocationConfiguration : IEntityTypeConfiguration<BlobContentLocation>
    {
        public void Configure(EntityTypeBuilder<BlobContentLocation> builder)
        {
            // Primary Key
            builder.HasKey(t => t.BlobContentLocationId);

            // Properties
            builder.Property(t => t.ContentDbConnectionStringName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Hash)
                .IsRequired()
                .HasMaxLength(64);

            // Table & Column Mappings
            builder.ToTable("BlobContentLocations");
            builder.Property(t => t.BlobContentLocationId).HasColumnName("BlobContentLocationId");
            builder.Property(t => t.BlobContentId).HasColumnName("BlobContentId");
            builder.Property(t => t.ContentDbConnectionStringName).HasColumnName("ContentDbConnectionStringName");
            builder.Property(t => t.Hash).HasColumnName("Hash");
            builder.Property(t => t.Size).HasColumnName("Size");
        }
    }
}
