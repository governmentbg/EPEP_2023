using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта HearingDocument, използван в уеб услугата Документ, предоставен в заседание
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class HearingDocument
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? HearingDocumentId { get; set; }

        /// <summary>
        /// Идентификатор на заседание
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid HearingId { get; set; }

        /// <summary>
        /// Идентификатор на свързано лице
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid SideId { get; set; }

        /// <summary>
        /// Тип на документа
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string HearingDocumentKind { get; set; }
    }
}