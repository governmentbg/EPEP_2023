using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class OutgoingDocumentTypeRepository : EntityCodeNomsRepository<OutgoingDocumentType, EntityCodeNomVO>
    {
        public OutgoingDocumentTypeRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.OutgoingDocumentTypeId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.OutgoingDocumentTypeId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}