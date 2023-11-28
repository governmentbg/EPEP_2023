using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта AssignmentFile, използван в уеб услугата Файл на протокол за разпределение по делото
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class AssignmentFile
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? AssignmentFileId { get; set; }

        /// <summary>
        /// Идентификатор на протокол за разпределение
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid AssignmentId { get; set; }

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