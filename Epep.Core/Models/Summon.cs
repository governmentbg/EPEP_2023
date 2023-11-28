using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class Summon : IAggregateRoot//, IEventEmitter
    {
        [Description("Съдебен акт")]
        public const string ActSummonType = "eede0aa1-d70d-47cd-b9b0-a56b07f39c28";
        [Description("Обжалване на съдебен акт")]
        public const string AppealSummonType = "1d61a695-6d1c-4646-b63e-4fe0e72cce72";
        [Description("Съдебно дело")]
        public const string CaseSummonType = "cee5a0f3-401f-41c1-86ae-4534a862a63c";
        [Description("Съдебно заседание")]
        public const string HearingSummonType = "4609e4c7-b818-42f2-8584-5c16f9320320";

        public Summon()
        {
            //UserAssignments = new HashSet<UserAssignment>();
            //((IEventEmitter)this).Events = new List<IDomainEvent>();
        }

        public long SummonId { get; set; }
        public Guid Gid { get; set; }
        public long SideId { get; set; }
        public long? CaseId { get; set; }
        public long? ActId { get; set; }
        public long? HearingId { get; set; }
        public long? AppealId { get; set; }
        public long SummonTypeId { get; set; }
        public string SummonKind { get; set; }
        public string Number { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateServed { get; set; }
        public string Addressee { get; set; }
        public string Address { get; set; }
        public string Subject { get; set; }
        public DateTime ArrivedTime { get; set; }
        public DateTime? ReadTime { get; set; }
        public DateTime? CourtReadTime { get; set; }
        public string CourtReadDescription { get; set; }
        public bool IsRead { get; set; }
        public Guid? SummonBlobKey { get; set; }
        public Guid? ReportBlobKey { get; set; }
        public Guid? ReportReadTimeBlobKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Act Act { get; set; }
        public virtual Appeal Appeal { get; set; }
        public virtual Blob SummonBlob { get; set; }
        public virtual Blob ReportReadTimeBlob { get; set; }
        public virtual Blob ReportBlob { get; set; }
        public virtual Case Case { get; set; }
        public virtual Hearing Hearing { get; set; }
        public virtual Side Side { get; set; }
        public virtual SummonType SummonType { get; set; }

        //public virtual ICollection<UserAssignment> UserAssignments { get; set; }

        //ICollection<IDomainEvent> IEventEmitter.Events { get; set; }

        //public void SendSummonNotification(string email)
        //{
        //    ((IEventEmitter)this).Events.Add(new SummonNotificationEvent()
        //    {
        //        Email = email
        //    });
        //}
    }

    public class SummonConfiguration : IEntityTypeConfiguration<Summon>
    {
        public void Configure(EntityTypeBuilder<Summon> builder)
        {
            // Primary Key
            builder.HasKey(t => t.SummonId);

            // Properties
            builder.Property(t => t.SummonKind)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Addressee)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Address)
                .HasMaxLength(500);

            builder.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.CourtReadDescription)
               .HasMaxLength(500);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Summons");
            builder.Property(t => t.SummonId).HasColumnName("SummonId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.SideId).HasColumnName("SideId");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.ActId).HasColumnName("ActId");
            builder.Property(t => t.HearingId).HasColumnName("HearingId");
            builder.Property(t => t.AppealId).HasColumnName("AppealId");
            builder.Property(t => t.SummonTypeId).HasColumnName("SummonTypeId");
            builder.Property(t => t.SummonKind).HasColumnName("SummonKind");
            builder.Property(t => t.DateCreated).HasColumnName("DateCreated");
            builder.Property(t => t.DateServed).HasColumnName("DateServed");
            builder.Property(t => t.Addressee).HasColumnName("Addressee");
            builder.Property(t => t.Address).HasColumnName("Address");
            builder.Property(t => t.Subject).HasColumnName("Subject");
            builder.Property(t => t.ArrivedTime).HasColumnName("ArrivedTime");
            builder.Property(t => t.ReadTime).HasColumnName("ReadTime");
            builder.Property(t => t.IsRead).HasColumnName("IsRead");
            builder.Property(t => t.CourtReadTime).HasColumnName("CourtReadTime");
            builder.Property(t => t.CourtReadDescription).HasColumnName("CourtReadDescription");
            builder.Property(t => t.SummonBlobKey).HasColumnName("SummonBlobKey");
            builder.Property(t => t.ReportBlobKey).HasColumnName("ReportBlobKey");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Act)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.ActId);
            builder.HasOne(t => t.Appeal)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.AppealId);
            builder.HasOne(t => t.SummonBlob)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.SummonBlobKey);
            builder.HasOne(t => t.ReportReadTimeBlob)
                .WithMany(t => t.ReportsTimeRead)
                .HasForeignKey(d => d.ReportReadTimeBlobKey);
            builder.HasOne(t => t.ReportBlob)
                .WithMany(t => t.Reports)
                .HasForeignKey(d => d.ReportBlobKey);
            builder.HasOne(t => t.Case)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.CaseId);
            builder.HasOne(t => t.Hearing)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.HearingId);
            builder.HasOne(t => t.Side)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.SideId)
                .IsRequired();
            builder.HasOne(t => t.SummonType)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.SummonTypeId)
                .IsRequired();
        }
    }
}