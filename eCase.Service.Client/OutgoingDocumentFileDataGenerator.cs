using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class OutgoingDocumentFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<OutgoingDocumentFile> outgoingDocFiles = new List<OutgoingDocumentFile>();

        public OutgoingDocumentFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding outgoing document files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var outgoingDocFile = new OutgoingDocumentFile
                        {
                            OutgoingDocumentFileId = Guid.NewGuid(),
                            OutgoingDocumentId = OutgoingDocumentDataGenerator.outgoingDocs[i].OutgoingDocumentId ?? Guid.NewGuid(),
                        };

                        var isPrivate = true;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        outgoingDocFile.OutgoingDocumentContent = File.ReadAllBytes(file.Item1);
                        outgoingDocFile.OutgoingDocumentMimeType = file.Item2;

                        client.InsertOutgoingDocumentFile(outgoingDocFile);
                        outgoingDocFiles.Add(outgoingDocFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Outgoing document file No:{0} throw exception {1}.", i, ex);
                }

            }

            Console.WriteLine("\nOutgoing document files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating outgoing document files");
                for (int i = 0; i < this.Count; i++)
                {
                    var outgoingDocFile = outgoingDocFiles[this.Random.GetRandomNumber(0, outgoingDocFiles.Count - 1)];

                    var isPrivate = true;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    outgoingDocFile.OutgoingDocumentContent = File.ReadAllBytes(protocolDoc.Item1);
                    outgoingDocFile.OutgoingDocumentMimeType = protocolDoc.Item2;

                    client.UpdateOutgoingDocumentFile(outgoingDocFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nOutgoing document files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random outgoing document file");

                var outgoingDocumentFile = outgoingDocFiles[this.Random.GetRandomNumber(0, outgoingDocFiles.Count - 1)];

                client.DeleteOutgoingDocumentFile(outgoingDocumentFile.OutgoingDocumentId);
                outgoingDocFiles.Remove(outgoingDocumentFile);
            }
        }
    }
}

