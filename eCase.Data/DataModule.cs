using Autofac;
using eCase.Common;
using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Data.Nomenclatures;
using eCase.Data.Repositories;
using eCase.Domain.Entities;
using Autofac.Extras.Attributed;
using eCase.Data.Upgrade.Contracts;

namespace eCase.Data
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder
                .Register(c =>
                    c.ResolveKeyed<UnitOfWorkFactory>("factory")(DbKey.BlobStorage) as IUnitOfWork)
                .Keyed<IUnitOfWork>(DbKey.BlobStorage).InstancePerLifetimeScope();

            moduleBuilder
                .Register(c =>
                    c.ResolveKeyed<UnitOfWorkFactory>("factory")(DbKey.Main) as IUnitOfWork)
                .Keyed<IUnitOfWork>(DbKey.Main).As<IUnitOfWork>().InstancePerLifetimeScope();

            moduleBuilder.RegisterType<UnitOfWork>().Keyed<IUnitOfWork>("factory");

            //DisposableTuple
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,,,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,,,,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,,,,,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,,,,,,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,,,,,,,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,,,,,,,,>)).AsSelf();
            moduleBuilder.RegisterGeneric(typeof(DisposableTuple<,,,,,,,,,,>)).AsSelf();

            //AggregateRoot Repositories
            moduleBuilder.RegisterType<ActPreparatorRepository>().As<IActPreparatorRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ActRepository>().As<IActRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<AppealRepository>().As<IAppealRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<AssignmentRepository>().As<IAssignmentRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<AttachedDocumentRepository>().As<IAttachedDocumentRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<CaseRepository>().As<ICaseRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<CaseRulingRepository>().As<ICaseRulingRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ConnectedCaseRepository>().As<IConnectedCaseRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<EntityRepository>().As<IEntityRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<HearingParticipantRepository>().As<IHearingParticipantRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<HearingRepository>().As<IHearingRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<HearingDocumentRepository>().As<IHearingDocumentRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<IncomingDocumentRepository>().As<IIncomingDocumentRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<LawyerAssignmentRepository>().As<ILawyerAssignmentRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<LawyerRegistrationRepository>().As<ILawyerRegistrationRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<LawyerRepository>().As<ILawyerRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<MailRepository>().As<IMailRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<OutgoingDocumentRepository>().As<IOutgoingDocumentRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<PersonAssignmentRepository>().As<IPersonAssignmentRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<PersonRegistrationRepository>().As<IPersonRegistrationRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<PersonRepository>().As<IPersonRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ReporterRepository>().As<IReporterRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ScannedFileRepository>().As<IScannedFileRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<SideRepository>().As<ISideRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<SubjectRepository>().As<ISubjectRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<SummonRepository>().As<ISummonRepository>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();

            //Nom Repositories
            moduleBuilder.RegisterType<ActKindRepository>().As<IEntityCodeNomsRepository<ActKind, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<AppealKindRepository>().As<IEntityCodeNomsRepository<AppealKind, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<CaseCodeRepository>().As<IEntityCodeNomsRepository<CaseCode, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<CaseKindRepository>().As<IEntityCodeNomsRepository<CaseKind, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<CaseRulingKindRepository>().As<IEntityCodeNomsRepository<CaseRulingKind, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<CaseTypeRepository>().As<IEntityCodeNomsRepository<CaseType, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ConnectedCaseTypeRepository>().As<IEntityCodeNomsRepository<ConnectedCaseType, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<CourtRepository>().As<IEntityCodeNomsRepository<Court, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<IncomingDocumentTypeRepository>().As<IEntityCodeNomsRepository<IncomingDocumentType, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<OutgoingDocumentTypeRepository>().As<IEntityCodeNomsRepository<OutgoingDocumentType, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<SideInvolvementKindRepository>().As<IEntityCodeNomsRepository<SideInvolvementKind, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<StatisticCodeRepository>().As<IEntityCodeNomsRepository<StatisticCode, EntityCodeNomVO>>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<SummonTypeRepository>().As<IEntityCodeNomsRepository<SummonType, EntityCodeNomVO>>().InstancePerLifetimeScope();

            moduleBuilder.RegisterType<MailRepository>().As<IMailRepository>().InstancePerLifetimeScope();

            moduleBuilder.RegisterType<BlobStorageRepository>().As<IBlobStorageRepository>().InstancePerLifetimeScope();
            
            //Upgrade repository
            moduleBuilder.RegisterType<UpgradeRepository>().As<IUpgradeRepository>().InstancePerLifetimeScope();
        }
    }
}
