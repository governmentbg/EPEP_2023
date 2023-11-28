using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class PublicActFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<PublicActFile> publicActFiles = new List<PublicActFile>();

        public PublicActFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding public act files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var publicActFile = new PublicActFile
                        {
                             PublicActFileId = Guid.NewGuid(),
                             ActId = ActDataGenerator.acts[i].ActId ?? Guid.NewGuid(),
                        };

                        var isPrivate = false;
                        var publicDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, publicDocsCount - 1)];
                        publicActFile.PublicActContent = File.ReadAllBytes(file.Item1);
                        publicActFile.PublicActMimeType = file.Item2;

                        client.InsertPublicActFile(publicActFile);
                        publicActFiles.Add(publicActFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Public act file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nPublic act files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating public act files");
                for (int i = 0; i < this.Count; i++)
                {
                    var publicActFile = publicActFiles[this.Random.GetRandomNumber(0, publicActFiles.Count - 1)];

                    var isPrivate = false;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    publicActFile.PublicActContent = File.ReadAllBytes(protocolDoc.Item1);
                    publicActFile.PublicActMimeType = protocolDoc.Item2;

                    client.UpdatePublicActFile(publicActFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPublic act files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random public act file");

                var publicActFile = publicActFiles[this.Random.GetRandomNumber(0, publicActFiles.Count - 1)];

                client.DeletePublicActFile(publicActFile.ActId);
                publicActFiles.Remove(publicActFile);
            }
        }
    }
}

