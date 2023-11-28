using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта CaseRuling, използван в уеб услугата Произнасяне на
    /// състава с отражение по хода на делото
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class CaseRuling
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? CaseRulingId { get; set; }

        /// <summary>
        /// Идентификатор на дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid CaseId { get; set; }

        /// <summary>
        /// Идентификатор на заседание
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? HearingId { get; set; }

        /// <summary>
        /// Идентификатор на съдебен акт
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? ActId { get; set; }

        /// <summary>
        /// Код на хода на делото
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string CaseRulingKindCode { get; set; }
    }
}