using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class LawyerAssignmentDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<LawyerAssignment> lаwyerAssignments = new List<LawyerAssignment>();

        public LawyerAssignmentDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding lawyer assignments");
                for (int i = 0; i < this.Count; i++)
                {
                    var lawyerAssignment = new LawyerAssignment()
                    {
                        LawyerAssignmentId = Guid.NewGuid(),
                        Date = this.Random.GetRandomDate(),
                        SideId = SideDataGenerator.sides[this.Random.GetRandomNumber(0, SideDataGenerator.sides.Count - 1)].SideId ?? Guid.NewGuid(),
                        LawyerRegistrationId = LawyerRegistrationDataGenerator.lawyerRegistrationIds[this.Random.GetRandomNumber(0, LawyerRegistrationDataGenerator.lawyerRegistrationIds.Count - 1)].Value,
                        IsActive = false                        
                    };

                    client.InsertLawyerAssignment(lawyerAssignment);
                    lаwyerAssignments.Add(lawyerAssignment);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nLawyer assignments added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating lawyer assignments");
                for (int i = 0; i < this.Count; i++)
                {
                    var lawyerAssignment = lаwyerAssignments[this.Random.GetRandomNumber(0, lаwyerAssignments.Count - 1)];

                    lawyerAssignment.Date = this.Random.GetRandomDate();
                    lawyerAssignment.IsActive = i % 2 == 0;

                    client.UpdateLawyerAssignment(lawyerAssignment);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nLawyer assignments updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random lawyer assignment");

                var lawyerAssignment = lаwyerAssignments[this.Random.GetRandomNumber(0, lаwyerAssignments.Count - 1)];

                client.DeleteLawyerAssignment(lawyerAssignment.LawyerAssignmentId ?? Guid.NewGuid());
                lаwyerAssignments.Remove(lawyerAssignment);
            }
        }
    }
}
