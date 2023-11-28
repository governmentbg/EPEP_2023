using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class HearingDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Hearing> hearings = new List<Hearing>();

        public HearingDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding hearings");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var hearing = new Hearing
                        {
                            HearingId = Guid.NewGuid(),
                            CaseId =
                                CaseDataGenerator.cases[
                                    this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ??
                                Guid.NewGuid(),
                            Date = this.Random.GetRandomDate(),
                            HearingType = this.Random.GetRandomStringWithRandomLength(3, 15),
                            HearingResult = this.Random.GetRandomStringWithRandomLength(10, 25),
                            ProsecutorName = this.Random.GetRandomStringWithRandomLength(5, 15),
                            SecretaryName = this.Random.GetRandomStringWithRandomLength(5, 15),
                            CourtRoom = this.Random.GetRandomStringWithRandomLength(5, 15),
                            IsCanceled = i % 2 == 0
                        };

                        client.InsertHearing(hearing);
                        hearings.Add(hearing);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hearing No:{0} throw exception {1}.", i, ex);
                }
            }

             Console.WriteLine("\nHearings added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating hearings");
                for (int i = 0; i < this.Count; i++)
                {
                    var hearing = hearings[this.Random.GetRandomNumber(0, hearings.Count - 1)];

                    hearing.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    hearing.Date = this.Random.GetRandomDate();
                    hearing.HearingType = this.Random.GetRandomStringWithRandomLength(3, 15);
                    hearing.HearingResult = this.Random.GetRandomStringWithRandomLength(10, 25);
                    hearing.ProsecutorName = this.Random.GetRandomStringWithRandomLength(5, 15);
                    hearing.SecretaryName = this.Random.GetRandomStringWithRandomLength(5, 15);
                    hearing.CourtRoom = this.Random.GetRandomStringWithRandomLength(5, 15);
                    hearing.IsCanceled = i % 2 == 0;

                    client.UpdateHearing(hearing);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nHearings updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random hearing");

                var hearing = hearings[this.Random.GetRandomNumber(0, hearings.Count - 1)];

                client.DeleteHearing(hearing.HearingId ?? Guid.NewGuid());
                hearings.Remove(hearing);
            }
        }
    }
}
