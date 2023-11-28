using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта ScannedDocument, използван в уеб услугата Сканиран
    /// документ
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class ScannedDocument
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? ScannedDocumentId { get; set; }

        /// <summary>
        /// Идентификатор на дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid CaseId { get; set; }

        /// <summary>
        /// Mime тип на прикачения документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string ScannedDocumentMimeType { get; set; }

        /// <summary>
        /// Съдържание на рикачения документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] ScannedDocumentContent { get; set; }

        /// <summary>
        /// Описание на прикачения документ
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}