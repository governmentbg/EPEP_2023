using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class AppealKindRepository : EntityCodeNomsRepository<AppealKind, EntityCodeNomVO>
    {
        public AppealKindRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.AppealKindId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.AppealKindId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}
