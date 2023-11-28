using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class SideDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Side> sides = new List<Side>();
        public static List<Person> persons = new List<Person>();
        public static List<Entity> entities = new List<Entity>();

        public SideDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding sides");
                for (int i = 0; i < this.Count; i++)
                {
                    try
                    {
                        var side = new Side
                        {
                            SideId = Guid.NewGuid(),
                            CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid(),
                            SideInvolvementKindCode = Nomenclatures.sideInvolvmentKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.sideInvolvmentKindCodes.Length - 1)],
                            InsertDate = this.Random.GetRandomDate(),
                            IsActive = i % 2 == 0,
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

                            side.Person = person;
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

                            side.Entity = entity;
                            entities.Add(entity);
                        }

                        client.InsertSide(side);
                        sides.Add(side);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0} Exception caught.", ex);
                    }
                }
            }

            Console.WriteLine("\nSides added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating sides");
                for (int i = 0; i < this.Count; i++)
                {
                    var side = sides[this.Random.GetRandomNumber(0, sides.Count - 1)];

                    side.CaseId = CaseDataGenerator.cases[this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ?? Guid.NewGuid();
                    side.SideInvolvementKindCode = Nomenclatures.sideInvolvmentKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.sideInvolvmentKindCodes.Length - 1)];
                    side.InsertDate = this.Random.GetRandomDate();
                    side.IsActive = i % 2 == 0;

                    if (side.Person != null)
                    {
                        side.Person = persons[this.Random.GetRandomNumber(0, persons.Count - 1)];

                        side.Person.Firstname = this.Random.GetRandomStringWithRandomLength(3, 15);
                        side.Person.Secondname = this.Random.GetRandomStringWithRandomLength(5, 20);
                        side.Person.Lastname = this.Random.GetRandomStringWithRandomLength(5, 20);
                        side.Person.EGN = this.Random.GetRandomNumber(100000000, 999999999).ToString();
                        side.Person.Address = this.Random.GetRandomStringWithRandomLength(10, 30);

                        side.Entity = null;                       
                    }

                    if (side.Entity != null)
                    {
                        side.Entity = entities[this.Random.GetRandomNumber(0, entities.Count - 1)];

                        side.Entity.Name = this.Random.GetRandomStringWithRandomLength(10, 20);
                        side.Entity.Bulstat = this.Random.GetRandomNumber(100000000, 999999999).ToString();
                        side.Entity.Address = this.Random.GetRandomStringWithRandomLength(10, 30);

                        side.Person = null;                 
                    }

                    client.UpdateSide(side);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nSides updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random side");

                var side = sides[this.Random.GetRandomNumber(0, sides.Count - 1)];

                client.DeleteSide(side.SideId ?? Guid.NewGuid());
                sides.Remove(side);

                if (side.Person != null)
                {
                    persons.Remove(side.Person);
                }

                if (side.Entity != null)
                {
                    entities.Remove(side.Entity);
                }
            }
        }
    }
}