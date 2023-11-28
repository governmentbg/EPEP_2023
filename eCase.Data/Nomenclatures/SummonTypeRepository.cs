using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class SummonTypeRepository : EntityCodeNomsRepository<SummonType, EntityCodeNomVO>
    {
        public SummonTypeRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.SummonTypeId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.SummonTypeId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}