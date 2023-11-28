using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.Models
{
    public class Email
    {
        [Key]
        public long EmailId { get; set; }
        public string Recipient { get; set; }
        public string MailTemplateName { get; set; }
        public string Context { get; set; }
        public int Status { get; set; }
        public int FailedAttempts { get; set; }
        public string FailedAttemptsErrors { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }

        public void IncrementFailedAttempts(string exception)
        {
            JObject fae;
            if (String.IsNullOrEmpty(this.FailedAttemptsErrors))
            {
                fae = new JObject();
            }
            else
            {
                fae = JObject.Parse(this.FailedAttemptsErrors);
            }
            fae.Add(this.FailedAttempts.ToString(), exception);
            this.FailedAttemptsErrors = fae.ToString();
            this.FailedAttempts++;
            this.ModifyDate = DateTime.Now;
        }
    }

    public class EmailConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            builder.ToTable("Emails");
        }
    }

    //public enum EmailStatus
    //{
    //    [Description("Предстоящо изпращане")]
    //    Pending = 1,

    //    [Description("Изпратен")]
    //    Sent = 2,

    //    [Description("Грешка")]
    //    UknownError = 3,
    //}

    public class EmailStatus
    {
        public const int Pending = 1;
        public const int Sent = 2;
        public const int UknownError = 3;
    }
}
