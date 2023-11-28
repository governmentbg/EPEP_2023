using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта PublicProtocolFile, използван в уеб услугата Файл на протокол от заседание със заличени данни
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class PublicProtocolFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? PublicProtocolFileId { get; set; }

        /// <summary>
        /// Идентификатор на заседание
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid HearingId { get; set; }

        /// <summary>
        /// Mime тип на протокола
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string ProtocolMimeType { get; set; }

        /// <summary>
        /// Съдържание на протокола
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] ProtocolContent { get; set; }
    }
}