using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class LawyerRegistrationDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Guid?> lawyerRegistrationIds = new List<Guid?>();

        public LawyerRegistrationDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding lawyer registrations");
                for (int i = 0; i < this.Count; i++)
                {
                    var lawyerRegistration = new LawyerRegistration()
                    {
                        LawyerRegistrationId = Guid.NewGuid(),
                        LawyerId = new Guid(Constants.LAWYER_ID),
                        Email = Constants.EMAIL,
                        BirthDate = DateTime.Parse(Constants.BIRTH_DATE),
                        Description = this.Random.GetRandomStringWithRandomLength(3, 15)
                    };

                    var guid = client.InsertLawyerRegistration(lawyerRegistration);
                    lawyerRegistrationIds.Add(guid);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nLawyer registrations added");
        }

        public override void Update()
        {
        }

        public override void Delete()
        {
        }
    }
}
