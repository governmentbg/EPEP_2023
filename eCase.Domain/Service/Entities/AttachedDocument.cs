using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта AttachedDocument, използван в уеб услугата Прикачени документи
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class AttachedDocument
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? AttachedDocumentId { get; set; }

        /// <summary>
        /// Тип на основен обект
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public int Type { get; set; }

        /// <summary>
        /// Идентификатор на основен обект
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Име на файл
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        /// Съдържание на файла
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] FileContent { get; set; }

        /// <summary>
        /// Описание на файла
        /// </summary>
        [DataMember]
        public string FileTitle { get; set; }

        /// <summary>
        /// Mime тип на файла
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string MimeType { get; set; }

        /// <summary>
        /// Дата на файла
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime FileDate { get; set; }
    }
}