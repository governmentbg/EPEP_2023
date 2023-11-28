using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Person, използван в уеб услугата Физическо лице
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Person
    {
        /// <summary>
        /// Име
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Firstname { get; set; }

        /// <summary>
        /// Бащино име
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Secondname { get; set; }

        /// <summary>
        /// Фамилно име
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Lastname { get; set; }

        /// <summary>
        /// ЕГН
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string EGN { get; set; }

        /// <summary>
        /// Адрес
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Address { get; set; }
    }
}