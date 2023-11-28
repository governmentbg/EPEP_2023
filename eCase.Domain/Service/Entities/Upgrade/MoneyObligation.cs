using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта MoneyObligation, вменени задължения 
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v3")]
    public class MoneyObligation
    {
        /// <summary>
        /// Тип задължение
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public int AttachmentType { get; set; }

        /// <summary>
        /// Идентификатор на електронно подаден документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Код на валута: BGN,EUR
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Сума на задължението, стойност в стотинки
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public int Amount { get; set; }

        /// <summary>
        /// Дата на възникване на задължението
        /// </summary>
        [DataMember]
        public DateTime DateCreate { get; set; }

        /// <summary>
        /// Описание на обекта, към който е задължението
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string ParentDescription { get; set; }

        /// <summary>
        /// Описание
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Description { get; set; }

    }
}