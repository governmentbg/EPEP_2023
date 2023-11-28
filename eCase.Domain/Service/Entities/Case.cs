using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Case, използван в уеб услугата Дело
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Case
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? CaseId { get; set; }

        /// <summary>
        /// Идентификатор на входящ документ
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? IncomingDocumentId { get; set; }

        /// <summary>
        /// Код на съд
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string CourtCode { get; set; }

        /// <summary>
        /// Код на вид на делото
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string CaseKindCode { get; set; }

        /// <summary>
        /// Код на общ вид на делото
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string CaseTypeCode { get; set; }

        /// <summary>
        /// Код на делото
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string CaseCode { get; set; }

        /// <summary>
        /// Статистически код
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string StatisticCode { get; set; }

        /// <summary>
        /// Номер
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public int Number { get; set; }

        /// <summary>
        /// Година на образуване
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public int CaseYear { get; set; }

        /// <summary>
        /// Статус
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// Дата на образуване
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime FormationDate { get; set; }

        /// <summary>
        /// Отделение в което е образувано
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string DepartmentName { get; set; }

        /// <summary>
        /// Име на съдебен състав
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string PanelName { get; set; }

        /// <summary>
        /// Предмет на делото
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public string LegalSubject { get; set; }


        /// <summary>
        /// Дело с ограничен достъп
        /// Полето не е задължително
        /// </summary>

        [DataMember]
        public bool? RestrictedAccess { get; set; } //comment for PROD
    }
}