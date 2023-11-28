using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class PrivateActFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<PrivateActFile> privateActFiles = new List<PrivateActFile>();

        public PrivateActFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding private act files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var privateActFile = new PrivateActFile
                        {
                             PrivateActFileId = Guid.NewGuid(),
                             ActId = ActDataGenerator.acts[i].ActId ?? Guid.NewGuid(),
                        };

                        var isPrivate = true;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        privateActFile.PrivateActContent = File.ReadAllBytes(file.Item1);
                        privateActFile.PrivateActMimeType = file.Item2;

                        client.InsertPrivateActFile(privateActFile);
                        privateActFiles.Add(privateActFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Private act file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nPrivate act files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating private act files");
                for (int i = 0; i < this.Count; i++)
                {
                    var privateActFile = privateActFiles[this.Random.GetRandomNumber(0, privateActFiles.Count - 1)];

                    var isPrivate = true;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    privateActFile.PrivateActContent = File.ReadAllBytes(protocolDoc.Item1);
                    privateActFile.PrivateActMimeType = protocolDoc.Item2;

                    client.UpdatePrivateActFile(privateActFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPrivate act files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random private act file");

                var privateActFile = privateActFiles[this.Random.GetRandomNumber(0, privateActFiles.Count - 1)];

                client.DeletePrivateActFile(privateActFile.ActId);
                privateActFiles.Remove(privateActFile);
            }
        }
    }
}

