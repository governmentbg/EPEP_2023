using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class CaseRulingKindRepository : EntityCodeNomsRepository<CaseRulingKind, EntityCodeNomVO>
    {
        public CaseRulingKindRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.CaseRulingKindId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.CaseRulingKindId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}