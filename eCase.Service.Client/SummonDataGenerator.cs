using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class SummonDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Summon> summons = new List<Summon>();

        public SummonDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding summons");
                for (int i = 0; i < this.Count; i++)
                {
                    var summon = new Summon
                    {
                        SummonId = Guid.NewGuid(),
                        SideId = SideDataGenerator.sides[this.Random.GetRandomNumber(0, SideDataGenerator.sides.Count - 1)].SideId ?? Guid.NewGuid(),
                        SummonKind = this.Random.GetRandomStringWithRandomLength(3, 10),
                        DateCreated = this.Random.GetRandomDate(),
                        DateServed = this.Random.GetRandomDate(),
                        Addressee = this.Random.GetRandomStringWithRandomLength(10, 50),
                        Address = this.Random.GetRandomStringWithRandomLength(15, 50),
                        Subject = this.Random.GetRandomStringWithRandomLength(10, 40),
                    };

                    switch (i % 4)
                    {
                        case 0: summon.SummonTypeCode = "1";
                            summon.ParentId = ActDataGenerator.acts[this.Random.GetRandomNumber(0, ActDataGenerator.acts.Count - 1)].ActId ?? Guid.NewGuid();
                            break;
                        case 1: summon.SummonTypeCode = "2";
                            summon.ParentId = AppealDataGenerator.appeals[this.Random.GetRandomNumber(0, AppealDataGenerator.appeals.Count - 1)].AppealId ?? Guid.NewGuid();
                            break;
                        case 2: summon.SummonTypeCode = "3";
                            summon.ParentId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                            break;
                        case 3: summon.SummonTypeCode = "4";
                            summon.ParentId = HearingDataGenerator.hearings[this.Random.GetRandomNumber(0, HearingDataGenerator.hearings.Count - 1)].HearingId ?? Guid.NewGuid();
                            break;
                        default:
                            break;
                    }

                    client.InsertSummon(summon, LawyerRegistrationDataGenerator.lawyerRegistrationIds[0].Value);
                    summons.Add(summon);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nSummons added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating summons");
                for (int i = 0; i < this.Count; i++)
                {
                    var summon = summons[this.Random.GetRandomNumber(0, summons.Count - 1)];

                    summon.SideId = SideDataGenerator.sides[this.Random.GetRandomNumber(0, SideDataGenerator.sides.Count - 1)].SideId ?? Guid.NewGuid();
                    summon.SummonKind = this.Random.GetRandomStringWithRandomLength(3, 10);
                    summon.DateCreated = this.Random.GetRandomDate();
                    summon.DateServed = this.Random.GetRandomDate();
                    summon.Addressee = this.Random.GetRandomStringWithRandomLength(10, 50);
                    summon.Address = this.Random.GetRandomStringWithRandomLength(15, 50);
                    summon.Subject = this.Random.GetRandomStringWithRandomLength(10, 40);

                    switch (i % 4)
                    {
                        case 0: summon.SummonTypeCode = "1";
                            summon.ParentId = ActDataGenerator.acts[this.Random.GetRandomNumber(0, ActDataGenerator.acts.Count - 1)].ActId ?? Guid.NewGuid();
                            break;
                        case 1: summon.SummonTypeCode = "2";
                            summon.ParentId = AppealDataGenerator.appeals[this.Random.GetRandomNumber(0, AppealDataGenerator.appeals.Count - 1)].AppealId ?? Guid.NewGuid();
                            break;
                        case 2: summon.SummonTypeCode = "3";
                            summon.ParentId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                            break;
                        case 3: summon.SummonTypeCode = "4";
                            summon.ParentId = HearingDataGenerator.hearings[this.Random.GetRandomNumber(0, HearingDataGenerator.hearings.Count - 1)].HearingId ?? Guid.NewGuid();
                            break;
                        default:
                            break;
                    }

                    client.UpdateSummon(summon);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nSummons updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random summon");

                var summon = summons[this.Random.GetRandomNumber(0, summons.Count - 1)];

                client.DeleteSummon(summon.SummonId ?? Guid.NewGuid());
                summons.Remove(summon);
            }
        }
    }
}
