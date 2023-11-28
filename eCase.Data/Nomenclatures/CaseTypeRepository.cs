using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class CaseTypeRepository : EntityCodeNomsRepository<CaseType, EntityCodeNomVO>
    {
        public CaseTypeRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.CaseTypeId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.CaseTypeId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}