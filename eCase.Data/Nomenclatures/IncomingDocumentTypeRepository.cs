using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class IncomingDocumentTypeRepository : EntityCodeNomsRepository<IncomingDocumentType, EntityCodeNomVO>
    {
        public IncomingDocumentTypeRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.IncomingDocumentTypeId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.IncomingDocumentTypeId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}