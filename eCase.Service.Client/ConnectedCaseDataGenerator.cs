using System;
using System.Collections.Generic;
using System.IO;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class ConnectedCaseDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<ConnectedCase> connectedCases = new List<ConnectedCase>();

        public ConnectedCaseDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding connected cases");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var connectedCase = new ConnectedCase
                        {
                            CaseId =
                                CaseDataGenerator.cases[i].CaseId ??
                                Guid.NewGuid(),
                            PredecessorCaseId = CaseDataGenerator.cases[
                                    this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ??
                                Guid.NewGuid(),
                            ConnectedCaseTypeCode = Nomenclatures.connectedCaseTypeCodes[
                                this.Random.GetRandomNumber(0, Nomenclatures.connectedCaseTypeCodes.Length - 1)],
                        };

                        client.InsertConnectedCase(connectedCase);
                        connectedCases.Add(connectedCase);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connected case No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nConnected cases added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating connected cases");
                for (int i = 0; i < this.Count; i++)
                {
                    var connectedCase = connectedCases[this.Random.GetRandomNumber(0, connectedCases.Count - 1)];

                    connectedCase.PredecessorCaseId = CaseDataGenerator.cases[
                            this.Random.GetRandomNumber(0, CaseDataGenerator.cases.Count - 1)].CaseId ??
                        Guid.NewGuid();
                    connectedCase.ConnectedCaseTypeCode = Nomenclatures.connectedCaseTypeCodes[
                        this.Random.GetRandomNumber(0, Nomenclatures.connectedCaseTypeCodes.Length - 1)];

                    client.UpdateConnectedCase(connectedCase);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nConnected cases updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random connected case");

                var connectedCase = connectedCases[this.Random.GetRandomNumber(0, connectedCases.Count - 1)];

                client.DeleteConnectedCase(connectedCase.CaseId);
                connectedCases.Remove(connectedCase);
            }
        }
    }
}
