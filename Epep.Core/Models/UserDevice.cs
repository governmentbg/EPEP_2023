using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Epep.Core.Models
{
    public class UserDevice
    {
        [Key]
        public int Id { get; set; }

        public int UserRegistrationId { get; set; }

        public Guid Gid { get; set; }

        public DateTime DateAdded { get; set; }

        public string DeviceId { get; set; }
        public string DeviceName { get; set; }

        public DateTime? DateRemoved { get; set; }

        [ForeignKey(nameof(UserRegistrationId))]
        public UserRegistration UserRegistration { get; set; }

    }
}
