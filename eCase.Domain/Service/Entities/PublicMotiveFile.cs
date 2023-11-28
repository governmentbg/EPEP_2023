using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта PublicMotiveFile, използван в уеб услугата Файл на мотив със заличени данни
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class PublicMotiveFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? PublicMotiveFileId { get; set; }

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
        public string PublicMotiveMimeType { get; set; }

        /// <summary>
        /// Съдържание на мотив със заличени данни
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] PublicMotiveContent { get; set; }
    }
}