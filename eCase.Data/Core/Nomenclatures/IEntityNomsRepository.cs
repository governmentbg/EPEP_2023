using System.Collections.Generic;

namespace eCase.Data.Core.Nomenclatures
{
    public interface IEntityNomsRepository<TEntity, out TNomVO> : IRepository
    {
        TNomVO GetNom(long nomValueId);
        IEnumerable<TNomVO> GetNoms(string term, int offset = 0, int? limit = null);
    }
}
