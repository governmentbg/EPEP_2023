using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта SummonReadTimeResult, резултат при проверка на дата на връчване на призовка
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v3")]
    public class SummonReadTimeResult
    {
        /// <summary>
        /// Идентификатор за прочетена призовка
        /// </summary>
        [DataMember]
        public bool IsRead { get; set; }

        /// <summary>
        /// Дата на прочитане на призовката
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime? ReadDate { get; set; }

        /// <summary>
        /// Крайна дата на отсъствие на адвоката - когато е приложимо
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime? VacationEndDate { get; set; }
    }
}