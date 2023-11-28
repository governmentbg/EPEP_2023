using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class CaseKindRepository : EntityCodeNomsRepository<CaseKind, EntityCodeNomVO>
    {
        public CaseKindRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.CaseKindId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.CaseKindId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}