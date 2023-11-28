using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class IncomingDocumentFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<IncomingDocumentFile> incomingDocFiles = new List<IncomingDocumentFile>();

        public IncomingDocumentFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding incoming document files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var incomingDocFile = new IncomingDocumentFile
                        {
                             IncomingDocumentFileId = Guid.NewGuid(),
                             IncomingDocumentId = IncomingDocumentDataGenerator.incomingDocs[i].IncomingDocumentId ?? Guid.NewGuid(),
                        };

                        var isPrivate = true;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        incomingDocFile.IncomingDocumentContent = File.ReadAllBytes(file.Item1);
                        incomingDocFile.IncomingDocumentMimeType = file.Item2;

                        client.InsertIncomingDocumentFile(incomingDocFile);
                        incomingDocFiles.Add(incomingDocFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Incoming document file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nIncoming document files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating incoming document files");
                for (int i = 0; i < this.Count; i++)
                {
                    var incomingDocFile = incomingDocFiles[this.Random.GetRandomNumber(0, incomingDocFiles.Count - 1)];

                    var isPrivate = true;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    incomingDocFile.IncomingDocumentContent = File.ReadAllBytes(protocolDoc.Item1);
                    incomingDocFile.IncomingDocumentMimeType = protocolDoc.Item2;

                    client.UpdateIncomingDocumentFile(incomingDocFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nIncoming document files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random incomming document file");

                var incommingDocumentFile = incomingDocFiles[this.Random.GetRandomNumber(0, incomingDocFiles.Count - 1)];

                client.DeleteIncomingDocumentFile(incommingDocumentFile.IncomingDocumentId);
                incomingDocFiles.Remove(incommingDocumentFile);
            }
        }
    }
}

