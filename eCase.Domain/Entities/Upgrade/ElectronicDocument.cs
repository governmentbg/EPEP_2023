using eCase.Domain.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCase.Domain.Entities.Upgrade
{
    [Table("ElectronicDocuments")]
    public class ElectronicDocument : IGidRoot
    {
        public ElectronicDocument()
        {
            Sides = new HashSet<ElectronicDocumentSide>();
        }

        public long Id { get; set; }
        public Guid Gid { get; set; }
        public long? CourtId { get; set; }
        public long? CaseId { get; set; }
        public long? SideId { get; set; }
        public int DocumentKind { get; set; }
        public long? ElectronicDocumentTypeId { get; set; }

        public long? MoneyCurrencyId { get; set; }
        public decimal? BaseAmount { get; set; }
        public long? MoneyPricelistId { get; set; }
        public long UserId { get; set; }
        public long CreateUserId { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public string NumberApply { get; set; }
        public DateTime? DateApply { get; set; }
        public Guid? BlobKeyTimeApply { get; set; }
        public Guid? BlobKeyDocumentApply { get; set; }
        public decimal? TaxAmount { get; set; }
        public DateTime? DatePaid { get; set; }
        public DateTime? DateCourtAccept { get; set; }
        public string CourtDocumentNumber { get; set; }
        public DateTime? CourtDocumentDate { get; set; }
        public DateTime? CourtDeniedDate { get; set; }


        [ForeignKey(nameof(CourtId))]
        public virtual Court Court { get; set; }

        [ForeignKey(nameof(CaseId))]
        public virtual Case Case { get; set; }

        [ForeignKey(nameof(SideId))]
        public virtual Side Side { get; set; }

        [ForeignKey(nameof(ElectronicDocumentTypeId))]
        public virtual ElectronicDocumentType ElectronicDocumentType { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual UserRegistration User { get; set; }

        [ForeignKey(nameof(CreateUserId))]
        public virtual UserRegistration CreateUser { get; set; }

        [ForeignKey(nameof(BlobKeyTimeApply))]
        public virtual Blob BlobTimeApply { get; set; }

        [ForeignKey(nameof(BlobKeyDocumentApply))]
        public virtual Blob BlobDocumentApply { get; set; }

        public virtual ICollection<ElectronicDocumentSide> Sides { get; set; }

        public virtual ICollection<IncomingDocument> IncomingDocuments { get; set; }

        [ForeignKey(nameof(MoneyCurrencyId))]
        public virtual MoneyCurrency Currency { get; set; }

        [ForeignKey(nameof(MoneyPricelistId))]
        public virtual MoneyPricelist MoneyPricelist { get; set; }

    }

}
