using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта PrivateActFile, използван в уеб услугата Файл на cъдебен акт без заличени данни
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class PrivateActFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? PrivateActFileId { get; set; }

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
        public string PrivateActMimeType { get; set; }

        /// <summary>
        /// Съдържание на съдебния акт без заличени данни
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public byte[] PrivateActContent { get; set; }
    }
}