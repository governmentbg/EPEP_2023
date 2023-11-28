using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class ActKindRepository : EntityCodeNomsRepository<ActKind, EntityCodeNomVO>
    {
        public ActKindRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.ActKindId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.ActKindId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}