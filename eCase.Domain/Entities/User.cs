using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Web.Helpers;

using eCase.Domain.Core;
using eCase.Domain.Events;

namespace eCase.Domain.Entities
{
    public partial class User : IAggregateRoot, IEventEmitter
    {
        private User()
        {
            ((IEventEmitter)this).Events = new List<IDomainEvent>();
        }

        public User(Guid gid, long groupId, string name, string email)
            : this()
        {
            this.Gid = gid;
            this.UserGroupId = groupId;
            this.Name = name;
            this.Username = email;
            this.SetPassword(Guid.NewGuid().ToString());
            this.ActivationCode = Guid.NewGuid().ToString();
            this.IsActivationCodeValid = true;

            ((IEventEmitter)this).Events.Add(new NewRegistrationEvent()
            {
                Email = email,
                ActivationCode = this.ActivationCode
            });
        }

        public long UserId { get; set; }
        public Guid Gid { get; set; }
        public long? CourtId { get; set; }
        public long UserGroupId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string ActivationCode { get; set; }
        public bool IsActivationCodeValid { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public byte[] Version { get; set; }
        public virtual Court Court { get; set; }
        public virtual LawyerRegistration LawyerRegistration { get; set; }
        public virtual PersonRegistration PersonRegistration { get; set; }
        public virtual UserGroup UserGroup { get; set; }

        ICollection<IDomainEvent> IEventEmitter.Events { get; set; }

        public void ActivateUser()
        {
            if (this.IsActive)
            {
                throw new DomainException("Cannot activate a user more than once");
            }
            else
            {
                this.IsActive = true;
            }
        }

        public void ChangeUserGroup(long userGroupId)
        {

            this.UserGroupId = userGroupId;

        }

        public bool VerifyPassword(string password)
        {
            if (!string.IsNullOrEmpty(this.PasswordHash) &&
                !string.IsNullOrEmpty(this.PasswordSalt))
            {
                return Crypto.VerifyHashedPassword(this.PasswordHash, password + this.PasswordSalt);
            }
            else
            {
                return false;
            }
        }

        public void ChangePassword(string newPassword)
        {
            this.SetPassword(newPassword);
            this.ModifyDate = DateTime.Now;
        }

        public void SetPassword(string password)
        {
            if (password == null)
            {
                this.PasswordSalt = null;
                this.PasswordHash = null;
            }
            else
            {
                this.PasswordSalt = Crypto.GenerateSalt();
                this.PasswordHash = Crypto.HashPassword(password + this.PasswordSalt);
            }
        }

        public void ForgottenPassword()
        {
            this.ActivationCode = Guid.NewGuid().ToString();
            this.IsActivationCodeValid = true;

            ((IEventEmitter)this).Events.Add(new ForgottenPasswordEvent()
            {
                Email = this.Username,
                ActivationCode = this.ActivationCode
            });
        }

        public void UpdateUsername(string oldUsername, string newUsername, string courtName)
        {
            this.Username = newUsername;
            this.ActivationCode = Guid.NewGuid().ToString();
            this.IsActivationCodeValid = true;

            ((IEventEmitter)this).Events.Add(new ChangeUserNameEvent()
            {
                Email = newUsername,
                ActivationCode = this.ActivationCode,
                CourtName = courtName,
                OldUserName = oldUsername
            });
        }

        public void UpdateUserProfile(string email, string name, string isActivated, string courtName)
        {
            ((IEventEmitter)this).Events.Add(new ChangeUserProfileEvent()
            {
                Email = email,
                Name = name,
                IsActivated = isActivated,
                CourtName = courtName
            });
        }

        public void ActivateSummonsAccess(string email, string caseAbbr, string courtName)
        {
            ((IEventEmitter)this).Events.Add(new SummonAccessActivationEvent()
            {
                Email = email,
                CaseAbbr = caseAbbr,
                CourtName = courtName
            });
        }

        public void DectivateSummonsAccess(string email, string caseAbbr, string courtName)
        {
            ((IEventEmitter)this).Events.Add(new SummonAccessDeactivationEvent()
            {
                Email = email,
                CaseAbbr = caseAbbr,
                CourtName = courtName
            });
        }
    }

    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            // Primary Key
            this.HasKey(t => t.UserId);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Username)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.PasswordHash)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.PasswordSalt)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.ActivationCode)
                .HasMaxLength(200);

            this.Property(t => t.Version)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(8)
                .IsRowVersion();

            // Table & Column Mappings
            this.ToTable("Users");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.Gid).HasColumnName("Gid");
            this.Property(t => t.CourtId).HasColumnName("CourtId");
            this.Property(t => t.UserGroupId).HasColumnName("UserGroupId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Username).HasColumnName("Username");
            this.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            this.Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
            this.Property(t => t.ActivationCode).HasColumnName("ActivationCode");
            this.Property(t => t.IsActivationCodeValid).HasColumnName("IsActivationCodeValid");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.ModifyDate).HasColumnName("ModifyDate");
            this.Property(t => t.Version).HasColumnName("Version");

            // Relationships
            this.HasOptional(t => t.Court)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.CourtId);
            this.HasRequired(t => t.UserGroup)
                .WithMany(t => t.Users)
                .HasForeignKey(d => d.UserGroupId);
        }
    }
}
