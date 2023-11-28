using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Epep.Core.Models
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

    public class ActConfiguration : IEntityTypeConfiguration<Act>
    {
        public void Configure(EntityTypeBuilder<Act> builder)
        {

            // Primary Key
            builder.HasKey(t => t.ActId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Acts");
            builder.Property(t => t.ActId).HasColumnName("ActId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.ActKindId).HasColumnName("ActKindId");
            builder.Property(t => t.HearingId).HasColumnName("HearingId");
            builder.Property(t => t.Number).HasColumnName("Number");
            builder.Property(t => t.DateSigned).HasColumnName("DateSigned");
            builder.Property(t => t.DateInPower).HasColumnName("DateInPower");
            builder.Property(t => t.MotiveDate).HasColumnName("MotiveDate");
            builder.Property(t => t.PrivateActBlobKey).HasColumnName("PrivateActBlobKey");
            builder.Property(t => t.PublicActBlobKey).HasColumnName("PublicActBlobKey");
            builder.Property(t => t.PrivateMotiveBlobKey).HasColumnName("PrivateMotiveBlobKey");
            builder.Property(t => t.PublicMotiveBlobKey).HasColumnName("PublicMotiveBlobKey");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.ActKind)
                .WithMany(t => t.Acts)
                .HasForeignKey(d => d.ActKindId)
                .IsRequired();
            builder.HasOne(t => t.Case)
                .WithMany(t => t.Acts)
                .HasForeignKey(d => d.CaseId)
                .IsRequired();
            builder.HasOne(t => t.Hearing)
                .WithMany(t => t.Acts)
                .HasForeignKey(d => d.HearingId);
            builder.HasOne(t => t.PrivateActBlob)
                .WithMany(t => t.PrivateActs)
                .HasForeignKey(d => d.PrivateActBlobKey);
            builder.HasOne(t => t.PublicActBlob)
                .WithMany(t => t.PublicActs)
                .HasForeignKey(d => d.PublicActBlobKey);
            builder.HasOne(t => t.PrivateMotiveBlob)
                .WithMany(t => t.PrivateMotives)
                .HasForeignKey(d => d.PrivateMotiveBlobKey);
            builder.HasOne(t => t.PublicMotiveBlob)
                .WithMany(t => t.PublicMotives)
                .HasForeignKey(d => d.PublicMotiveBlobKey);
        }
    }
}
