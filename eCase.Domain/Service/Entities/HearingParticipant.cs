using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта HearingParticipant, използван в уеб услугата Участник в
    /// заседание
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class HearingParticipant
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? HearingParticipantId { get; set; }

        /// <summary>
        /// Идентификатор на заседание
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid HearingId { get; set; }

        /// <summary>
        /// Име на съдия
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string JudgeName { get; set; }

        /// <summary>
        /// Роля: председател, член на състав
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Role { get; set; }

        /// <summary>
        /// Име на заместващ
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string SubstituteFor { get; set; }

        /// <summary>
        /// Причина за заместване
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string SubstituteReason { get; set; }
    }
}