using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Hearing, използван в уеб услугата Заседание
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Hearing
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? HearingId { get; set; }

        /// <summary>
        /// Идентификатор на дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid CaseId { get; set; }

        /// <summary>
        /// Тип на заседанието
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string HearingType { get; set; }

        /// <summary>
        /// Код на резултат от заседание
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string HearingResult { get; set; }

        /// <summary>
        /// Дата на заседание
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime Date { get; set; }

        /// <summary>
        /// Име на секретар
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string SecretaryName { get; set; }

        /// <summary>
        /// Име на прокурор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string ProsecutorName { get; set; }

        /// <summary>
        /// Съдебна зала
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string CourtRoom { get; set; }

        /// <summary>
        /// Адрес за провеждане на онлайн заседание
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string VideoUrl { get; set; }

        /// <summary>
        /// Отменено
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public bool IsCanceled { get; set; }


    }
}