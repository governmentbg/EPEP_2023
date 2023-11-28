using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class PrivateProtocolFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<PrivateProtocolFile> privateProtocolFiles = new List<PrivateProtocolFile>();

        public PrivateProtocolFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding private protocol files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var privateProtocolFile = new PrivateProtocolFile
                        {
                            PrivateProtocolFileId = Guid.NewGuid(),
                            HearingId = HearingDataGenerator.hearings[i].HearingId ?? Guid.NewGuid(),
                        };

                        var isPrivate = true;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        privateProtocolFile.ProtocolContent = File.ReadAllBytes(file.Item1);
                        privateProtocolFile.ProtocolMimeType = file.Item2;

                        client.InsertPrivateProtocolFile(privateProtocolFile);
                        privateProtocolFiles.Add(privateProtocolFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Private protocol file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nPrivate protocol files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating private protocol files");
                for (int i = 0; i < this.Count; i++)
                {
                    var privateProtocolFile = privateProtocolFiles[this.Random.GetRandomNumber(0, privateProtocolFiles.Count - 1)];

                    var isPrivate = true;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    privateProtocolFile.ProtocolContent = File.ReadAllBytes(protocolDoc.Item1);
                    privateProtocolFile.ProtocolMimeType = protocolDoc.Item2;

                    client.UpdatePrivateProtocolFile(privateProtocolFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPrivate protocol files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random private protocol file");

                var privateProtocolFile = privateProtocolFiles[this.Random.GetRandomNumber(0, privateProtocolFiles.Count - 1)];

                client.DeletePrivateProtocolFile(privateProtocolFile.HearingId);
                privateProtocolFiles.Remove(privateProtocolFile);
            }
        }
    }
}

