using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class BlobContentLocationMap : EntityTypeConfiguration<BlobContentLocation>
    {
        public BlobContentLocationMap()
        {
            // Primary Key
            this.HasKey(t => t.BlobContentLocationId);

            // Properties
            this.Property(t => t.ContentDbConnectionStringName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Hash)
                .IsRequired()
                .HasMaxLength(64);

            // Table & Column Mappings
            this.ToTable("BlobContentLocations");
            this.Property(t => t.BlobContentLocationId).HasColumnName("BlobContentLocationId");
            this.Property(t => t.BlobContentId).HasColumnName("BlobContentId");
            this.Property(t => t.ContentDbConnectionStringName).HasColumnName("ContentDbConnectionStringName");
            this.Property(t => t.Hash).HasColumnName("Hash");
            this.Property(t => t.Size).HasColumnName("Size");
        }
    }
}
