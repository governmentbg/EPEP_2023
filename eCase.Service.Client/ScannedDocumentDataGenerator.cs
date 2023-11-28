using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class ScannedDocumentDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<ScannedDocument> scannedDocuments = new List<ScannedDocument>();

        public ScannedDocumentDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding scanned files");
                for (int i = 0; i < this.Count; i++)
                {
                    try
                    {
                        var scannedDocument = new ScannedDocument
                        {
                            ScannedDocumentId = Guid.NewGuid(),
                            CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid(),
                            Description = this.Random.GetRandomStringWithRandomLength(10, 200)                            
                        };

                        var isPrivate = true;
                        var privateDocsCount = Constants.Docs(isPrivate).Count;

                        var scannedDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                        scannedDocument.ScannedDocumentContent = File.ReadAllBytes(scannedDoc.Item1);
                        scannedDocument.ScannedDocumentMimeType = scannedDoc.Item2;

                        client.InsertScannedDocument(scannedDocument);
                        scannedDocuments.Add(scannedDocument);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Scanned file No:{0} throw exception {1}.", i, ex);
                    }
                }
            }

            Console.WriteLine("\nScanned files added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating scanned documents");
                for (int i = 0; i < this.Count; i++)
                {
                    var scannedDocument = scannedDocuments[this.Random.GetRandomNumber(0, scannedDocuments.Count - 1)];

                    scannedDocument.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    scannedDocument.Description = this.Random.GetRandomStringWithRandomLength(10, 200);

                    var isPrivate = true;
                    var privateDocsCount = Constants.Docs(isPrivate).Count;

                    var scannedDoc = Constants.Docs(isPrivate)[this.Random.GetRandomNumber(0, privateDocsCount - 1)];
                    scannedDocument.ScannedDocumentContent = File.ReadAllBytes(scannedDoc.Item1);
                    scannedDocument.ScannedDocumentMimeType = scannedDoc.Item2;

                    client.UpdateScannedDocument(scannedDocument);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nScanned documents updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random scanned document");

                var scannedDocument = scannedDocuments[this.Random.GetRandomNumber(0, scannedDocuments.Count - 1)];

                client.DeleteScannedDocument(scannedDocument.ScannedDocumentId ?? Guid.NewGuid());
                scannedDocuments.Remove(scannedDocument);
            }
        }
    }
}
