using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта PersonRegistration, използван в уеб услугата Регистрация на физическо лице
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class PersonRegistration
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? PersonRegistrationId { get; set; }

        /// <summary>
        /// Електронна поща
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Име
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// ЕГН
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string EGN { get; set; }

        /// <summary>
        /// Рождена дата
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Адрес
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// Допълнително описание
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}