using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Summon, използван в уеб услугата Призовка/съобщение
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Summon
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? SummonId { get; set; }

        /// <summary>
        /// Идентификатор на връзката (дело, заседание, акт ...)
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Идентификатор на връзката страна
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid SideId { get; set; }

        /// <summary>
        /// Вид на призовката/съобщението
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string SummonKind { get; set; }

        /// <summary>
        /// Тип на призовката/съобщението
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string SummonTypeCode { get; set; }

        /// <summary>
        /// Номер на призовката/съобщението
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// Дата на създаване
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Дата на връчване
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public DateTime? DateServed { get; set; }

        /// <summary>
        /// Адресат на призовката/съобщението
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Addressee { get; set; }

        /// <summary>
        /// Адрес на призовката/съобщението
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// Предмет на призовката/съобщението
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Subject { get; set; }
    }
}