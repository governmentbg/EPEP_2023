using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class HearingParticipantDataGenerator : DataGenerator, IDataGenerator
    {
        public static List<HearingParticipant> hearingParticipants = new List<HearingParticipant>();

        public HearingParticipantDataGenerator(IRandomDataGenerator randomDataGenerator, int countOfGeneratedObjects)
            : base(randomDataGenerator, countOfGeneratedObjects)
        {
        }

        public override void Insert()
        {
            Console.WriteLine("Adding hearing participants");
            for (int i = 0; i < this.Count; i++)
            {
                try
                {
                    using (var client = new IeCaseServiceClient())
                    {
                        var hearingParticipant = new HearingParticipant
                        {
                            HearingParticipantId = Guid.NewGuid(),
                            HearingId =
                                HearingDataGenerator.hearings[
                                    this.Random.GetRandomNumber(0, HearingDataGenerator.hearings.Count - 1)].HearingId ??
                                Guid.NewGuid(),
                            JudgeName = this.Random.GetRandomStringWithRandomLength(5, 15),
                            Role = this.Random.GetRandomStringWithRandomLength(5, 15),
                            SubstituteFor = this.Random.GetRandomStringWithRandomLength(10, 25),
                            SubstituteReason = this.Random.GetRandomStringWithRandomLength(15, 40)                             
                        };

                        client.InsertHearingParticipant(hearingParticipant);
                        hearingParticipants.Add(hearingParticipant);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hearing participant No:{0} throw exception {1}.", i, ex);
                }
            }

            Console.WriteLine("\nHearing participants added");
        }

        public override void Update()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Updating hearing participants");
                for (int i = 0; i < this.Count; i++)
                {
                    var hearingParticipant = hearingParticipants[this.Random.GetRandomNumber(0, hearingParticipants.Count - 1)];

                    hearingParticipant.HearingId = HearingDataGenerator.hearings[this.Random.GetRandomNumber(0, HearingDataGenerator.hearings.Count - 1)].HearingId ?? Guid.NewGuid();
                    hearingParticipant.JudgeName = this.Random.GetRandomStringWithRandomLength(5, 15);
                    hearingParticipant.Role = this.Random.GetRandomStringWithRandomLength(5, 15);
                    hearingParticipant.SubstituteFor = this.Random.GetRandomStringWithRandomLength(10, 25);
                    hearingParticipant.SubstituteReason = this.Random.GetRandomStringWithRandomLength(15, 40);

                    client.UpdateHearingParticipant(hearingParticipant);

                    if (i % 100 == 0)
                    {
                        Console.Write(".");
                    }
                }
            }

            Console.WriteLine("\nHearing participants updated");
        }

        public override void Delete()
        {
            using (IeCaseServiceClient client = new IeCaseServiceClient())
            {
                Console.WriteLine("Delete random hearing participant");

                var hearingParticipant = hearingParticipants[this.Random.GetRandomNumber(0, hearingParticipants.Count - 1)];

                client.DeleteHearingParticipant(hearingParticipant.HearingParticipantId ?? Guid.NewGuid());
                hearingParticipants.Remove(hearingParticipant);
            }
        }
    }
}

