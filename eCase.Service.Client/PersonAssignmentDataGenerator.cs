using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class PersonAssignmentDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<PersonAssignment> personAssignments = new List<PersonAssignment>();

        public PersonAssignmentDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding person assignments");
                for (int i = 0; i < this.Count; i++)
                {
                    var personAssignment = new PersonAssignment
                    {
                        PersonAssignmentId = Guid.NewGuid(),
                        PersonRegistrationId = PersonRegistrationDataGenerator.personRegistrationGuids[this.Random.GetRandomNumber(0, PersonRegistrationDataGenerator.personRegistrationGuids.Count - 1)] ?? Guid.NewGuid(),
                        SideId = SideDataGenerator.sides[this.Random.GetRandomNumber(0, SideDataGenerator.sides.Count - 1)].SideId ?? Guid.NewGuid(),
                        Date = this.Random.GetRandomDate(),
                        IsActive = i % 2 == 0
                    };

                    client.InsertPersonAssignment(personAssignment);
                    personAssignments.Add(personAssignment);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPerson assignments added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating person assignments");
                for (int i = 0; i < this.Count; i++)
                {
                    var personAssignment = personAssignments[this.Random.GetRandomNumber(0, personAssignments.Count - 1)];
                    personAssignment.PersonRegistrationId = PersonRegistrationDataGenerator.personRegistrationGuids[this.Random.GetRandomNumber(0, PersonRegistrationDataGenerator.personRegistrationGuids.Count - 1)] ?? Guid.NewGuid();
                    personAssignment.Date = this.Random.GetRandomDate();
                    personAssignment.IsActive = i % 2 == 0;

                    client.UpdatePersonAssignment(personAssignment);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nPerson assignments updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random person assignment");

                var personAssignment = personAssignments[this.Random.GetRandomNumber(0, personAssignments.Count - 1)];

                client.DeletePersonAssignment(personAssignment.PersonAssignmentId ?? Guid.NewGuid());
                personAssignments.Remove(personAssignment);
            }
        }
    }
}
