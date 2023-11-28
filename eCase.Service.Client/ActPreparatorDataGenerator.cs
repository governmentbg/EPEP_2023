using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class ActPreparatorDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<ActPreparator> actPreparators = new List<ActPreparator>();

        public ActPreparatorDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding act preparators");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var actPreparator = new ActPreparator
                        {
                            ActPreparatorId = Guid.NewGuid(),
                            ActId =
                                ActDataGenerator.acts[this.Random.GetRandomNumber(0, ActDataGenerator.acts.Count - 1)]
                                    .ActId ?? Guid.NewGuid(),
                            JudgeName = this.Random.GetRandomStringWithRandomLength(5, 15),
                            Role = this.Random.GetRandomStringWithRandomLength(5, 15),
                            SubstituteFor = this.Random.GetRandomStringWithRandomLength(10, 25),
                            SubstituteReason = this.Random.GetRandomStringWithRandomLength(15, 40)
                        };

                        client.InsertActPreparator(actPreparator);
                        actPreparators.Add(actPreparator);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Act preparator No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nAct preparators added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating act preparators");
                for (int i = 0; i < this.Count; i++)
                {
                    var actPreparator = actPreparators[this.Random.GetRandomNumber(0, actPreparators.Count - 1)];

                    actPreparator.ActId = ActDataGenerator.acts[this.Random.GetRandomNumber(0, ActDataGenerator.acts.Count - 1)].ActId ?? Guid.NewGuid();
                    actPreparator.JudgeName = this.Random.GetRandomStringWithRandomLength(5, 15);
                    actPreparator.Role = this.Random.GetRandomStringWithRandomLength(5, 15);
                    actPreparator.SubstituteFor = this.Random.GetRandomStringWithRandomLength(10, 25);
                    actPreparator.SubstituteReason = this.Random.GetRandomStringWithRandomLength(15, 40);

                    client.UpdateActPreparator(actPreparator);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nAct preparators updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random act preparator");

                var actPreparator = actPreparators[this.Random.GetRandomNumber(0, actPreparators.Count - 1)];

                client.DeleteActPreparator(actPreparator.ActPreparatorId ?? Guid.NewGuid());
                actPreparators.Remove(actPreparator);
            }
        }
    }
}
