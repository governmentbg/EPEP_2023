using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Act : IAggregateRoot
    {
        public Act()
        {
            this.ActPreparators = new List<ActPreparator>();
            this.Appeals = new List<Appeal>();
            this.CaseRulings = new List<CaseRuling>();
            this.Summons = new List<Summon>();
        }

        public long ActId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public long ActKindId { get; set; }
        public long? HearingId { get; set; }
        public int? Number { get; set; }
        public DateTime DateSigned { get; set; }
        public DateTime? DateInPower { get; set; }
        public DateTime? MotiveDate { get; set; }
        // public bool? Finishing { get; set; } //comment for PROD
        // public bool? CanBeSubjectToAppeal { get; set; } //comment for PROD
        public Guid? PrivateActBlobKey { get; set; }
        public Guid? PublicActBlobKey { get; set; }
        public Guid? PrivateMotiveBlobKey { get; set; }
        public Guid? PublicMotiveBlobKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual ActKind ActKind { get; set; }
        public virtual ICollection<ActPreparator> ActPreparators { get; set; }
        public virtual Case Case { get; set; }
        public virtual Hearing Hearing { get; set; }
        public virtual Blob PrivateActBlob { get; set; }
        public virtual Blob PublicActBlob { get; set; }
        public virtual Blob PrivateMotiveBlob { get; set; }
        public virtual Blob PublicMotiveBlob { get; set; }
        public virtual ICollection<Appeal> Appeals { get; set; }
        public virtual ICollection<CaseRuling> CaseRulings { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }
    }

    public class ActMap : EntityTypeConfiguration<Act>
    {
        public ActMap()
        {
            // Primary Key
            this.HasKey(t => t.ActId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Acts");
            this.Property(t => t.ActId).HasColumnName("ActId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.ActKindId).HasColumnName("ActKindId");
            this.Property(t => t.HearingId).HasColumnName("HearingId");
            this.Property(t => t.Number).HasColumnName("Number");
            this.Property(t => t.DateSigned).HasColumnName("DateSigned");
            this.Property(t => t.DateInPower).HasColumnName("DateInPower");
            this.Property(t => t.MotiveDate).HasColumnName("MotiveDate");
            this.Property(t => t.PrivateActBlobKey).HasColumnName("PrivateActBlobKey");
            this.Property(t => t.PublicActBlobKey).HasColumnName("PublicActBlobKey");
            this.Property(t => t.PrivateMotiveBlobKey).HasColumnName("PrivateMotiveBlobKey");
            this.Property(t => t.PublicMotiveBlobKey).HasColumnName("PublicMotiveBlobKey");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.ActKind)
                .WithMany(t => t.Acts)
                .HasForeignKey(d => d.ActKindId);
            this.HasRequired(t => t.Case)
                .WithMany(t => t.Acts)
                .HasForeignKey(d => d.CaseId);
            this.HasOptional(t => t.Hearing)
                .WithMany(t => t.Acts)
                .HasForeignKey(d => d.HearingId);
            this.HasOptional(t => t.PrivateActBlob)
                .WithMany(t => t.PrivateActs)
                .HasForeignKey(d => d.PrivateActBlobKey);
            this.HasOptional(t => t.PublicActBlob)
                .WithMany(t => t.PublicActs)
                .HasForeignKey(d => d.PrivateMotiveBlobKey);
            this.HasOptional(t => t.PrivateMotiveBlob)
                .WithMany(t => t.PrivateMotives)
                .HasForeignKey(d => d.PublicActBlobKey);
            this.HasOptional(t => t.PublicMotiveBlob)
                .WithMany(t => t.PublicMotives)
                .HasForeignKey(d => d.PublicMotiveBlobKey);
        }
    }
}
