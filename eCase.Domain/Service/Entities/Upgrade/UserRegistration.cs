using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта UserRegistrationModel, използван в уеб услугата Информация на физическо лице
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v3")]
    public class UserRegistration
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [DataMember]
        public Guid UserRegistrationId { get; set; }

        /// <summary>
        /// Тип лице: 1-Физическо лице, 8-Организация
        /// </summary>
        [DataMember]
        public int UserType { get; set; }

        /// <summary>
        /// Електронна поща
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Име на лице/ Наименование на организация
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// ЕГН/ЕИК
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string UIC { get; set; }

        /// <summary>
        /// Номер на адвокат
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string LawyerNumber { get; set; }

        /// <summary>
        /// Адрес
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// Дата на последна промяна
        /// </summary>
        [DataMember]
        public DateTime ModifyDate { get; set; }
    }
}