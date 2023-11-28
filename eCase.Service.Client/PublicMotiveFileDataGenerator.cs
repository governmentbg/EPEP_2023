using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class PublicMotiveFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<PublicMotiveFile> publicMotiveFiles = new List<PublicMotiveFile>();

        public PublicMotiveFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding public motive files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var publicMotiveFile = new PublicMotiveFile
                        {
                             PublicMotiveFileId = Guid.NewGuid(),
                             ActId = ActDataGenerator.acts[i].ActId ?? Guid.NewGuid(),
                        };

                        var isPrivate = false;
                        var publicDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, publicDocsCount - 1)];
                        publicMotiveFile.PublicMotiveContent = File.ReadAllBytes(file.Item1);
                        publicMotiveFile.PublicMotiveMimeType = file.Item2;

                        client.InsertPublicMotiveFile(publicMotiveFile);
                        publicMotiveFiles.Add(publicMotiveFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Public motive file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nPublic motive files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating public motive files");
                for (int i = 0; i < this.Count; i++)
                {
                    var publicMotiveFile = publicMotiveFiles[this.Random.GetRandomNumber(0, publicMotiveFiles.Count - 1)];

                    var isPrivate = false;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    publicMotiveFile.PublicMotiveContent = File.ReadAllBytes(protocolDoc.Item1);
                    publicMotiveFile.PublicMotiveMimeType = protocolDoc.Item2;

                    client.UpdatePublicMotiveFile(publicMotiveFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPublic motive files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random public motive files");

                var publicMotiveFile = publicMotiveFiles[this.Random.GetRandomNumber(0, publicMotiveFiles.Count - 1)];

                client.DeletePublicMotiveFile(publicMotiveFile.ActId);
                publicMotiveFiles.Remove(publicMotiveFile);
            }
        }
    }
}

