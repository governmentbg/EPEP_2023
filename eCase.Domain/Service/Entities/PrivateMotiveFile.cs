using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта PrivateMotiveFile, използван в уеб услугата Файл на мотив без заличени данни
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class PrivateMotiveFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? PrivateMotiveFileId { get; set; }

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
        public string PrivateMotiveMimeType { get; set; }

        /// <summary>
        /// Съдържание на мотив без заличени данни
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] PrivateMotiveContent { get; set; }
    }
}