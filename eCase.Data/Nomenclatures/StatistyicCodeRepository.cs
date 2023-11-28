using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Domain.Entities;

namespace eCase.Data.Nomenclatures
{
    internal class StatisticCodeRepository : EntityCodeNomsRepository<StatisticCode, EntityCodeNomVO>
    {
        public StatisticCodeRepository(IUnitOfWork unitOfWork)
            : base(
                unitOfWork,
                t => t.StatisticCodeId,
                t => t.Name,
                t => t.Code,
                t => new EntityCodeNomVO
                {
                    NomValueId = t.StatisticCodeId,
                    Code = t.Code,
                    Name = t.Name
                })
        {
        }
    }
}