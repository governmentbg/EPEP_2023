using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class AssignmentFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<AssignmentFile> assignmentFiles = new List<AssignmentFile>();

        public AssignmentFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding assignment files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var assignmentFile = new AssignmentFile
                        {
                            AssignmentFileId = Guid.NewGuid(),
                            AssignmentId = AssignmentDataGenerator.assignments[i].AssignmentId ?? Guid.NewGuid(),
                        };

                        var isPrivate = true;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        assignmentFile.ProtocolContent = File.ReadAllBytes(file.Item1);
                        assignmentFile.ProtocolMimeType = file.Item2;

                        client.InsertAssignmentFile(assignmentFile);
                        assignmentFiles.Add(assignmentFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Assignment file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nAssignment files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating assignment files");
                for (int i = 0; i < this.Count; i++)
                {
                    var assignmentFile = assignmentFiles[this.Random.GetRandomNumber(0, assignmentFiles.Count - 1)];

                    var isPrivate = true;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    assignmentFile.ProtocolContent = File.ReadAllBytes(protocolDoc.Item1);
                    assignmentFile.ProtocolMimeType = protocolDoc.Item2;

                    client.UpdateAssignmentFile(assignmentFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nAssignment files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random assignment file");

                var assignmentFile = assignmentFiles[this.Random.GetRandomNumber(0, assignmentFiles.Count - 1)];

                client.DeleteAssignmentFile(assignmentFile.AssignmentId);
                assignmentFiles.Remove(assignmentFile);
            }
        }
    }
}

