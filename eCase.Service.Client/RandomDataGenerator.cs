using System;

namespace eCase.Service.Client
{
    public class RandomDataGenerator : IRandomDataGenerator
    {
        private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private static IRandomDataGenerator randomDataGenerator;
        private Random random;

        private RandomDataGenerator()
        {
            this.random = new Random();
        }

        public static IRandomDataGenerator Instance
        {
            get
            {
                if (randomDataGenerator == null)
                {
                    randomDataGenerator = new RandomDataGenerator();
                }

                return randomDataGenerator;
            }
        }

        public int GetRandomNumber(int min, int max)
        {
            return this.random.Next(min, max + 1);
        }

        public string GetRandomString(int length)
        {
            var result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = Letters[this.GetRandomNumber(0, Letters.Length - 1)];
            }

            return new string(result);
        }

        public string GetRandomStringWithRandomLength(int min, int max)
        {
            return this.GetRandomString(this.GetRandomNumber(min, max));
        }

        public byte[] GetRandomByteArrayWithRandomLength(int min, int max)
        {
            byte[] randBytes = new byte[this.GetRandomNumber(min, max)];

            this.random.NextBytes(randBytes);

            return randBytes;
        }

        public DateTime GetRandomDate()
        {
            int year = this.random.Next(1900, 2015);
            int month = this.random.Next(1, 12);
            int day = DateTime.DaysInMonth(year, month);

            int Day = this.random.Next(1, day);

            return new DateTime(year, month, Day);
        }
    }
}
