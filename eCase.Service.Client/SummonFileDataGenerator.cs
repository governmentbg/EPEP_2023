using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class SummonFileDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<SummonFile> summonFiles = new List<SummonFile>();

        public SummonFileDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding summon files");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var summonFile = new SummonFile
                        {
                            SummonFileId = Guid.NewGuid(),
                            SummonId = SummonDataGenerator.summons[i].SummonId ?? Guid.NewGuid(),
                        };

                        var isPrivate = true;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        summonFile.Content = File.ReadAllBytes(file.Item1);
                        summonFile.MimeType = file.Item2;

                        client.InsertSummonFile(summonFile);
                        summonFiles.Add(summonFile);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Summon file No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nSummon files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating summon files");
                for (int i = 0; i < this.Count; i++)
                {
                    var summonFile = summonFiles[this.Random.GetRandomNumber(0, summonFiles.Count - 1)];

                    var isPrivate = true;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var file = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    summonFile.Content = File.ReadAllBytes(file.Item1);
                    summonFile.MimeType = file.Item2;

                    client.UpdateSummonFile(summonFile);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nSummon files updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random summon file");

                var summonFile = summonFiles[this.Random.GetRandomNumber(0, summonFiles.Count - 1)];

                client.DeleteSummonFile(summonFile.SummonId);
                summonFiles.Remove(summonFile);
            }
        }
    }
}

