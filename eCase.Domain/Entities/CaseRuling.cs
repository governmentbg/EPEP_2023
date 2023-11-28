using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class CaseRuling : IAggregateRoot
    {
        public long CaseRulingId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public long? HearingId { get; set; }
        public long? ActId { get; set; }
        public long CaseRulingKindId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Act Act { get; set; }
        public virtual CaseRulingKind CaseRulingKind { get; set; }
        public virtual Case Case { get; set; }
        public virtual Hearing Hearing { get; set; }
    }

    public class CaseRulingMap : EntityTypeConfiguration<CaseRuling>
    {
        public CaseRulingMap()
        {
            // Primary Key
            this.HasKey(t => t.CaseRulingId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("CaseRulings");
            this.Property(t => t.CaseRulingId).HasColumnName("CaseRulingId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.HearingId).HasColumnName("HearingId");
            this.Property(t => t.ActId).HasColumnName("ActId");
            this.Property(t => t.CaseRulingKindId).HasColumnName("CaseRulingKindId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasOptional(t => t.Act)
                .WithMany(t => t.CaseRulings)
                .HasForeignKey(d => d.ActId);
            this.HasRequired(t => t.CaseRulingKind)
                .WithMany(t => t.CaseRulings)
                .HasForeignKey(d => d.CaseRulingKindId);
            this.HasRequired(t => t.Case)
                .WithMany(t => t.CaseRulings)
                .HasForeignKey(d => d.CaseId);
            this.HasOptional(t => t.Hearing)
                .WithMany(t => t.CaseRulings)
                .HasForeignKey(d => d.HearingId);
        }
    }
}
