using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
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

    public class SubjectMap : EntityTypeConfiguration<Subject>
    {
        public SubjectMap()
        {
            // Primary Key
            this.HasKey(t => t.SubjectId);

            // Properties
            this.Property(t => t.Uin)
                .HasMaxLength(20);

            this.Property(t => t.Name)
                .HasMaxLength(500);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Subjects");
            this.Property(t => t.SubjectId).HasColumnName("SubjectId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.Uin).HasColumnName("Uin");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.SubjectTypeId).HasColumnName("SubjectTypeId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");
        }
    }
}
