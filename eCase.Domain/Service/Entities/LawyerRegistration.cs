using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта LawyerRegistration, използван в уеб услугата Регистрация на адвокат
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class LawyerRegistration
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? LawyerRegistrationId { get; set; }

        /// <summary>
        /// Идентификатор на адвокат
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid LawyerId { get; set; }

        /// <summary>
        /// Рождена дата на адвокат
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Електронна поща
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Допълнително описание
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}