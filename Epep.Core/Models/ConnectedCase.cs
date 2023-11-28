using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
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
        [ForeignKey(nameof(CaseId))]
        public virtual Case Case { get; set; }
        [ForeignKey(nameof(PredecessorCaseId))]
        public virtual Case PredecessorCase { get; set; }
        public virtual ConnectedCaseType ConnectedCaseType { get; set; }
    }

    public class ConnectedCaseConfiguration : IEntityTypeConfiguration<ConnectedCase>
    {
        public void Configure(EntityTypeBuilder<ConnectedCase> builder)
        {
            // Primary Key
            builder.HasKey(t => t.CaseId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("ConnectedCases");
            builder.Property(t => t.CaseId).HasColumnName("CaseId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.PredecessorCaseId).HasColumnName("PredecessorCaseId");
            builder.Property(t => t.ConnectedCaseTypeId).HasColumnName("ConnectedCaseTypeId");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            //builder.HasOne(t => t.Case)
            //    .WithOne(t => t.ConnectedCase);

            //builder.HasOne(t => t.PredecessorCase)
            //    .WithMany(t => t.PredecessorCases)
            //    .HasForeignKey(d => d.PredecessorCaseId)
            //    .IsRequired();
            builder.HasOne(t => t.ConnectedCaseType)
                .WithMany(t => t.ConnectedCases)
                .HasForeignKey(d => d.ConnectedCaseTypeId);
        }
    }
}
