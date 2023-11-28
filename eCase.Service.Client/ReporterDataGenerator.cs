using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class ReporterDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Reporter> reporters = new List<Reporter>();

        public ReporterDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding reporters");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var reporter = new Reporter
                        {
                            ReporterId = Guid.NewGuid(),
                            CaseId =
                                CaseDataGenerator.cases[
                                    this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ??
                                Guid.NewGuid(),
                            DateAssigned = this.Random.GetRandomDate(),
                            DateReplaced = this.Random.GetRandomDate(),
                            JudgeName = this.Random.GetRandomStringWithRandomLength(5, 15),
                            ReasonReplaced = this.Random.GetRandomStringWithRandomLength(10, 50)                            
                        };

                        client.InsertReporter(reporter);
                        reporters.Add(reporter);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Reporter No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nReporters added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating reporters");
                for (int i = 0; i < this.Count; i++)
                {
                    var reporter = reporters[this.Random.GetRandomNumber(0, reporters.Count - 1)];

                    reporter.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    reporter.DateAssigned = this.Random.GetRandomDate();
                    reporter.DateReplaced = this.Random.GetRandomDate();
                    reporter.JudgeName = this.Random.GetRandomStringWithRandomLength(5, 15);
                    reporter.ReasonReplaced = this.Random.GetRandomStringWithRandomLength(10, 50);

                    client.UpdateReporter(reporter);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nReporters updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random reporter");

                var reporter = reporters[this.Random.GetRandomNumber(0, reporters.Count - 1)];

                client.DeleteReporter(reporter.ReporterId ?? Guid.NewGuid());
                reporters.Remove(reporter);
            }
        }
    }
}
