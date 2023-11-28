using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class SideInvolvementKindRepository : EntityCodeNomsRepository<SideInvolvementKind, EntityCodeNomVO>
    {
        public SideInvolvementKindRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.SideInvolvementKindId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.SideInvolvementKindId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}