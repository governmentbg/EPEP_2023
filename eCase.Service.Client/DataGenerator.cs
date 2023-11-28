namespace eCase.Service.Client
{
    public abstract class DataGenerator : IDataGenerator
    {
        private IRandomDataGenerator random;
        private int count;

        public DataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
        {
            this.random = randomDataGenerator;
            this.count = countOfGeneratedObjects;
        }

        protected IRandomDataGenerator Random
        {
            get
            {
                return this.random;
            }
        }

        protected int Count
        {
            get
            {
                return this.count;
            }
        }

        public abstract void Insert();

        public abstract void Update();

        public abstract void Delete();
    }
}
