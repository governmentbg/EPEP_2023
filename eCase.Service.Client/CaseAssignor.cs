using System;
using System.Collections.Generic;

using eCase.Service.Client.eCaseService;

namespace eCase.Service.Client
{
    public class CaseAssignor : ICaseAssignor
    {
        public void AssignLawyerToCases(Tuple<Guid, string> lawyerData, List<Guid> caseIds)
        {
            using (var client = new IeCaseServiceClient())
            {
                var lawyerRegistration = new LawyerRegistration()
                {
                    LawyerRegistrationId = Guid.NewGuid(),
                    LawyerId = lawyerData.Item1,
                    Email = lawyerData.Item2
                };

                client.InsertLawyerRegistration(lawyerRegistration);

                foreach (var caseId in caseIds)
                {
                    var side = new Side
                    {
                        SideId = Guid.NewGuid(),
                        CaseId = caseId,
                        SideInvolvementKindCode = "9052",
                        InsertDate = DateTime.Now,
                        IsActive = false,                        
                    };

                    client.InsertSide(side);

                    var lawyerAssignment = new LawyerAssignment()
                    {
                        LawyerAssignmentId = Guid.NewGuid(),
                        Date = DateTime.Now,
                        SideId = side.SideId,
                        LawyerRegistrationId = lawyerRegistration.LawyerRegistrationId.Value,
                        IsActive = true
                    };

                    client.InsertLawyerAssignment(lawyerAssignment);
                }
            }
        }

        public void AssignPersonToCases(Tuple<string, string> personData, List<Guid> caseIds)
        {
            using (var client = new IeCaseServiceClient())
            {
                var personRegistration = new PersonRegistration()
                {
                    PersonRegistrationId = Guid.NewGuid(),
                    Name = personData.Item1,
                    Email = personData.Item2
                };

                client.InsertPersonRegistration(personRegistration);

                foreach (var caseId in caseIds)
                {
                    var side = new Side
                    {
                        SideId = Guid.NewGuid(),
                        CaseId = caseId,
                        SideInvolvementKindCode = "9052",
                        InsertDate = DateTime.Now,
                        IsActive = false,
                    };

                    client.InsertSide(side);

                    var personAssignment = new PersonAssignment()
                    {
                        PersonAssignmentId = Guid.NewGuid(),
                        PersonRegistrationId = personRegistration.PersonRegistrationId ?? new Guid(),
                        SideId = side.SideId,
                        Date = DateTime.Now,
                        IsActive = true
                    };

                    client.InsertPersonAssignment(personAssignment);
                }
            }
        }
    }
}
