using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта LаwyerAssignment, използван в уеб услугата Назначен
    /// адвокат
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class LawyerAssignment
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? LawyerAssignmentId { get; set; }

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
        /// Идентификатор на регистрация на адвокат
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid LawyerRegistrationId { get; set; }

        /// <summary>
        /// Активен
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }
    }
}