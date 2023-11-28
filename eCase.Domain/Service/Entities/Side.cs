using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта Side, използван в уеб услугата Страна по дело
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class Side
    {
        /// <summary>
        /// Идентификатор
        /// Полето не е задължително
        /// При попълване централизираната база няма да я генерира
        /// </summary>
        [DataMember]
        public Guid? SideId { get; set; }

        /// <summary>
        /// Идентификатор на дело
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid CaseId { get; set; }

        /// <summary>
        /// Код на вид на страната
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string SideInvolvementKindCode { get; set; }

        /// <summary>
        /// Физическо лице
        /// Полето не е задължително, едно от двете полета Person или Entity трябва да е попълнено
        /// </summary>
        [DataMember]
        public Person Person { get; set; }

        /// <summary>
        /// Юридическо лице
        /// Полето не е задължително, едно от двете полета Person или Entity трябва да е попълнено
        /// </summary>
        [DataMember]
        public Entity Entity { get; set; }

        /// <summary>
        /// Активен
        /// Процесуално качество на страната
        /// </summary>
        [DataMember]
        public string ProceduralRelation { get; set; }

        /// <summary>
        /// Активен
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Дата на запис на страната
        /// Полето не е задължително
        /// </summary>
        [DataMember]
        public DateTime InsertDate { get; set; }
    }
}