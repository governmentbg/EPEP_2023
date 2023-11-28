using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
{
    public partial class Court
    {
        public Court()
        {
            this.Cases = new List<Case>();
            this.IncomingDocuments = new List<IncomingDocument>();
            this.Users = new List<User>();
        }

        public long CourtId { get; set; }
        public long CourtTypeId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsIntegrated { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
        public virtual CourtType CourtType { get; set; }
        public virtual ICollection<IncomingDocument> IncomingDocuments { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

    public class CourtMap : EntityTypeConfiguration<Court>
    {
        public CourtMap()
        {
            // Primary Key
            this.HasKey(t => t.CourtId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(4);

            // Table & Column Mappings
            this.ToTable("Courts");
            this.Property(t => t.CourtId).HasColumnName("CourtId");
            this.Property(t => t.CourtTypeId).HasColumnName("CourtTypeId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsIntegrated).HasColumnName("IsIntegrated");

            // Relationships
            this.HasRequired(t => t.CourtType)
                .WithMany(t => t.Courts)
                .HasForeignKey(d => d.CourtTypeId);
        }
    }
}
