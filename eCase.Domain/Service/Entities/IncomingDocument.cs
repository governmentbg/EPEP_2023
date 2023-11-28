using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта IncomingDocument, използван в уеб услугата Входящ документ
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class IncomingDocument
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? IncomingDocumentId { get; set; }

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
        /// Код на съд
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string CourtCode { get; set; }

        /// <summary>
        /// Входящ номер
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public int IncomingNumber { get; set; }

        /// <summary>
        /// Входяща дата
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public DateTime IncomingDate { get; set; }

        /// <summary>
        /// Код на типа на входящ документ
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string IncomingDocumentTypeCode { get; set; }

        /// <summary>
        /// Идентификатор на електронно подаден документ, по който е регистриран 
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? ElectronicDocumentId { get; set; }
    }
}