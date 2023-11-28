using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var random = RandomDataGenerator.Instance;

            var listOfGenerators = new List<IDataGenerator>
            {
                new IncomingDocumentDataGenerator(random, 5),
                new IncomingDocumentFileDataGenerator(random, 4),

                new CaseDataGenerator(random,5),
                new ReporterDataGenerator(random, 5),

                new HearingDataGenerator(random, 10),
                new PrivateProtocolFileDataGenerator(random, 3),
                new PublicProtocolFileDataGenerator(random, 3),

                new AssignmentDataGenerator(random, 5),
                new AssignmentFileDataGenerator(random, 4),

                new HearingParticipantDataGenerator(random, 5),

                new ActDataGenerator(random, 20),
                new PrivateActFileDataGenerator(random, 3),
                new PublicActFileDataGenerator(random, 3),
                new PrivateMotiveFileDataGenerator(random, 3),
                new PublicMotiveFileDataGenerator(random, 3),

                new ActPreparatorDataGenerator(random, 5),
                new CaseRulingDataGenerator(random, 5),

                new ScannedDocumentDataGenerator(random, 5),
                new ConnectedCaseDataGenerator(random, 5),
                new SideDataGenerator(random, 5),
                new OutgoingDocumentDataGenerator(random, 5),
                new OutgoingDocumentFileDataGenerator(random, 4),
                new AppealDataGenerator(random, 5),
                new LawyerRegistrationDataGenerator(random, 1),
                 //new PersonRegistrationDataGenerator(random, 1),
                new LawyerAssignmentDataGenerator(random, 1),
                 //new PersonAssignmentDataGenerator(random, 3),
                new SummonDataGenerator(random, 7),
                new SummonFileDataGenerator(random, 5)
            };

            foreach (var generator in listOfGenerators)
            {
                generator.Insert();
            }

            foreach (var generator in listOfGenerators)
            {
                generator.Update();
            }

            var caseId = new Guid("03289877-DDD0-4420-B077-EE55975E5FED");
            var actId = new Guid("2FD9A271-0FC0-4DE5-AF4C-EB4B1B33B288");
            var hearingId = new Guid("1E87833C-B339-4FE6-9445-CA33285F6E5D");

            using (var client = new IeCaseServiceClient())
            {
                //client.GetIncomingDocumentById(new Guid("6430520B-BC06-4A98-9A5E-2E1EF0F1B483"));
                //client.GetOutgoingDocumentById(new Guid("6720327B-C44C-4D81-BD7A-A9047963C9BEF8"));
                //client.GetCaseById(new Guid("8F3E72C2-506C-417A-A8B2-A070A3C9CE4A"));
                //client.GetConnectedCaseById(new Guid("9808AA08-1D41-40C8-AE5F-A036895507B3"));
                //client.GetReporterById(new Guid("504E05F8-AF50-4EFA-BD0F-5098DF0F36C3"));
                //client.GetAssignmentById(new Guid("BFFB6471-071F-429C-8D8E-429D7725F3BF"));
                //client.GetHearingById(new Guid("C6554DF7-FC49-418D-80C4-080491F5B9B0"));
                //client.GetHearingParticipantById(new Guid("B92B3540-1242-47E0-9E9A-DC8029E23551"));
                //client.GetActById(new Guid("1CECC05D-5B06-4F59-8B0B-98E500FD0D71"));
                //client.GetActPreparatorById(new Guid("9B534A82-40FF-4F19-9EBA-BC14F4DDAA55"));
                //client.GetAppealById(new Guid("9873FF8A-08D8-49E4-98EB-4AD3E9E56E75"));
                //client.GetCaseRulingById(new Guid("A4478D7F-B800-4663-B387-14BE180AFB5E"));
                //client.GetSideById(new Guid("72802BB3-0D79-488D-8DF5-F636E204EBB1"));
                //client.GetLawyerAssignmentById(new Guid("B1DBD083-065C-4E27-9B66-7E187FCF7455"));
                //client.GetPersonAssignmentById(new Guid("B20E5507-E008-40F2-896D-DAA2351A6D86"));
                //client.GetLawyerRegistrationById(new Guid("562E494F-8C25-4E30-8D18-EA2D325339F7"));
                //client.GetPersonRegistrationById(new Guid("062A2028-4B8C-4CD2-9B16-690D69A51E93"));
                //client.GetSummonById(new Guid("AA84E6FD-F3AE-4184-9BDD-0B90DD130113"));
                //client.GetScannedDocumentById(new Guid("0EC155CB-A19B-42EE-BBF1-4A45C5A3E02A"));
                //client.GetAssignmentFileById(new Guid("79519B66-B7DC-4CC5-A9CE-AAEFBC4DAA0F"));
                //client.GetPrivateProtocolFileById(new Guid("08D672FC-4F4B-48D2-8BF7-88354E099B09"));
                //client.GetPublicProtocolFileById(new Guid("6E9F8D19-3BC4-477D-A52D-F7E87CE48882"));
                //client.GetIncomingDocumentFileById(new Guid("6720327B-C44C-4D81-BD7A-A9047963C9BE"));
                //client.GetOutgoingDocumentFileById(new Guid("85E380E7-329B-468B-A2D3-93074990D3E9"));
                //client.GetSummonFileById(new Guid("CEC5227C-BB78-4CC8-BCE8-287A9A824476"));
                //client.GetPrivateActFileById(new Guid("1C26C036-DD1F-4EE7-A313-99FA5B6D5CC7"));
                //client.GetPublicActFileById(new Guid("FF4976D2-BC26-42EB-A170-41156E9E715C"));
                //client.GetPrivateMotiveFileById(new Guid("586AF453-3DA9-434D-A5EA-75EC20F9E0CB"));
                //client.GetPublicMotiveFileById(new Guid("80A08146-D722-46ED-A29F-326C767DFF52"));

                //client.GetIncomingDocumentIdentifiersByCaseId(caseId);
                //client.GetOutgoingDocumentIdentifiersByCaseId(caseId);
                //client.GetConnectedCaseIdentifiersByCaseId(caseId);
                //client.GetReporterIdentifiersByCaseId(caseId);
                //client.GetAssignmentIdentifiersByCaseId(caseId);
                //client.GetHearingIdentifiersByCaseId(caseId);
                //client.GetHearingParticipantIdentifiersByHearingId(hearingId);
                //client.GetActIdentifiersByCaseId(caseId);
                //client.GetActPreparatorIdentifiersByActId(actId);
                //client.GetAppealIdentifiersByActId(actId);
                //client.GetCaseRulingIdentifiersByCaseId(caseId);
                //client.GetSideIdentifiersByCaseId(caseId);
                //client.GetLawyerAssignmentIdentifiersBySideId(new Guid("8633D920-FEE3-4679-8079-1F4867B34E35"));
                //client.GetPersonAssignmentIdentifiersBySideId(new Guid("8633D920-FEE3-4679-8079-1F4867B34E35"));
                //client.GetLawyerRegistrationIdentifiersByLawyerId(new Guid("C296E0C0-2EAE-4717-8FE6-015E9AAA2DE8"));
                ////client.GetPersonRegistrationIdentifierByPersonAssignmentId(Guid personassignmentId);
                //client.GetSummonIdentifiersByParentId(caseId, "3");
                //client.GetScannedDocumentIdentifiersByCaseId(caseId);
                //client.GetAssignmentFileIdentifiersByAssignmentId(new Guid("563A62D0-B5C8-4821-9D1B-85925537DCDB"));
                //client.GetPrivateProtocolFileIdentifierByHearingId(hearingId);
                //client.GetPublicProtocolFileIdentifierByHearingId(hearingId);
                //client.GetIncomingDocumentFileIdentifierByIncomingDocumentId(new Guid("5572836C-BB27-4D0E-8863-ED0F0D980D7E"));
                //client.GetOutgoingDocumentFileIdentifierByOutgoingDocumentId(new Guid("498D84A1-CED0-4B12-ADBB-DE660ECF00BE"));
                //client.GetSummonFileIdentifierBySummonId(new Guid("374CD4FF-4C86-4317-8881-B905BB08C7BA"));
                //client.GetPrivateActFileIdentifiersByActId(actId);
                //client.GetPublicActFileIdentifierByActId(actId);
                //client.GetPrivateMotiveFileIdentifierByActId(actId);
                //client.GetPublicMotiveFileIdentifierByActId(actId);
            }

            Console.WriteLine("Completed successfull");
            Console.ReadKey();
        }
    }
}
