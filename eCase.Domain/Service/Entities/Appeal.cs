using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Appeal, използван в уеб услугата Обжалване на съдебен акт
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Appeal
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? AppealId { get; set; }

        /// <summary>
        /// Идентификатор на съдебен акт
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid ActId { get; set; }

        /// <summary>
        /// Код на вид на обжалването
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string AppealKindCode { get; set; }

        /// <summary>
        /// Идентификатор на страна по делото
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid SideId { get; set; }

        /// <summary>
        /// Дата на подаване
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime DateFiled { get; set; }
    }
}