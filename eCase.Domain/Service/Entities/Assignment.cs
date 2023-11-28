using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Assignment, използван в уеб услугата Протокол за разпределение по делото
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Assignment
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? AssignmentId { get; set; }

        /// <summary>
        /// Идентификатор на дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid CaseId { get; set; }

        /// <summary>
        /// Идентификатор на входящ документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid IncomingDocumentId { get; set; }

        /// <summary>
        /// Начин на разпределение
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Дата на разпределение
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Име на съдия докладчик
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string JudgeName { get; set; }

        /// <summary>
        /// Име на извършител на разпределението
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Assignor { get; set; }
    }
}