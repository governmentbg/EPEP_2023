using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class AppealDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<Appeal> appeals = new List<Appeal>();

        public AppealDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Adding appeals");
                for (int i = 0; i < this.Count; i++)
                {
                    try
                    {
                        var appeal = new Appeal
                        {
                            AppealId = Guid.NewGuid(),                            
                            ActId = ActDataGenerator.acts[this.Random.GetRandomNumber(0, ActDataGenerator.acts.Count - 1)].ActId ?? Guid.NewGuid(),
                            AppealKindCode = Nomenclatures.appealKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.appealKindCodes.Length - 1)],
                            SideId = SideDataGenerator.sides[this.Random.GetRandomNumber(0, SideDataGenerator.sides.Count - 1)].SideId ?? Guid.NewGuid(),
                            DateFiled = this.Random.GetRandomDate()
                        };

                        client.InsertAppeal(appeal);
                        appeals.Add(appeal);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0} Exception caught.", ex);
                    }
                }
            }

           Console.WriteLine("\nAppeals added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating appeals");
                for (int i = 0; i < this.Count; i++)
                {
                    var appeal = appeals[this.Random.GetRandomNumber(0, appeals.Count - 1)];

                    appeal.ActId = ActDataGenerator.acts[this.Random.GetRandomNumber(0, ActDataGenerator.acts.Count - 1)].ActId ?? Guid.NewGuid();
                    appeal.AppealKindCode = Nomenclatures.appealKindCodes[this.Random.GetRandomNumber(0, Nomenclatures.appealKindCodes.Length - 1)];
                    appeal.SideId = SideDataGenerator.sides[this.Random.GetRandomNumber(0, SideDataGenerator.sides.Count - 1)].SideId ?? Guid.NewGuid();
                    appeal.DateFiled = this.Random.GetRandomDate();

                    client.UpdateAppeal(appeal);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nAppeals updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random appeal");

                var appeal = appeals[this.Random.GetRandomNumber(0, appeals.Count - 1)];

                client.DeleteAppeal(appeal.AppealId ?? Guid.NewGuid());
                appeals.Remove(appeal);
            }
        }
    }
}
