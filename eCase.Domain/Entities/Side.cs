using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;
using eCase.Domain.Service.Entities;

namespace eCase.Domain.Entities
{
    public partial class Side : IAggregateRoot
    {
        public Side()
        {
            this.Appeals = new List<Appeal>();
            this.LawyerAssignments = new List<LawyerAssignment>();
            this.PersonAssignments = new List<PersonAssignment>();
            this.HearingDocuments = new List<HearingDocument>();
            this.Summons = new List<Summon>();
        }

        public long SideId { get; set; }
        public Guid Gid { get; set; }
        public long CaseId { get; set; }
        public long SideInvolvementKindId { get; set; }
        public long? SubjectId { get; set; }
        public DateTime InsertDate { get; set; }
        public string ProceduralRelation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual ICollection<Appeal> Appeals { get; set; }
        public virtual Case Case { get; set; }
        public virtual ICollection<LawyerAssignment> LawyerAssignments { get; set; }
        public virtual ICollection<PersonAssignment> PersonAssignments { get; set; }

        public virtual ICollection<HearingDocument> HearingDocuments { get; set; }
        public virtual SideInvolvementKind SideInvolvementKind { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }

        public string Name
        {
            get
            {
                if (this.Subject != null)
                {
                    return this.Subject.Name;
                }

                return string.Empty;
            }
        }
    }

    public class SideMap : EntityTypeConfiguration<Side>
    {
        public SideMap()
        {
            // Primary Key
            this.HasKey(t => t.SideId);

            // Properties
            this.Property(t => t.ProceduralRelation)
                .HasMaxLength(200);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Sides");
            this.Property(t => t.SideId).HasColumnName("SideId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.SideInvolvementKindId).HasColumnName("SideInvolvementKindId");
            this.Property(t => t.SubjectId).HasColumnName("SubjectId");
            this.Property(t => t.InsertDate).HasColumnName("InsertDate");
            this.Property(t => t.ProceduralRelation).HasColumnName("ProceduralRelation");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Case)
                .WithMany(t => t.Sides)
                .HasForeignKey(d => d.CaseId);
            this.HasRequired(t => t.SideInvolvementKind)
                .WithMany(t => t.Sides)
                .HasForeignKey(d => d.SideInvolvementKindId);
            this.HasOptional(t => t.Subject)
                .WithMany(t => t.Sides)
                .HasForeignKey(d => d.SubjectId);
        }
    }
}
