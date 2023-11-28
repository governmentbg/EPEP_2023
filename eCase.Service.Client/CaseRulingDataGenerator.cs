using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class CaseRulingDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<CaseRuling> caseRulings = new List<CaseRuling>();

        public CaseRulingDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding case rulings");
                for (int i = 0; i < this.Count; i++)
                {
                    var caseRuling = new CaseRuling
                    {
                        CaseRulingId = Guid.NewGuid(),
                        CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid(),
                        HearingId = HearingDataGenerator.hearings[this.Random.GetRandomNumber(0, HearingDataGenerator.hearings.Count - 1)].HearingId ?? Guid.NewGuid(),
                        ActId = ActDataGenerator.acts[this.Random.GetRandomNumber(0, ActDataGenerator.acts.Count - 1)].ActId ?? Guid.NewGuid(),
                        CaseRulingKindCode = Nomenclatures.caseRulingKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.caseRulingKindCodes.Length - 1)]                         
                    };

                    client.InsertCaseRuling(caseRuling);
                    caseRulings.Add(caseRuling);
                }
            }

            Console.WriteLine("\nCase rulings added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating case rulings");
                for (int i = 0; i < this.Count; i++)
                {
                    var caseRuling = caseRulings[this.Random.GetRandomNumber(0, caseRulings.Count - 1)];

                    caseRuling.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    caseRuling.HearingId = HearingDataGenerator.hearings[this.Random.GetRandomNumber(0, HearingDataGenerator.hearings.Count - 1)].HearingId ?? Guid.NewGuid();
                    caseRuling.ActId = ActDataGenerator.acts[this.Random.GetRandomNumber(0, ActDataGenerator.acts.Count - 1)].ActId ?? Guid.NewGuid();
                    caseRuling.CaseRulingKindCode = Nomenclatures.caseRulingKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.caseRulingKindCodes.Length - 1)];

                    client.UpdateCaseRuling(caseRuling);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nCase rulings updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random case ruling");

                var caseRuling = caseRulings[this.Random.GetRandomNumber(0, caseRulings.Count - 1)];

                client.DeleteCaseRuling(caseRuling.CaseRulingId ?? Guid.NewGuid());
                caseRulings.Remove(caseRuling);
            }
        }
    }
}
