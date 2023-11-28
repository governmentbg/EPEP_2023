using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта ActPreparator, използван в уеб услугата Подготовчик на
    /// съдебен акт
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class ActPreparator
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? ActPreparatorId { get; set; }

        /// <summary>
        /// Идентификатор на съдебен акт
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid ActId { get; set; }

        /// <summary>
        /// Име на съдия
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string JudgeName { get; set; }

        /// <summary>
        /// Роля: председател, член на състава
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