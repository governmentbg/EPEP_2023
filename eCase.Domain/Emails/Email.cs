using System;
using System.Data.Entity.ModelConfiguration;

using Newtonsoft.Json.Linq;

using eCase.Domain.Core;

namespace eCase.Domain.Emails
{
    public partial class Email : IAggregateRoot
    {
        public long EmailId { get; set; }
        public string Recipient { get; set; }
        public string MailTemplateName { get; set; }
        public string Context { get; set; }
        public EmailStatus Status { get; set; }
        public int FailedAttempts { get; set; }
        public string FailedAttemptsErrors { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }

        private Email()
        {
        }

        public Email(string recipient, string mailTemplateName, JObject context = null)
        {
            this.Recipient = recipient;
            this.MailTemplateName = mailTemplateName;
            if (context != null)
            {
                this.Context = context.ToString();
            }

            this.Status = EmailStatus.Pending;
            this.FailedAttempts = 0;
        }
    }

    public class EmailMap : EntityTypeConfiguration<Email>
    {
        public EmailMap()
        {
            // Primary Key
            this.HasKey(t => t.EmailId);

            // Properties
            this.Property(t => t.Recipient)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.MailTemplateName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Emails");
            this.Property(t => t.EmailId).HasColumnName("EmailId");
            this.Property(t => t.Recipient).HasColumnName("Recipient");
            this.Property(t => t.MailTemplateName).HasColumnName("MailTemplateName");
            this.Property(t => t.Context).HasColumnName("Context");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.FailedAttempts).HasColumnName("FailedAttempts");
            this.Property(t => t.FailedAttemptsErrors).HasColumnName("FailedAttemptsErrors");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");
        }
    }
}
