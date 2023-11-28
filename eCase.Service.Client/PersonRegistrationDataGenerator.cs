using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class PersonRegistrationDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Guid?> personRegistrationGuids = new List<Guid?>();

        public PersonRegistrationDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (var client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding person registrations");
                for (int i = 0; i < this.Count; i++)
                {
                    var personRegistration = new PersonRegistration()
                    {
                        PersonRegistrationId = Guid.NewGuid(),
                        Name = "Person Registration",
                        Email = Constants.EMAIL,
                        EGN = "0000000000",
                        BirthDate = DateTime.Parse(Constants.BIRTH_DATE),
                        Address = this.Random.GetRandomStringWithRandomLength(3, 15),
                        Description = this.Random.GetRandomStringWithRandomLength(3, 15)                        
                    };

                    var guid = client.InsertPersonRegistration(personRegistration);
                    personRegistrationGuids.Add(guid);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPerson registrations added");
        }

        public override void Update()
        {
        }

        public override void Delete()
        {
        }
    }
}
