using eCase.Domain.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCase.Domain.BlobStorage
{
    public partial class BlobContent
    {
        public long BlobContentId { get; set; }
        public string Hash { get; set; }
        public Nullable<long> Size { get; set; }
        public byte[] Content { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class BlobContentMap : EntityTypeConfiguration<BlobContent>
    {
        public BlobContentMap()
        {
            // Primary Key
            this.HasKey(t => t.BlobContentId);

            // Properties
            this.Property(t => t.BlobContentId)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);

            this.Property(t => t.Hash)
                .HasMaxLength(64);

            // Table & Column Mappings
            this.ToTable("BlobContents");
            this.Property(t => t.BlobContentId).HasColumnName("BlobContentId");
            this.Property(t => t.Hash).HasColumnName("Hash");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.Content).HasColumnName("Content");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}
