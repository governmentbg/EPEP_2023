using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта ElectronicDocumentFile, Файл към документ
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v3")]
    public class ElectronicDocumentFile
    {
        /// <summary>
        /// Тип на документа
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public int AttachmentType { get; set; }

        /// <summary>
        /// Описание на файла
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Име на файла
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public long FileSize { get; set; }

        public Guid FileId { get; set; }

        /// <summary>
        /// Идентификатор на файл за изтегляне
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] Content { get; set; }
    }
}