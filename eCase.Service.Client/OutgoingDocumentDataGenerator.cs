using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class OutgoingDocumentDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<OutgoingDocument> outgoingDocs = new List<OutgoingDocument>();
        public static List<Person> persons = new List<Person>();
        public static List<Entity> entities = new List<Entity>();

        public OutgoingDocumentDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding outgoing documents");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var outgoingDoc = new OutgoingDocument
                        {
                            OutgoingDocumentId = Guid.NewGuid(),
                            CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid(),
                            OutgoingNumber = this.Random.GetRandomNumber(1, 100),
                            OutgoingDate = this.Random.GetRandomDate(),
                            OutgoingDocumentTypeCode = Nomenclatures.outgoingDocumentTypeCodes[this.Random.GetRandomNumber(0, Nomenclatures.outgoingDocumentTypeCodes.Length - 1)]                             
                        };

                        if (i % 2 == 0)
                        {
                            var person = new Person
                            {
                                Firstname = this.Random.GetRandomStringWithRandomLength(3, 15),
                                Secondname = this.Random.GetRandomStringWithRandomLength(5, 20),
                                Lastname = this.Random.GetRandomStringWithRandomLength(5, 20),
                                EGN = this.Random.GetRandomNumber(100000000, 999999999).ToString(),
                                Address = this.Random.GetRandomStringWithRandomLength(10, 30)
                            };

                            outgoingDoc.Person = person;
                            persons.Add(person);
                        }
                        else
                        {
                            var entity = new Entity
                            {
                                Name = this.Random.GetRandomStringWithRandomLength(10, 20),
                                Bulstat = this.Random.GetRandomNumber(100000000, 999999999).ToString(),
                                Address = this.Random.GetRandomStringWithRandomLength(10, 30)
                            };

                            outgoingDoc.Entity = entity;
                            entities.Add(entity);
                        }

                        client.InsertOutgoingDocument(outgoingDoc);
                        outgoingDocs.Add(outgoingDoc);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Outgoing documents No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nOutgoing documents added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating outgoing documents");
                for (int i = 0; i < this.Count; i++)
                {
                    var outgoingDoc = outgoingDocs[this.Random.GetRandomNumber(0, outgoingDocs.Count - 1)];

                    outgoingDoc.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    outgoingDoc.OutgoingNumber = this.Random.GetRandomNumber(1, 100);
                    outgoingDoc.OutgoingDate = this.Random.GetRandomDate();
                    outgoingDoc.OutgoingDocumentTypeCode = Nomenclatures.outgoingDocumentTypeCodes[this.Random.GetRandomNumber(0, Nomenclatures.outgoingDocumentTypeCodes.Length - 1)];

                    if (outgoingDoc.Person != null)
                    {
                        outgoingDoc.Person = persons[this.Random.GetRandomNumber(0, persons.Count - 1)];

                        outgoingDoc.Person.Firstname = this.Random.GetRandomStringWithRandomLength(3, 15);
                        outgoingDoc.Person.Secondname = this.Random.GetRandomStringWithRandomLength(5, 20);
                        outgoingDoc.Person.Lastname = this.Random.GetRandomStringWithRandomLength(5, 20);
                        outgoingDoc.Person.EGN = this.Random.GetRandomNumber(100000000, 999999999).ToString();
                        outgoingDoc.Person.Address = this.Random.GetRandomStringWithRandomLength(10, 30);

                        outgoingDoc.Entity = null;
                    }

                    if (outgoingDoc.Entity != null)
                    {
                        outgoingDoc.Entity = entities[this.Random.GetRandomNumber(0, entities.Count - 1)];

                        outgoingDoc.Entity.Name = this.Random.GetRandomStringWithRandomLength(10, 20);
                        outgoingDoc.Entity.Bulstat = this.Random.GetRandomNumber(100000000, 999999999).ToString();
                        outgoingDoc.Entity.Address = this.Random.GetRandomStringWithRandomLength(10, 30);

                        outgoingDoc.Person = null;
                    }

                    client.UpdateOutgoingDocument(outgoingDoc);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nOutgoing documents updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random outgoing documents");

                var outgoingDocument = outgoingDocs[this.Random.GetRandomNumber(0, outgoingDocs.Count - 1)];

                client.DeleteOutgoingDocument(outgoingDocument.OutgoingDocumentId ?? Guid.NewGuid());
                outgoingDocs.Remove(outgoingDocument);

                if (outgoingDocument.Person != null)
                {
                    persons.Remove(outgoingDocument.Person);
                }

                if (outgoingDocument.Entity != null)
                {
                    entities.Remove(outgoingDocument.Entity);
                }
            }
        }
    }
}