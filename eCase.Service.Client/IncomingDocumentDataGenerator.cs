using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class IncomingDocumentDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<IncomingDocument> incomingDocs = new List<IncomingDocument>();
        public static List<Person> persons = new List<Person>();
        public static List<Entity> entities = new List<Entity>();

        public IncomingDocumentDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding incoming documents");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var incomingDoc = new IncomingDocument
                        {
                            IncomingDocumentId = Guid.NewGuid(),
                            CourtCode = Nomenclatures.courtCodes[this.Random.GetRandomNumber(0, Nomenclatures.courtCodes.Length - 1)],
                            IncomingNumber = this.Random.GetRandomNumber(1, 100),
                            IncomingDate = this.Random.GetRandomDate(),
                            IncomingDocumentTypeCode = Nomenclatures.incomingDocumentTypeCodes[this.Random.GetRandomNumber(0, Nomenclatures.incomingDocumentTypeCodes.Length - 1)],
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

                            incomingDoc.Person = person;
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

                            incomingDoc.Entity = entity;
                            entities.Add(entity);
                        }

                        client.InsertIncomingDocument(incomingDoc);
                        incomingDocs.Add(incomingDoc);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Incoming documents No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nIncoming documents added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating incoming documents");
                for (int i = 0; i < this.Count; i++)
                {
                    var incomingDoc = incomingDocs[this.Random.GetRandomNumber(0, incomingDocs.Count - 1)];

                    incomingDoc.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    incomingDoc.CourtCode = Nomenclatures.courtCodes[this.Random.GetRandomNumber(0, Nomenclatures.courtCodes.Length - 1)];
                    incomingDoc.IncomingNumber = this.Random.GetRandomNumber(1, 100);
                    incomingDoc.IncomingDate = this.Random.GetRandomDate();
                    incomingDoc.IncomingDocumentTypeCode = Nomenclatures.incomingDocumentTypeCodes[this.Random.GetRandomNumber(0, Nomenclatures.incomingDocumentTypeCodes.Length - 1)];

                    if (incomingDoc.Person != null)
                    {
                        incomingDoc.Person = persons[this.Random.GetRandomNumber(0, persons.Count - 1)];

                        incomingDoc.Person.Firstname = this.Random.GetRandomStringWithRandomLength(3, 15);
                        incomingDoc.Person.Secondname = this.Random.GetRandomStringWithRandomLength(5, 20);
                        incomingDoc.Person.Lastname = this.Random.GetRandomStringWithRandomLength(5, 20);
                        incomingDoc.Person.EGN = this.Random.GetRandomNumber(100000000, 999999999).ToString();
                        incomingDoc.Person.Address = this.Random.GetRandomStringWithRandomLength(10, 30);

                        incomingDoc.Entity = null;
                    }

                    if (incomingDoc.Entity != null)
                    {
                        incomingDoc.Entity = entities[this.Random.GetRandomNumber(0, entities.Count - 1)];

                        incomingDoc.Entity.Name = this.Random.GetRandomStringWithRandomLength(10, 20);
                        incomingDoc.Entity.Bulstat = this.Random.GetRandomNumber(100000000, 999999999).ToString();
                        incomingDoc.Entity.Address = this.Random.GetRandomStringWithRandomLength(10, 30);

                        incomingDoc.Person = null;
                    }

                    client.UpdateIncomingDocument(incomingDoc);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nIncoming documents updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random incoming documents");

                var incomingDocument = incomingDocs[this.Random.GetRandomNumber(0, incomingDocs.Count - 1)];

                client.DeleteIncomingDocument(incomingDocument.IncomingDocumentId ?? Guid.NewGuid());
                incomingDocs.Remove(incomingDocument);

                if (incomingDocument.Person != null)
                {
                    persons.Remove(incomingDocument.Person);
                }

                if (incomingDocument.Entity != null)
                {
                    entities.Remove(incomingDocument.Entity);
                }
            }
        }
    }
}

