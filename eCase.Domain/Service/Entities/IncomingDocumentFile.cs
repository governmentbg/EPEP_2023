using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта IncomingDocumentFile, използван в уеб услугата Филе на входящ документ
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class IncomingDocumentFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? IncomingDocumentFileId { get; set; }

        /// <summary>
        /// Идентификатор на входящ документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid IncomingDocumentId { get; set; }

        /// <summary>
        /// Mime тип на входящ документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string IncomingDocumentMimeType { get; set; }

        /// <summary>
        /// Съдържание на входящ документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] IncomingDocumentContent { get; set; }
    }
}