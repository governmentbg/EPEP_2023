using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class ActDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Act> acts = new List<Act>();

        public ActDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding acts");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var act = new Act
                        {
                            ActId = Guid.NewGuid(),                             
                            ActKindCode = Nomenclatures.actKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.actKindCodes.Length - 1)],
                            CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid(),
                            HearingId = HearingDataGenerator.hearings[this.Random.GetRandomNumber(0, HearingDataGenerator.hearings.Count - 1)].HearingId ?? Guid.NewGuid(),
                            Number = this.Random.GetRandomNumber(1, 50),
                            DateSigned = this.Random.GetRandomDate(),
                            DateInPower = this.Random.GetRandomDate(),
                            MotiveDate = this.Random.GetRandomDate()
                        };

                        client.InsertAct(act);
                        acts.Add(act);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Act №{0} throw exception: {1}", i, ex);
                }
            }

            Console.WriteLine("\nActs added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating acts");
                for (int i = 0; i < this.Count; i++)
                {
                    var act = acts[this.Random.GetRandomNumber(0, acts.Count - 1)];

                    act.ActKindCode = Nomenclatures.actKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.actKindCodes.Length - 1)];
                    act.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    act.HearingId = HearingDataGenerator.hearings[this.Random.GetRandomNumber(0, HearingDataGenerator.hearings.Count - 1)].HearingId ?? Guid.NewGuid();
                    act.Number = this.Random.GetRandomNumber(1, 50);
                    act.DateSigned = this.Random.GetRandomDate();
                    act.DateInPower = this.Random.GetRandomDate();
                    act.MotiveDate = this.Random.GetRandomDate();

                    client.UpdateAct(act);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nActs updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random act");

                var act = acts[this.Random.GetRandomNumber(0, acts.Count - 1)];

                client.DeleteAct(act.ActId ?? Guid.NewGuid());
                acts.Remove(act);
            }
        }
    }
}
