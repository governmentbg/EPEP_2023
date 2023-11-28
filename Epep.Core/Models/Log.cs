using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class Log
    {
        public long LogId { get; set; }
        public string Level { get; set; }
        public DateTime? LogDate { get; set; }
        public string IP { get; set; }
        public string RawUrl { get; set; }
        public string Form { get; set; }
        public string UserAgent { get; set; }
        public string SessionId { get; set; }
        public Guid? RequestId { get; set; }
        public string Message { get; set; }
    }

    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            // Primary Key
            builder.HasKey(t => t.LogId);

            // Properties
            builder.Property(t => t.Level)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.IP)
                .HasMaxLength(50);

            builder.Property(t => t.RawUrl)
                .HasMaxLength(500);

            builder.Property(t => t.UserAgent)
                .HasMaxLength(200);

            builder.Property(t => t.SessionId)
                .HasMaxLength(50);

            // Table & Column Mappings
            builder.ToTable("Logs");
            builder.Property(t => t.LogId).HasColumnName("LogId");
            builder.Property(t => t.Level).HasColumnName("Level");
            builder.Property(t => t.LogDate).HasColumnName("LogDate");
            builder.Property(t => t.IP).HasColumnName("IP");
            builder.Property(t => t.RawUrl).HasColumnName("RawUrl");
            builder.Property(t => t.Form).HasColumnName("Form");
            builder.Property(t => t.UserAgent).HasColumnName("UserAgent");
            builder.Property(t => t.SessionId).HasColumnName("SessionId");
            builder.Property(t => t.RequestId).HasColumnName("RequestId");
            builder.Property(t => t.Message).HasColumnName("Message");
        }
    }
}
