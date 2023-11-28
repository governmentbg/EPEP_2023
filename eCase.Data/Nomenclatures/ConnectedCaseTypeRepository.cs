using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class ConnectedCaseTypeRepository : EntityCodeNomsRepository<ConnectedCaseType, EntityCodeNomVO>
    {
        public ConnectedCaseTypeRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.ConnectedCaseTypeId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.ConnectedCaseTypeId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}