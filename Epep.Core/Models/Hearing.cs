using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class HearingConfiguration : IEntityTypeConfiguration<Hearing>
    {
        public void Configure(EntityTypeBuilder<Hearing> builder)
        {
            // Primary Key
            builder.HasKey(t => t.HearingId);

            // Properties
            builder.Property(t => t.HearingType)
                .HasMaxLength(200);

            builder.Property(t => t.HearingResult)
                .HasMaxLength(200);

            builder.Property(t => t.SecretaryName)
                .HasMaxLength(200);

            builder.Property(t => t.ProsecutorName)
                .HasMaxLength(200);

            builder.Property(t => t.CourtRoom)
                .HasMaxLength(100);

            builder.Property(t => t.VideoUrl)
               .HasMaxLength(1000);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Hearings");
            builder.Property(t => t.HearingId).HasColumnName("HearingId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.HearingType).HasColumnName("HearingType");
            builder.Property(t => t.HearingResult).HasColumnName("HearingResult");
            builder.Property(t => t.Date).HasColumnName("Date");
            builder.Property(t => t.SecretaryName).HasColumnName("SecretaryName");
            builder.Property(t => t.ProsecutorName).HasColumnName("ProsecutorName");
            builder.Property(t => t.CourtRoom).HasColumnName("CourtRoom");
            builder.Property(t => t.IsCanceled).HasColumnName("IsCanceled");
            builder.Property(t => t.PrivateBlobKey).HasColumnName("PrivateBlobKey");
            builder.Property(t => t.PublicBlobKey).HasColumnName("PublicBlobKey");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");
            builder.Property(t => t.VideoUrl).HasColumnName("VideoUrl");

            // Relationships
            builder.HasOne(t => t.PrivateBlob)
                .WithMany(t => t.PrivateHearings)
                .HasForeignKey(d => d.PrivateBlobKey);
            builder.HasOne(t => t.PublicBlob)
                .WithMany(t => t.PublicHearings)
                .HasForeignKey(d => d.PublicBlobKey);
            builder.HasOne(t => t.Case)
                .WithMany(t => t.Hearings)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
        }
    }
}
