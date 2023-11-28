using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Hearing : IAggregateRoot
    {
        public Hearing()
        {
            this.Acts = new List<Act>();
            this.CaseRulings = new List<CaseRuling>();
            this.HearingParticipants = new List<HearingParticipant>();
            this.HearingDocuments = new List<HearingDocument>();
            this.Summons = new List<Summon>();
        }

        public long HearingId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public string HearingType { get; set; }
        public string HearingResult { get; set; }
        public DateTime Date { get; set; }
        public string SecretaryName { get; set; }
        public string ProsecutorName { get; set; }
        public string CourtRoom { get; set; }
        public bool IsCanceled { get; set; }
        public Guid? PrivateBlobKey { get; set; }
        public Guid? PublicBlobKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public string VideoUrl { get; set; }
        public virtual ICollection<Act> Acts { get; set; }
        public virtual Blob PrivateBlob { get; set; }
        public virtual Blob PublicBlob { get; set; }
        public virtual ICollection<CaseRuling> CaseRulings { get; set; }
        public virtual Case Case { get; set; }
        public virtual ICollection<HearingParticipant> HearingParticipants { get; set; }
        public virtual ICollection<HearingDocument> HearingDocuments { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }
    }

    public class HearingMap : EntityTypeConfiguration<Hearing>
    {
        public HearingMap()
        {
            // Primary Key
            this.HasKey(t => t.HearingId);

            // Properties
            this.Property(t => t.HearingType)
                .HasMaxLength(200);

            this.Property(t => t.HearingResult)
                .HasMaxLength(200);

            this.Property(t => t.SecretaryName)
                .HasMaxLength(200);

            this.Property(t => t.ProsecutorName)
                .HasMaxLength(200);

            this.Property(t => t.CourtRoom)
                .HasMaxLength(100);

            this.Property(t => t.VideoUrl)
               .HasMaxLength(1000);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Hearings");
            this.Property(t => t.HearingId).HasColumnName("HearingId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.HearingType).HasColumnName("HearingType");
            this.Property(t => t.HearingResult).HasColumnName("HearingResult");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.SecretaryName).HasColumnName("SecretaryName");
            this.Property(t => t.ProsecutorName).HasColumnName("ProsecutorName");
            this.Property(t => t.CourtRoom).HasColumnName("CourtRoom");
            this.Property(t => t.IsCanceled).HasColumnName("IsCanceled");
            this.Property(t => t.PrivateBlobKey).HasColumnName("PrivateBlobKey");
            this.Property(t => t.PublicBlobKey).HasColumnName("PublicBlobKey");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");
            this.Property(t => t.VideoUrl).HasColumnName("VideoUrl");

            // Relationships
            this.HasOptional(t => t.PrivateBlob)
                .WithMany(t => t.PrivateHearings)
                .HasForeignKey(d => d.PrivateBlobKey);
            this.HasOptional(t => t.PublicBlob)
                .WithMany(t => t.PublicHearings)
                .HasForeignKey(d => d.PublicBlobKey);
            this.HasRequired(t => t.Case)
                .WithMany(t => t.Hearings)
                .HasForeignKey(d => d.CaseId);
        }
    }
}
