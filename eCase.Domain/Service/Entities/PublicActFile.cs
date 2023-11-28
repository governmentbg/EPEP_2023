using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта PublicActFile, използван в уеб услугата Файл на cъдебен акт със заличени данни
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class PublicActFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? PublicActFileId { get; set; }

        /// <summary>
        /// Идентификатор на акт
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid ActId { get; set; }

        /// <summary>
        /// Mime тип на съдебния акт
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string PublicActMimeType { get; set; }

        /// <summary>
        /// Съдържание на съдебния акт със заличени данни
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] PublicActContent { get; set; }
    }
}