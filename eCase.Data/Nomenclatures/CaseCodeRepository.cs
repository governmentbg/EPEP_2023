using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class CaseCodeRepository : EntityCodeNomsRepository<CaseCode, EntityCodeNomVO>
    {
        public CaseCodeRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.CaseCodeId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.CaseCodeId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}