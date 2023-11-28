using System;

namespace eCase.Service.Client
{
    public interface IRandomDataGenerator
    {
        int GetRandomNumber(int min, int max);

        string GetRandomString(int length);

        string GetRandomStringWithRandomLength(int min, int max);

        byte[] GetRandomByteArrayWithRandomLength(int min, int max);

        DateTime GetRandomDate();
    }
}
