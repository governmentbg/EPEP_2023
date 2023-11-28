using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class AssignmentDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Assignment> assignments = new List<Assignment>();

        public AssignmentDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding assignments");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var assignment = new Assignment
                        {
                            AssignmentId = Guid.NewGuid(),
                            CaseId = CaseDataGenerator.cases[
                                    this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ??
                                Guid.NewGuid(),
                            IncomingDocumentId = IncomingDocumentDataGenerator.incomingDocs[
                                    this.Random.GetRandomNumber(0, IncomingDocumentDataGenerator.incomingDocs.Count - 1)].IncomingDocumentId ??
                                Guid.NewGuid(),
                            Date = this.Random.GetRandomDate(),
                            Assignor = this.Random.GetRandomStringWithRandomLength(5, 15),
                            JudgeName = this.Random.GetRandomStringWithRandomLength(10, 25),                             
                            Type = this.Random.GetRandomStringWithRandomLength(5, 15),
                        };

                        client.InsertAssignment(assignment);
                        assignments.Add(assignment);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Assignment No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nAssignments added");
        }

        public override void Update()
        {
            using (var client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating assignments");
                for (int i = 0; i < this.Count; i++)
                {
                    var assignment = assignments[this.Random.GetRandomNumber(0, assignments.Count - 1)];

                    assignment.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    assignment.Date = this.Random.GetRandomDate();
                    assignment.Type = this.Random.GetRandomStringWithRandomLength(3, 15);
                    assignment.Assignor = this.Random.GetRandomStringWithRandomLength(5, 15);
                    assignment.JudgeName = this.Random.GetRandomStringWithRandomLength(5, 15);
                    assignment.IncomingDocumentId = IncomingDocumentDataGenerator.incomingDocs[
                            this.Random.GetRandomNumber(0, IncomingDocumentDataGenerator.incomingDocs.Count - 1)].IncomingDocumentId ??
                        Guid.NewGuid();

                    client.UpdateAssignment(assignment);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nAssignments updated");
        }

        public override void Delete()
        {
            using (var client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random assignment");

                var assignment = assignments[this.Random.GetRandomNumber(0, assignments.Count - 1)];

                client.DeleteAssignment(assignment.AssignmentId ?? Guid.NewGuid());
                assignments.Remove(assignment);
            }
        }
    }
}

