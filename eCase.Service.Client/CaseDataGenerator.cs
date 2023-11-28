using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class CaseDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Case> cases = new List<Case>();

        public CaseDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding cases");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var c = new Case
                        {
                            CaseId = Guid.NewGuid(),
                            IncomingDocumentId = IncomingDocumentDataGenerator.incomingDocs[
                                    this.Random.GetRandomNumber(0, IncomingDocumentDataGenerator.incomingDocs.Count - 1)].IncomingDocumentId ??
                                Guid.NewGuid(),
                            CaseCode = Nomenclatures.caseCodes[this.Random.GetRandomNumber(0, Nomenclatures.caseCodes.Length - 1)],
                            CaseKindCode = Nomenclatures.caseKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.caseKindCodes.Length - 1)],
                            CaseTypeCode = Nomenclatures.caseTypes[this.Random.GetRandomNumber(0, Nomenclatures.caseTypes.Length - 1)],
                            StatisticCode = Nomenclatures.statisticCodes[this.Random.GetRandomNumber(0, Nomenclatures.statisticCodes.Length - 1)],
                            CaseYear = this.Random.GetRandomNumber(1900, 2015),
                            CourtCode = Nomenclatures.courtCodes[this.Random.GetRandomNumber(0, Nomenclatures.courtCodes.Length - 1)],
                            FormationDate = this.Random.GetRandomDate(),
                            Status = this.Random.GetRandomStringWithRandomLength(3, 15),
                            Number = this.Random.GetRandomNumber(1, 100),
                            DepartmentName = this.Random.GetRandomStringWithRandomLength(3, 15),
                            PanelName = this.Random.GetRandomStringWithRandomLength(5, 20),
                            LegalSubject = this.Random.GetRandomStringWithRandomLength(5, 20)                           
                        };

                        client.InsertCase(c);
                        cases.Add(c);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Case No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nCases added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating cases");
                for (int i = 0; i < this.Count; i++)
                {
                    var c = cases[this.Random.GetRandomNumber(0, cases.Count - 1)];

                    c.CaseCode = Nomenclatures.caseCodes[this.Random.GetRandomNumber(0, Nomenclatures.caseCodes.Length - 1)];
                    c.CaseKindCode = Nomenclatures.caseKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.caseKindCodes.Length - 1)];
                    c.CaseTypeCode = Nomenclatures.caseTypes[this.Random.GetRandomNumber(0, Nomenclatures.caseTypes.Length - 1)];
                    c.StatisticCode = Nomenclatures.statisticCodes[this.Random.GetRandomNumber(0, Nomenclatures.statisticCodes.Length - 1)];
                    c.CaseYear = this.Random.GetRandomNumber(1800, 2015);
                    c.CourtCode = Nomenclatures.courtCodes[this.Random.GetRandomNumber(0, Nomenclatures.courtCodes.Length - 1)];
                    c.FormationDate = this.Random.GetRandomDate();
                    c.Status = this.Random.GetRandomStringWithRandomLength(3, 15);
                    c.Number = this.Random.GetRandomNumber(1, 100);
                    c.DepartmentName = this.Random.GetRandomStringWithRandomLength(3, 15);
                    c.PanelName = this.Random.GetRandomStringWithRandomLength(5, 20);
                    c.LegalSubject = this.Random.GetRandomStringWithRandomLength(5, 20);

                    client.UpdateCase(c);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nCases updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random case");

                var c = cases[this.Random.GetRandomNumber(0, cases.Count - 1)];

                client.DeleteCase(c.CaseId ?? Guid.NewGuid());
                cases.Remove(c);
            }
        }
    }
}
