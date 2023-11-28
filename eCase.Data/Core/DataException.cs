using System;

namespace eCase.Data.Core
{
    public class DataException : Exception
    {
        public DataException()
        {
        }

        public DataException(string message)
            : base(message)
        {
        }
    }
}
