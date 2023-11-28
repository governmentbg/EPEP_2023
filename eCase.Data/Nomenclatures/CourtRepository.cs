using System.Linq;

using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class CourtRepository : EntityCodeNomsRepository<Court, EntityCodeNomVO>
    {
        public CourtRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.CourtId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.CourtId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}