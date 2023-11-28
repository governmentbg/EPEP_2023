using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта UserAssignment, използван в уеб услугата Назначенo лице
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v3")]
    public class UserAssignment
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? UserAssignmentId { get; set; }

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
        public Guid SideId { get; set; }

        /// <summary>
        /// Качество на лицето в делото получателя
        /// 1 - Адвокат
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public int AssignmentRole { get; set; }

        /// <summary>
        /// Идентификатор на регистрирано лице
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid UserRegistrationId { get; set; }

        /// <summary>
        /// Активен
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }
    }
}