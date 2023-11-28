namespace eCase.Data.Core
{
    public class DataUpdateConcurrencyException : DataException
    {
        public DataUpdateConcurrencyException()
            : base("Entity already modified")
        {
        }
    }
}
