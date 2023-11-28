using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace eCase.Domain.Service.Entities
{
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class UserRegistrationInfo
    {
        [DataMember]
        public Guid? PersonRegistrationId { get; set; }
        [DataMember]
        public Guid? LawyerRegistrationId { get; set; }
        [DataMember]
        public bool IsRegistered { get; set; }
    }
}
