using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Epep.Core.Models
{
    public partial class Role
    {
        public const string ViewCase = "ViewCase";
        public const string ViewSummon = "ViewSummon";
        public const string UnrestrictedCourtFilter = "UnrestrictedCourtFilter";
        public const string GrantLawyerAccess = "GrantLawyerAccess";

        public Role()
        {
            this.UserGroups = new List<UserGroup>();
        }

        public long RoleId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Primary Key
            builder.HasKey(t => t.RoleId);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            builder.ToTable("Roles");
            builder.Property(t => t.RoleId).HasColumnName("RoleId");
            builder.Property(t => t.Name).HasColumnName("Name");

            // Relationships
            builder.HasMany(t => t.UserGroups)
                .WithMany(t => t.Roles);
                //.Map(m =>
                //{
                //    m.ToTable("UserGroupRoles");
                //    m.MapLeftKey("RoleId");
                //    m.MapRightKey("UserGroupId");
                //});
        }
    }
}
