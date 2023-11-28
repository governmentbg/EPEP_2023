using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта SummonFile, използван в уеб услугата Файл на призовка/съобщение
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class SummonFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? SummonFileId { get; set; }

        /// <summary>
        /// Идентификатор на призовка/съобщение
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid SummonId { get; set; }

        /// <summary>
        /// Mime тип на призовката/съобщението
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string MimeType { get; set; }

        /// <summary>
        /// Съдържание на призовката/съобщението
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] Content { get; set; }
    }
}