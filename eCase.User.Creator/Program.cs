using Autofac;
using eCase.Common.NLog;
using eCase.Components.EventHandlers;
using eCase.Data;
using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Data.Repositories;
using eCase.Domain;
using eCase.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCase.User.Creator
{
    class Program
    {
        static void Main(string[] args)
        {
            //var personData = new Tuple<string, string, string>("Илияз", "Илияз", "iliyaz.iliyaz@abbaty.com");
            var lawyerEmails = new List<string>() { "taniapetkova@abv.bg" };
            var list = new List<KeyValuePair<int, int>>();
            list.Add(new KeyValuePair<int, int>(81, 1948));

            //AssignPerson(personData);
            //AssignPersonToCeartainCases(personData, list);
            //AssignLawyers(lawyerEmails);
            //AssignLawyersToCertainCases(lawyerEmails, list);
        }

        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DataModule());
            builder.RegisterModule(new LogModule());
            builder.RegisterModule(new EventHandlersModule());

            return builder.Build();
        }

        public static void AssignPerson(Tuple<string, string, string> personData)
        {
            var container = BuildContainer();

            IUnitOfWork unitOfWork = (IUnitOfWork)container.ResolveKeyed(DbKey.Main, typeof(IUnitOfWork));

            var personRegRepo = container.Resolve<IPersonRegistrationRepository>();
            var userRepo = container.Resolve<IUserRepository>();

            var currentTime = DateTime.Now;

            var user = userRepo.Find(personData.Item3);
            Guid dbUserId;

            if (user == null)
            {
                var personRegistration = new PersonRegistration()
                {
                    PersonRegistrationId = Guid.NewGuid(),
                    Name = personData.Item1 + " " + personData.Item2,
                    Email = personData.Item3,
                    CreateDate = currentTime,
                    ModifyDate = currentTime
                };

                var domainUser = new eCase.Domain.Entities.User
                (
                    personRegistration.PersonRegistrationId,
                    new Guid(eCase.Domain.Entities.UserGroup.Person),
                    personRegistration.Name,
                    personRegistration.Email,
                    currentTime
                );

                using (var transaction = unitOfWork.BeginTransaction())
                {
                    personRegRepo.Add(personRegistration);
                    userRepo.Add(domainUser);

                    unitOfWork.Save();
                    transaction.Commit();
                }

                dbUserId = personRegistration.PersonRegistrationId;
            }
            else
            {
                dbUserId = userRepo.Find(personData.Item3).UserId;
            }

            var caseRepo = container.Resolve<ICaseRepository>();
            var sideRepo = container.Resolve<ISideRepository>();
            var personAssignmentRepo = container.Resolve<IPersonAssignmentRepository>();

            var sideInvolvementKindRepo = container.Resolve<IEntityCodeNomsRepository<SideInvolvementKind, EntityCodeNomVO>>();
            var cases = caseRepo.GetAll().ToList();

            foreach (var c in cases)
            {
                using (var transaction = unitOfWork.BeginTransaction())
                {
                    var side = new Side
                    {
                        SideId = Guid.NewGuid(),
                        CaseId = c.CaseId,
                        SideInvolvementKindId = sideInvolvementKindRepo.GetNomIdByCode("9052"),
                        InsertDate = DateTime.Now,
                        IsActive = false,
                        Person = new Person()
                        {
                            PersonId = Guid.NewGuid(),
                            Firstname = personData.Item1,
                            Lastname = personData.Item2
                        }
                    };

                    sideRepo.Add(side);

                    var personAssignment = new PersonAssignment()
                    {
                        PersonAssignmentId = Guid.NewGuid(),
                        PersonRegistrationId = dbUserId,
                        SideId = side.SideId,
                        Date = DateTime.Now,
                        IsActive = true
                    };

                    personAssignmentRepo.Add(personAssignment);

                    unitOfWork.Save();
                    transaction.Commit();
                }
            }
        }

        public static void AssignPersonToCeartainCases(Tuple<string, string, string> personData, List<KeyValuePair<int, int>> caseData)
        {
            var container = BuildContainer();

            IUnitOfWork unitOfWork = (IUnitOfWork)container.ResolveKeyed(DbKey.Main, typeof(IUnitOfWork));

            var personRegRepo = container.Resolve<IPersonRegistrationRepository>();
            var userRepo = container.Resolve<IUserRepository>();

            var currentTime = DateTime.Now;

            var user = userRepo.Find(personData.Item3);
            Guid dbUserId;

            if (user == null)
            {
                var personRegistration = new PersonRegistration()
                {
                    PersonRegistrationId = Guid.NewGuid(),
                    Name = personData.Item1 + " " + personData.Item2,
                    Email = personData.Item3,
                    CreateDate = currentTime,
                    ModifyDate = currentTime
                };

                var domainUser = new eCase.Domain.Entities.User
                (
                    personRegistration.PersonRegistrationId,
                    new Guid(eCase.Domain.Entities.UserGroup.Person),
                    personRegistration.Name,
                    personRegistration.Email,
                    currentTime
                );

                using (var transaction = unitOfWork.BeginTransaction())
                {
                    personRegRepo.Add(personRegistration);
                    userRepo.Add(domainUser);

                    unitOfWork.Save();
                    transaction.Commit();
                }

                dbUserId = personRegistration.PersonRegistrationId;
            }
            else
            {
                dbUserId = userRepo.Find(personData.Item3).UserId;
            }

            var caseRepo = container.Resolve<ICaseRepository>();
            var sideRepo = container.Resolve<ISideRepository>();
            var personAssignmentRepo = container.Resolve<IPersonAssignmentRepository>();

            var sideInvolvementKindRepo = container.Resolve<IEntityCodeNomsRepository<SideInvolvementKind, EntityCodeNomVO>>();

            var cases = caseRepo.GetAll();

            foreach (var c in cases)
            {
                var foundCase = caseData.Any(cd => cd.Key == c.Number && cd.Value == c.CaseYear);

                if (!foundCase)
                {
                    continue;
                }

                var side = new Side
                {
                    SideId = Guid.NewGuid(),
                    CaseId = c.CaseId,
                    SideInvolvementKindId = sideInvolvementKindRepo.GetNomIdByCode("9052"),
                    InsertDate = DateTime.Now,
                    IsActive = false,
                    Person = new Person()
                    {
                        PersonId = Guid.NewGuid(),
                        Firstname = personData.Item1,
                        Lastname = personData.Item2
                    }
                };

                sideRepo.Add(side);

                var personAssignment = new PersonAssignment()
                {
                    PersonAssignmentId = Guid.NewGuid(),
                    PersonRegistrationId = dbUserId,
                    SideId = side.SideId,
                    Date = DateTime.Now,
                    IsActive = true
                };

                personAssignmentRepo.Add(personAssignment);
            }

            unitOfWork.Save();
        }

        //public static void AssignLawyers(List<string> lawyerEmails)
        //{
        //    var container = BuildContainer();

        //    IUnitOfWork unitOfWork = (IUnitOfWork)container.ResolveKeyed(DbKey.Main, typeof(IUnitOfWork));

        //    var lawyerRegRepo = container.Resolve<ILawyerRegistrationRepository>();
        //    var userRepo = container.Resolve<IUserRepository>();
        //    var lawyerRepo = container.Resolve<ILawyerRepository>();

        //    foreach (var email in lawyerEmails)
        //    {
        //        var dbLawyer = lawyerRepo.GetAllLawyers().Where(l => l.Email == email).FirstOrDefault();
        //        var currentTime = DateTime.Now;

        //        if (dbLawyer == null)
        //        {
        //            dbLawyer = new Lawyer
        //            {
        //                LawyerId = Guid.NewGuid(),
        //                Name = "FakeLawyer",
        //                Email = email,
        //                Number = "0000000000",
        //                CreateDate = currentTime,
        //                ModifyDate = currentTime
        //            };

        //            lawyerRepo.Add(dbLawyer);
        //        }

        //        var user = userRepo.Find(email);
        //        Guid dbUserId;

        //        if (user == null)
        //        {
        //            var lawyerRegistration = new LawyerRegistration()
        //            {
        //                LawyerRegistrationId = Guid.NewGuid(),
        //                LawyerId = dbLawyer.LawyerId,
        //                CreateDate = currentTime,
        //                ModifyDate = currentTime
        //            };

        //            var domainUser = new eCase.Domain.Entities.User
        //            (
        //                lawyerRegistration.LawyerRegistrationId,
        //                new Guid(eCase.Domain.Entities.UserGroup.Lawyer),
        //                dbLawyer.Name,
        //                email,
        //                currentTime
        //            );

        //            using (var transaction = unitOfWork.BeginTransaction())
        //            {
        //                lawyerRegRepo.Add(lawyerRegistration);
        //                userRepo.Add(domainUser);

        //                unitOfWork.Save();
        //                transaction.Commit();
        //            }

        //            dbUserId = lawyerRegistration.LawyerRegistrationId;
        //        }
        //        else
        //        {
        //            dbUserId = userRepo.Find(email).UserId;
        //        }

        //        var caseRepo = container.Resolve<ICaseRepository>();
        //        var sideRepo = container.Resolve<ISideRepository>();
        //        var lawyerAssignmentRepo = container.Resolve<ILawyerAssignmentRepository>();

        //        var sideInvolvementKindRepo = container.Resolve<IEntityCodeNomsRepository<SideInvolvementKind, EntityCodeNomVO>>();
        //        var cases = caseRepo.GetAll().ToList();

        //        foreach (var c in cases)
        //        {
        //            var side = new Side
        //            {
        //                SideId = Guid.NewGuid(),
        //                CaseId = c.CaseId,
        //                SideInvolvementKindId = sideInvolvementKindRepo.GetNomIdByCode("9052"),
        //                InsertDate = DateTime.Now,
        //                IsActive = false,
        //                Person = new Person()
        //                {
        //                    PersonId = Guid.NewGuid(),
        //                    Firstname = "Fake name " + email,
        //                    Lastname = "Fake last name"
        //                }
        //            };

        //            sideRepo.Add(side);

        //            var lawyerAssignment = new LawyerAssignment()
        //            {
        //                LawyerAssignmentId = Guid.NewGuid(),
        //                LawyerId = dbLawyer.LawyerId,
        //                SideId = side.SideId,
        //                Date = DateTime.Now,
        //                IsActive = true,
        //                CreateDate = DateTime.Now,
        //                ModifyDate = DateTime.Now
        //            };

        //            lawyerAssignmentRepo.Add(lawyerAssignment);
        //        }
        //    }

        //    unitOfWork.Save();
        //}

        //public static void AssignLawyersToCertainCases(List<string> lawyerEmails, List<KeyValuePair<int, int>> caseData)
        //{
        //    var container = BuildContainer();

        //    IUnitOfWork unitOfWork = (IUnitOfWork)container.ResolveKeyed(DbKey.Main, typeof(IUnitOfWork));

        //    var lawyerRegRepo = container.Resolve<ILawyerRegistrationRepository>();
        //    var userRepo = container.Resolve<IUserRepository>();
        //    var lawyerRepo = container.Resolve<ILawyerRepository>();

        //    foreach (var email in lawyerEmails)
        //    {
        //        var dbLawyer = lawyerRepo.GetAllLawyers().Where(l => l.Email == email).FirstOrDefault();
        //        var currentTime = DateTime.Now;

        //        if (dbLawyer == null)
        //        {
        //            dbLawyer = new Lawyer
        //            {
        //                LawyerId = Guid.NewGuid(),
        //                Name = "FakeLawyer",
        //                Email = email,
        //                Number = "0000000000",
        //                CreateDate = currentTime,
        //                ModifyDate = currentTime
        //            };

        //            lawyerRepo.Add(dbLawyer);
        //        }

        //        var user = userRepo.Find(email);
        //        Guid dbUserId;

        //        if (user == null)
        //        {
        //            var lawyerRegistration = new LawyerRegistration()
        //            {
        //                LawyerRegistrationId = Guid.NewGuid(),
        //                LawyerId = dbLawyer.LawyerId,
        //                CreateDate = currentTime,
        //                ModifyDate = currentTime
        //            };

        //            var domainUser = new eCase.Domain.Entities.User
        //            (
        //                lawyerRegistration.LawyerRegistrationId,
        //                new Guid(eCase.Domain.Entities.UserGroup.Lawyer),
        //                dbLawyer.Name,
        //                email,
        //                currentTime
        //            );

        //            using (var transaction = unitOfWork.BeginTransaction())
        //            {
        //                lawyerRegRepo.Add(lawyerRegistration);
        //                userRepo.Add(domainUser);

        //                unitOfWork.Save();
        //                transaction.Commit();
        //            }

        //            dbUserId = lawyerRegistration.LawyerRegistrationId;
        //        }
        //        else
        //        {
        //            dbUserId = userRepo.Find(email).UserId;
        //        }

        //        var caseRepo = container.Resolve<ICaseRepository>();
        //        var sideRepo = container.Resolve<ISideRepository>();
        //        var lawyerAssignmentRepo = container.Resolve<ILawyerAssignmentRepository>();

        //        var sideInvolvementKindRepo = container.Resolve<IEntityCodeNomsRepository<SideInvolvementKind, EntityCodeNomVO>>();
        //        var cases = caseRepo.GetAll().ToList();

        //        foreach (var c in cases)
        //        {
        //            var foundCase = caseData.Any(cd => cd.Key == c.Number && cd.Value == c.CaseYear);

        //            if (!foundCase)
        //            {
        //                continue;
        //            }

        //            var side = new Side
        //            {
        //                SideId = Guid.NewGuid(),
        //                CaseId = c.CaseId,
        //                SideInvolvementKindId = sideInvolvementKindRepo.GetNomIdByCode("9052"),
        //                InsertDate = DateTime.Now,
        //                IsActive = false,
        //                Person = new Person()
        //                {
        //                    PersonId = Guid.NewGuid(),
        //                    Firstname = "Fake name " + email,
        //                    Lastname = "Fake last name"
        //                }
        //            };

        //            sideRepo.Add(side);

        //            var lawyerAssignment = new LawyerAssignment()
        //            {
        //                LawyerAssignmentId = Guid.NewGuid(),
        //                LawyerId = dbLawyer.LawyerId,
        //                SideId = side.SideId,
        //                Date = DateTime.Now,
        //                IsActive = true,
        //            };

        //            lawyerAssignment.GetCaseAccess(email, c.Number, c.CaseYear, c.Court.Name);

        //            lawyerAssignmentRepo.Add(lawyerAssignment);
        //        }
        //    }

        //    unitOfWork.Save();
        //}
    }
}
