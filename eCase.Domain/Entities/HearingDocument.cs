using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class HearingDocument : IAggregateRoot
    {
        public long HearingDocumentId { get; set; }
        public Guid Gid { get; set; }
        public long HearingId { get; set; }
        public long SideId { get; set; }
        public string HearingDocumentKind { get; set; }
     
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Hearing Hearing { get; set; }
        public virtual Side Side { get; set; }
    }

    public class HearingDocumentMap : EntityTypeConfiguration<HearingDocument>
    {
        public HearingDocumentMap()
        {
            // Primary Key
            this.HasKey(t => t.HearingDocumentId);

            // Properties
            this.Property(t => t.HearingDocumentKind)
                .IsRequired()
                .HasMaxLength(200);
         

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("HearingDocuments");
            this.Property(t => t.HearingDocumentId).HasColumnName("HearingDocumentId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.HearingId).HasColumnName("HearingId");
            this.Property(t => t.SideId).HasColumnName("SideId");
            this.Property(t => t.HearingDocumentKind).HasColumnName("HearingDocumentKind");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Hearing)
                .WithMany(t => t.HearingDocuments)
                .HasForeignKey(d => d.HearingId);

            this.HasRequired(t => t.Side)
                .WithMany(t => t.HearingDocuments)
                .HasForeignKey(d => d.SideId);
        }
    }
}
