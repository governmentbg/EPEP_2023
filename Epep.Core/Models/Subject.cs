using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class Subject : IAggregateRoot
    {
        public Subject()
        {
            this.IncomingDocuments = new List<IncomingDocument>();
            this.OutgoingDocuments = new List<OutgoingDocument>();
            this.Sides = new List<Side>();
        }

        public long SubjectId { get; set; }
        public Guid Gid { get; set; }
        public string Uin { get; set; }
        public string Name { get; set; }
        public int? SubjectTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual ICollection<IncomingDocument> IncomingDocuments { get; set; }
        public virtual ICollection<OutgoingDocument> OutgoingDocuments { get; set; }
        public virtual Person Person { get; set; }
        public virtual ICollection<Side> Sides { get; set; }
    }

    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            // Primary Key
            builder.HasKey(t => t.SubjectId);

            // Properties
            builder.Property(t => t.Uin)
                .HasMaxLength(20);

            builder.Property(t => t.Name)
                .HasMaxLength(500);

            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Subjects");
            builder.Property(t => t.SubjectId).HasColumnName("SubjectId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.Uin).HasColumnName("Uin");
            builder.Property(t => t.Name).HasColumnName("Name");
            builder.Property(t => t.SubjectTypeId).HasColumnName("SubjectTypeId");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");
        }
    }
}
