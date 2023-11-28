using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Entity, използван в уеб услугата Юридическо лице
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Entity
    {
        /// <summary>
        /// Име на юридическото лице
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// ЕИК / Булстат
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Bulstat { get; set; }

        /// <summary>
        /// Адрес
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Address { get; set; }
    }
}