using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Case : IAggregateRoot
    {
        public Case()
        {
            this.Acts = new List<Act>();
            this.Assignments = new List<Assignment>();
            this.CaseRulings = new List<CaseRuling>();
            this.ConnectedCases = new List<ConnectedCase>();
            this.Hearings = new List<Hearing>();
            this.IncomingDocuments = new List<IncomingDocument>();
            this.OutgoingDocuments = new List<OutgoingDocument>();
            this.Reporters = new List<Reporter>();
            this.ScannedFiles = new List<ScannedFile>();
            this.Sides = new List<Side>();
            this.Summons = new List<Summon>();
        }

        public long CaseId { get; set; }
        public Guid Gid { get; set; }
        public long? IncomingDocumentId { get; set; }
        public long CourtId { get; set; }
        public long CaseKindId { get; set; }
        public long CaseTypeId { get; set; }
        public long? CaseCodeId { get; set; }
        public long? StatisticCodeId { get; set; }
        public int Number { get; set; }
        public int CaseYear { get; set; }
        public string Status { get; set; }
        public DateTime FormationDate { get; set; }
        public string DepartmentName { get; set; }
        public string PanelName { get; set; }
        public string LegalSubject { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public bool? RestrictedAccess { get; set; } //comment for PROD
        public byte[] Version { get; set; }
        public virtual ICollection<Act> Acts { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual CaseCode CaseCode { get; set; }
        public virtual CaseKind CaseKind { get; set; }
        public virtual ICollection<CaseRuling> CaseRulings { get; set; }
        public virtual ConnectedCase ConnectedCase { get; set; }
        public virtual CaseType CaseType { get; set; }
        public virtual Court Court { get; set; }
        public virtual IncomingDocument IncomingDocument { get; set; }
        public virtual StatisticCode StatisticCode { get; set; }
        public virtual ICollection<ConnectedCase> ConnectedCases { get; set; }
        public virtual ICollection<Hearing> Hearings { get; set; }
        public virtual ICollection<IncomingDocument> IncomingDocuments { get; set; }
        public virtual ICollection<OutgoingDocument> OutgoingDocuments { get; set; }
        public virtual ICollection<Reporter> Reporters { get; set; }
        public virtual ICollection<ScannedFile> ScannedFiles { get; set; }
        public virtual ICollection<Side> Sides { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }

        public string SideNames
        {
            get
            {
                if (this.Sides.Count == 0)
                    return string.Empty;

                StringBuilder sb = new StringBuilder();

                foreach (var side in this.Sides.OrderBy(e => e.InsertDate).Take(2).OrderBy(e => e.Name))
                {
                    sb.AppendFormat("{0}, ", side.Name.Trim());
                }

                return sb.ToString().Substring(0, sb.Length - 2);
            }
        }
    }

    public class CaseMap : EntityTypeConfiguration<Case>
    {
        public CaseMap()
        {
            // Primary Key
            this.HasKey(t => t.CaseId);

            // Properties
            this.Property(t => t.Status)
                .HasMaxLength(200);

            this.Property(t => t.DepartmentName)
                .HasMaxLength(200);

            this.Property(t => t.PanelName)
                .HasMaxLength(200);

            this.Property(t => t.LegalSubject)
                .HasMaxLength(200);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Cases");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.IncomingDocumentId).HasColumnName("IncomingDocumentId");
            this.Property(t => t.CourtId).HasColumnName("CourtId");
            this.Property(t => t.CaseKindId).HasColumnName("CaseKindId");
            this.Property(t => t.CaseTypeId).HasColumnName("CaseTypeId");
            this.Property(t => t.CaseCodeId).HasColumnName("CaseCodeId");
            this.Property(t => t.StatisticCodeId).HasColumnName("StatisticCodeId");
            this.Property(t => t.Number).HasColumnName("Number");
            this.Property(t => t.CaseYear).HasColumnName("CaseYear");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.FormationDate).HasColumnName("FormationDate");
            this.Property(t => t.DepartmentName).HasColumnName("DepartmentName");
            this.Property(t => t.PanelName).HasColumnName("PanelName");
            this.Property(t => t.LegalSubject).HasColumnName("LegalSubject");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");
             this.Property(t => t.RestrictedAccess).HasColumnName("RestrictedAccess"); //comment for PROD

            // Relationships
            this.HasOptional(t => t.CaseCode)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.CaseCodeId);
            this.HasRequired(t => t.CaseKind)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.CaseKindId);
            this.HasRequired(t => t.CaseType)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.CaseTypeId);
            this.HasRequired(t => t.Court)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.CourtId);
            this.HasOptional(t => t.IncomingDocument)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.IncomingDocumentId);
            this.HasOptional(t => t.StatisticCode)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.StatisticCodeId);
        }
    }
}
