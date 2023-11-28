using System;
using System.Runtime.Serialization;

namespace eCase.Domain.Service.Entities
{
    /// <summary>
    /// Описва ред от обекта ElectronicDocumentSide, Страна към документ
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v3")]
    public class ElectronicDocumentSide
    {
        /// <summary>
        /// Идентификатор на страна електронно подаден документ
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public Guid ElectronicDocumentSideId { get; set; }
        
        /// <summary>
        /// Идентификатор на роля лице
        /// Полето е задължително
        /// </summary>
        [DataMember]
        public string SideInvolvementKind { get; set; }

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
    }
}