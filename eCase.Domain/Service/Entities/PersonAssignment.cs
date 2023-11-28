using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта PersonAssignment, използван в уеб услугата Назначенo
    /// физическо лице
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class PersonAssignment
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? PersonAssignmentId { get; set; }

        /// <summary>
        /// Дата на назначение
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Идентификатор на страна по делото
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? SideId { get; set; }

        /// <summary>
        /// Идентификатор на регистрирано лице
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid PersonRegistrationId { get; set; }

        /// <summary>
        /// Активен
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }
    }
}