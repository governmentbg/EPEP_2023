using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Reporter, използван в уеб услугата Съдия докладчик
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Reporter
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? ReporterId { get; set; }

        /// <summary>
        /// Идентификатор на дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid CaseId { get; set; }

        /// <summary>
        /// Име на съдия
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string JudgeName { get; set; }

        /// <summary>
        /// Дана на която е разпределен
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime DateAssigned { get; set; }

        /// <summary>
        /// Дата на замяна
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public DateTime? DateReplaced { get; set; }

        /// <summary>
        /// Причина на замяна
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string ReasonReplaced { get; set; }
    }
}