using System;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class ConnectedCase : IAggregateRoot
    {
        public long CaseId { get; set; }
        public Guid Gid { get; set; }
        public long PredecessorCaseId { get; set; }
        public long? ConnectedCaseTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Case Case { get; set; }
        public virtual Case PredecessorCase { get; set; }
        public virtual ConnectedCaseType ConnectedCaseType { get; set; }
    }

    public class ConnectedCaseMap : EntityTypeConfiguration<ConnectedCase>
    {
        public ConnectedCaseMap()
        {
            // Primary Key
            this.HasKey(t => t.CaseId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("ConnectedCases");
            this.Property(t => t.CaseId).HasColumnName("CaseId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.PredecessorCaseId).HasColumnName("PredecessorCaseId");
            this.Property(t => t.ConnectedCaseTypeId).HasColumnName("ConnectedCaseTypeId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Case)
                .WithOptional(t => t.ConnectedCase);
            this.HasRequired(t => t.PredecessorCase)
                .WithMany(t => t.ConnectedCases)
                .HasForeignKey(d => d.PredecessorCaseId);
            this.HasOptional(t => t.ConnectedCaseType)
                .WithMany(t => t.ConnectedCases)
                .HasForeignKey(d => d.ConnectedCaseTypeId);
        }
    }
}
