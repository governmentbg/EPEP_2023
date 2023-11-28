using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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
        public short? SystemCode { get; set; }
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

        [InverseProperty("Case")]
        public virtual ICollection<ConnectedCase> ConnectedCases { get; set; }
        public virtual ICollection<Hearing> Hearings { get; set; }
        public virtual ICollection<IncomingDocument> IncomingDocuments { get; set; }
        public virtual ICollection<OutgoingDocument> OutgoingDocuments { get; set; }
        public virtual ICollection<Reporter> Reporters { get; set; }
        public virtual ICollection<ScannedFile> ScannedFiles { get; set; }
        public virtual ICollection<Side> Sides { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }
        public virtual ICollection<UserCaseFocus> CaseFocuses { get; set; }
        public virtual ICollection<UserOrganizationCase> OrganizationCases { get; set; }

        public virtual ICollection<UserAssignment> UserAssignments { get; set; }


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

    public class CaseConfiguration : IEntityTypeConfiguration<Case>
    {
        public void Configure(EntityTypeBuilder<Case> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CaseId);

            // Properties
            builder.Property(t => t.Status)
                .HasMaxLength(200);

            builder.Property(t => t.DepartmentName)
                .HasMaxLength(200);

            builder.Property(t => t.PanelName)
                .HasMaxLength(200);

            builder.Property(t => t.LegalSubject)
                .HasMaxLength(200);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Cases");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.IncomingDocumentId).HasColumnName("IncomingDocumentId");
            builder.Property(t => t.CourtId).HasColumnName("CourtId");
            builder.Property(t => t.CaseKindId).HasColumnName("CaseKindId");
            builder.Property(t => t.CaseTypeId).HasColumnName("CaseTypeId");
            builder.Property(t => t.CaseCodeId).HasColumnName("CaseCodeId");
            builder.Property(t => t.StatisticCodeId).HasColumnName("StatisticCodeId");
            builder.Property(t => t.Number).HasColumnName("Number");
            builder.Property(t => t.CaseYear).HasColumnName("CaseYear");
            builder.Property(t => t.Status).HasColumnName("Status");
            builder.Property(t => t.FormationDate).HasColumnName("FormationDate");
            builder.Property(t => t.DepartmentName).HasColumnName("DepartmentName");
            builder.Property(t => t.PanelName).HasColumnName("PanelName");
            builder.Property(t => t.LegalSubject).HasColumnName("LegalSubject");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");
            builder.Property(t => t.RestrictedAccess).HasColumnName("RestrictedAccess"); //comment for PROD

            // Relationships
            builder.HasOne(t => t.CaseCode)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.CaseCodeId);
            builder.HasOne(t => t.CaseKind)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.CaseKindId)
                .IsRequired();
            builder.HasOne(t => t.CaseType)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.CaseTypeId)
                .IsRequired();
            builder.HasOne(t => t.Court)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.CourtId)
                .IsRequired();
            builder.HasOne(t => t.IncomingDocument)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.IncomingDocumentId);
            builder.HasOne(t => t.StatisticCode)
                .WithMany(t => t.Cases)
                .HasForeignKey(d => d.StatisticCodeId);
        }
    }
}
