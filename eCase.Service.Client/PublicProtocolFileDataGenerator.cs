using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class PublicProtocolFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<PublicProtocolFile> publicProtocolFiles = new List<PublicProtocolFile>();

        public PublicProtocolFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding public protocol files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var piblicProtocolFile = new PublicProtocolFile
                        {
                             PublicProtocolFileId = Guid.NewGuid(),
                             HearingId = HearingDataGenerator.hearings[i].HearingId ?? Guid.NewGuid(),
                        };

                        var isPrivate = false;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        piblicProtocolFile.ProtocolContent = File.ReadAllBytes(file.Item1);
                        piblicProtocolFile.ProtocolMimeType = file.Item2;

                        client.InsertPublicProtocolFile(piblicProtocolFile);
                        publicProtocolFiles.Add(piblicProtocolFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Public protocol file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nPublic protocol files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating public protocol files");
                for (int i = 0; i < this.Count; i++)
                {
                    var publicProtocolFile = publicProtocolFiles[this.Random.GetRandomNumber(0, publicProtocolFiles.Count - 1)];

                    var isPrivate = false;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var protocolDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    publicProtocolFile.ProtocolContent = File.ReadAllBytes(protocolDoc.Item1);
                    publicProtocolFile.ProtocolMimeType = protocolDoc.Item2;

                    client.UpdatePublicProtocolFile(publicProtocolFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPublic protocol files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random private protocol file");

                var publicProtocolFile = publicProtocolFiles[this.Random.GetRandomNumber(0, publicProtocolFiles.Count - 1)];

                client.DeletePublicProtocolFile(publicProtocolFile.HearingId);
                publicProtocolFiles.Remove(publicProtocolFile);
            }
        }
    }
}