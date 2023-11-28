using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта OutgoingDocuments, използван в уеб услугата Изходящ документ
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class OutgoingDocument
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? OutgoingDocumentId { get; set; }

        /// <summary>
        /// Идентификатор на дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid? CaseId { get; set; }

        /// <summary>
        /// Физическо лице
        /// Полето не е задължително, едно от двете полета Person или Entity трябва да е попълнено
        /// </summary>
        [DataMember]
        public Person Person { get; set; }

        /// <summary>
        /// Юридическо лице
        /// Полето не е задължително, едно от двете полета Person или Entity трябва да е попълнено
        /// </summary>
        [DataMember]
        public Entity Entity { get; set; }

        /// <summary>
        /// Изходящ номер
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public int OutgoingNumber { get; set; }

        /// <summary>
        /// Изходяща дата
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public DateTime? OutgoingDate { get; set; }

        /// <summary>
        /// Код на типа на изходящ документ
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string OutgoingDocumentTypeCode { get; set; }
    }
}