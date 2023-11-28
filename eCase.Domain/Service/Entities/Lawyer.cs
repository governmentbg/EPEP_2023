using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Lawyer, използван в уеб услугата Адвокат
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Lawyer
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? LawyerId { get; set; }

        /// <summary>
        /// Номер
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// Име
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Колегия
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string College { get; set; }
    }
}