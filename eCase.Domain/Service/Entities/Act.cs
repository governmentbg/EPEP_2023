using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Act, използван в уеб услугата Съдебен акт
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Act
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? ActId { get; set; }

        /// <summary>
        /// Идентификатор на дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid CaseId { get; set; }

        /// <summary>
        /// Код на вид на акта
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string ActKindCode { get; set; }

        /// <summary>
        /// Идентификатор на заседание
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public Guid? HearingId { get; set; }

        /// <summary>
        /// Номер
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public int? Number { get; set; }

        /// <summary>
        /// Дата на подписване
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public DateTime DateSigned { get; set; }

        /// <summary>
        /// Дата на влизане в сила
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public DateTime? DateInPower { get; set; }

        /// <summary>
        /// Дата на мотив
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public DateTime? MotiveDate { get; set; }

        /// <summary>
        /// Финализиращ акт
        /// Полето не е задължително
        /// </summary>

        //[DataMember]
        //public bool? Finishing { get; set; } //comment for PROD

        /// <summary>
        /// Подлежи на обжалване
        /// Полето не е задължително
        /// </summary>

        //[DataMember]
        //public bool? CanBeSubjectToAppeal { get; set; } //comment for PROD
    }
}