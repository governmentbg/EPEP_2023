using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

using eCase.Domain.Core;

namespace eCase.Domain.Entities
{
    public partial class Appeal : IAggregateRoot
    {
        public Appeal()
        {
            this.Summons = new List<Summon>();
        }

        public long AppealId { get; set; }
        public Guid Gid { get; set; }
        public long ActId { get; set; }
        public long AppealKindId { get; set; }
        public long SideId { get; set; }
        public DateTime DateFiled { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Act Act { get; set; }
        public virtual AppealKind AppealKind { get; set; }
        public virtual Side Side { get; set; }
        public virtual ICollection<Summon> Summons { get; set; }
    }

    public class AppealMap : EntityTypeConfiguration<Appeal>
    {
        public AppealMap()
        {
            // Primary Key
            this.HasKey(t => t.AppealId);

            // Properties
            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Appeals");
            this.Property(t => t.AppealId).HasColumnName("AppealId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.ActId).HasColumnName("ActId");
            this.Property(t => t.AppealKindId).HasColumnName("AppealKindId");
            this.Property(t => t.SideId).HasColumnName("SideId");
            this.Property(t => t.DateFiled).HasColumnName("DateFiled");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasRequired(t => t.Act)
                .WithMany(t => t.Appeals)
                .HasForeignKey(d => d.ActId);
            this.HasRequired(t => t.AppealKind)
                .WithMany(t => t.Appeals)
                .HasForeignKey(d => d.AppealKindId);
            this.HasRequired(t => t.Side)
                .WithMany(t => t.Appeals)
                .HasForeignKey(d => d.SideId);
        }
    }
}
