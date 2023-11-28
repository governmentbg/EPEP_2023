using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
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

    public class AppealConfiguration : IEntityTypeConfiguration<Appeal>
    {
        public void Configure(EntityTypeBuilder<Appeal> builder)
        {
            // Primary Key
            builder.HasKey(t => t.AppealId);

            // Properties
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Appeals");
            builder.Property(t => t.AppealId).HasColumnName("AppealId");
            builder.Property(t => t.Gid).HasColumnName("Gid");
            builder.Property(t => t.ActId).HasColumnName("ActId");
            builder.Property(t => t.AppealKindId).HasColumnName("AppealKindId");
            builder.Property(t => t.SideId).HasColumnName("SideId");
            builder.Property(t => t.DateFiled).HasColumnName("DateFiled");
            builder.Property(t => t.CreateDate).HasColumnName("CreateDate");
            builder.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            builder.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            builder.HasOne(t => t.Act)
                .WithMany(t => t.Appeals)
                .HasForeignKey(d => d.ActId)
                .IsRequired();
            builder.HasOne(t => t.AppealKind)
                .WithMany(t => t.Appeals)
                .HasForeignKey(d => d.AppealKindId)
                .IsRequired();
            builder.HasOne(t => t.Side)
                .WithMany(t => t.Appeals)
                .HasForeignKey(d => d.SideId)
                .IsRequired();
        }
    }
}
