using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта ElectronicDocument, използван в уеб услугата Подадени електронни документи
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v3")]
    public class ElectronicDocument
    {
        /// <summary>
        /// Идентификатор на електронно подаден документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid ElectronicDocumentId { get; set; }

        /// <summary>
        /// Идентификатор на свързано дело, 
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? CaseId { get; set; }

        /// <summary>
        /// Идентификатор на лице по свързаното дело, 
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? SideId { get; set; }

        /// <summary>
        /// Идентификатор на лице, подало документа
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid UserRegistrationId { get; set; }

        /// <summary>
        /// Идентификатор на съд
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string CourtCode { get; set; }

        /// <summary>
        /// Идентификатор на вид документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string DocumentKind { get; set; }

        /// <summary>
        /// Идентификатор на основен тип документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string DocumentType { get; set; }

        /// <summary>
        /// Идентификатор на тарифа
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string PricelistCode { get; set; }


        /// <summary>
        /// Код на валута: BGN,EUR
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Материален интерес, стойност в стотинки
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public int? BaseAmount { get; set; }

        /// <summary>
        /// Изчислена такса, стойност в стотинки
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public int? TaxAmount { get; set; }

        /// <summary>
        /// Описание
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Номер на електронно подаден документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string NumberApply { get; set; }

        /// <summary>
        /// Дата на електронно подаден документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime DateApply { get; set; }

        /// <summary>
        /// Дата на електронно плащане
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime DatePaid { get; set; }

        /// <summary>
        /// Списък на страни по документ
        /// </summary>
        [DataMember]
        public ElectronicDocumentSide[] Sides { get; set; }

        /// <summary>
        /// Списък на Файлове по документ
        /// </summary>
        [DataMember]
        public ElectronicDocumentFile[] Files { get; set; }
    }
}