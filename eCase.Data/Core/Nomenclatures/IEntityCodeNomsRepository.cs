namespace eCase.Data.Core.Nomenclatures
{
    public interface IEntityCodeNomsRepository<TEntity, TCodeNomVO> : IEntityNomsRepository<TEntity, TCodeNomVO>
    {
        long GetNomIdByCode(string code);
        bool HasCode(string code);
    }
}
