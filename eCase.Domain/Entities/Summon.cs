using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;
using eCase.Domain.Entities.Upgrade;
using eCase.Domain.Events;

namespace eCase.Domain.Entities
{
    public partial class Summon : IAggregateRoot, IEventEmitter
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
            ((IEventEmitter)this).Events = new List<IDomainEvent>();
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
        //ALTERLEGACYEPEP
        public long? AddresseeUserRegistrationId { get; set; }
        public string Address { get; set; }
        public string Subject { get; set; }
        public DateTime ArrivedTime { get; set; }
        public DateTime? ReadTime { get; set; }
        public DateTime? CourtReadTime { get; set; }
        public string CourtReadDescription { get; set; }
        public bool IsRead { get; set; }
        public Guid? SummonBlobKey { get; set; }
        public Guid? ReportBlobKey { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Act Act { get; set; }
        public virtual Appeal Appeal { get; set; }
        public virtual Blob SummonBlob { get; set; }
        public virtual Blob ReportBlob { get; set; }
        public virtual Case Case { get; set; }
        public virtual Hearing Hearing { get; set; }
        public virtual Side Side { get; set; }
        public virtual SummonType SummonType { get; set; }
        public virtual UserRegistration AddresseeUserRegistration { get; set; }


        ICollection<IDomainEvent> IEventEmitter.Events { get; set; }

        public void SendSummonNotification(string email)
        {
            ((IEventEmitter)this).Events.Add(new SummonNotificationEvent()
            {
                Email = email
            });
        }
    }

    public class SummonMap : EntityTypeConfiguration<Summon>
    {
        public SummonMap()
        {
            // Primary Key
            this.HasKey(t => t.SummonId);

            // Properties
            this.Property(t => t.SummonKind)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Addressee)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Address)
                .HasMaxLength(500);

            this.Property(t => t.Subject)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.CourtReadDescription)
               .HasMaxLength(500);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Summons");
            this.Property(t => t.SummonId).HasColumnName("SummonId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.SideId).HasColumnName("SideId");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.ActId).HasColumnName("ActId");
            this.Property(t => t.HearingId).HasColumnName("HearingId");
            this.Property(t => t.AppealId).HasColumnName("AppealId");
            this.Property(t => t.SummonTypeId).HasColumnName("SummonTypeId");
            this.Property(t => t.SummonKind).HasColumnName("SummonKind");
            this.Property(t => t.DateCreated).HasColumnName("DateCreated");
            this.Property(t => t.DateServed).HasColumnName("DateServed");
            this.Property(t => t.Addressee).HasColumnName("Addressee");
            this.Property(t => t.AddresseeUserRegistrationId).HasColumnName("AddresseeUserRegistrationId");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.Subject).HasColumnName("Subject");
            this.Property(t => t.ArrivedTime).HasColumnName("ArrivedTime");
            this.Property(t => t.ReadTime).HasColumnName("ReadTime");
            this.Property(t => t.IsRead).HasColumnName("IsRead");
            this.Property(t => t.CourtReadTime).HasColumnName("CourtReadTime");
            this.Property(t => t.CourtReadDescription).HasColumnName("CourtReadDescription");
            this.Property(t => t.SummonBlobKey).HasColumnName("SummonBlobKey");
            this.Property(t => t.ReportBlobKey).HasColumnName("ReportBlobKey");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasOptional(t => t.Act)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.ActId);
            this.HasOptional(t => t.Appeal)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.AppealId);
            this.HasOptional(t => t.SummonBlob)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.SummonBlobKey);
            this.HasOptional(t => t.ReportBlob)
                .WithMany(t => t.Reports)
                .HasForeignKey(d => d.ReportBlobKey);
            this.HasOptional(t => t.Case)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.CaseId);
            this.HasOptional(t => t.Hearing)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.HearingId);
            this.HasRequired(t => t.Side)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.SideId);
            this.HasRequired(t => t.SummonType)
                .WithMany(t => t.Summons)
                .HasForeignKey(d => d.SummonTypeId);
            this.HasOptional(t => t.AddresseeUserRegistration)
               .WithMany(t => t.Summons)
               .HasForeignKey(d => d.AddresseeUserRegistrationId);
        }
    }
}