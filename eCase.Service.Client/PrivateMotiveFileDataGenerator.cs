using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class PrivateMotiveFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<PrivateMotiveFile> privateMotiveFiles = new List<PrivateMotiveFile>();

        public PrivateMotiveFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding private motive files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var privateMotiveFile = new PrivateMotiveFile
                        {
                             PrivateMotiveFileId = Guid.NewGuid(),
                             ActId = ActDataGenerator.acts[i].ActId ?? Guid.NewGuid(),
                        };

                        var isPrivate = true;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        privateMotiveFile.PrivateMotiveContent = File.ReadAllBytes(file.Item1);
                        privateMotiveFile.PrivateMotiveMimeType = file.Item2;

                        client.InsertPrivateMotiveFile(privateMotiveFile);
                        privateMotiveFiles.Add(privateMotiveFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Private motive file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nPrivate motive files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating private motive files");
                for (int i = 0; i < this.Count; i++)
                {
                    var privateMotiveFile = privateMotiveFiles[this.Random.GetRandomNumber(0, privateMotiveFiles.Count - 1)];

                    var isPrivate = true;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    privateMotiveFile.PrivateMotiveContent = File.ReadAllBytes(protocolDoc.Item1);
                    privateMotiveFile.PrivateMotiveMimeType = protocolDoc.Item2;

                    client.UpdatePrivateMotiveFile(privateMotiveFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPrivate motive files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random private motive file");

                var privateMotiveFile = privateMotiveFiles[this.Random.GetRandomNumber(0, privateMotiveFiles.Count - 1)];

                client.DeletePrivateMotiveFile(privateMotiveFile.ActId);
                privateMotiveFiles.Remove(privateMotiveFile);
            }
        }
    }
}

