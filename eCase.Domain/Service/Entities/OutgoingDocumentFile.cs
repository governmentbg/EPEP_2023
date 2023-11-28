using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта OutgoingDocumentFile, използван в уеб услугата Файл на изходящ документ
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class OutgoingDocumentFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? OutgoingDocumentFileId { get; set; }

        /// <summary>
        /// Идентификатор на изходящ документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid OutgoingDocumentId { get; set; }

        /// <summary>
        /// Mime тип на изходящ документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string OutgoingDocumentMimeType { get; set; }

        /// <summary>
        /// Съдържание на изходящ документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] OutgoingDocumentContent { get; set; }
    }
}