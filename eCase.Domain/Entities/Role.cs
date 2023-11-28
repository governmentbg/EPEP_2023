using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace eCase.Domain.Entities
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

    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            // Primary Key
            this.HasKey(t => t.RoleId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Roles");
            this.Property(t => t.RoleId).HasColumnName("RoleId");
            this.Property(t => t.Name).HasColumnName("Name");

            // Relationships
            this.HasMany(t => t.UserGroups)
                .WithMany(t => t.Roles)
                .Map(m =>
                {
                    m.ToTable("UserGroupRoles");
                    m.MapLeftKey("RoleId");
                    m.MapRightKey("UserGroupId");
                });
        }
    }
}
