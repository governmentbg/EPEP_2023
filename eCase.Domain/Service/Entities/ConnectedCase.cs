using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта ConnectedCase, използван в уеб услугата Свързано дело
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class ConnectedCase
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid CaseId { get; set; }

        /// <summary>
        /// Идентификатор на предходно дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid PredecessorCaseId { get; set; }

        /// <summary>
        /// Код на общ вид на свързано дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string ConnectedCaseTypeCode { get; set; }
    }
}