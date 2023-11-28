using Autofac.Features.Indexed;
using eCase.Common.Db;
using eCase.Common.Helpers;
using eCase.Common.NLog;
using eCase.Data.Core;
using eCase.Data.Core.Nomenclatures;
using eCase.Data.Linq;
using eCase.Data.Repositories;
using eCase.Data.Upgrade;
using eCase.Data.Upgrade.Contracts;
using eCase.Domain.Core;
using eCase.Domain.Entities;
using eCase.Domain.Service;
using eCase.Domain.Service.Entities;
using eCase.Domain.Service.Entities.Upgrade;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text.RegularExpressions;
using FaultCode = eCase.Domain.Service.FaultCode;

namespace eCase.Service
{
    public class eCaseService : IeCaseService
    {
        private const string EMAIL_PATTERN = @"(?=^.{1,64}@)^[a-zA-Z0-9!#$%&amp;'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&amp;'*+/=?^_`{|}~-]+)*@(?=.{1,255}$|.{1,255};)(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])(;(?=.{1,64}@)[a-zA-Z0-9!#$%&amp;'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&amp;'*+/=?^_`{|}~-]+)*@(?=.{1,255}$|.{1,255};)(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9]))*$";

        private IIndex<DbKey, IUnitOfWork> _unitOfWorks;
        private ILogger _logger;
        private IUserRepository _userRepository;
        private ICaseRepository _caseRepository;
        private IReporterRepository _reporterRepository;
        private IAssignmentRepository _assignmentRepository;
        private IHearingRepository _hearingRepository;
        private IHearingParticipantRepository _hearingParticipantRepository;
        private IActRepository _actRepository;
        private IActPreparatorRepository _actPreparatorRepository;
        private IAppealRepository _appealRepository;
        private ICaseRulingRepository _caseRulingRepository;
        private ISideRepository _sideRepository;
        private ILawyerAssignmentRepository _lawyerAssignmentRepository;
        private ILawyerRepository _lawyerRepository;
        private IScannedFileRepository _scannedFileRepository;
        private ISummonRepository _summonRepository;
        private IPersonAssignmentRepository _personAssignmentRepository;
        private IPersonRepository _personRepository;
        private IEntityRepository _entityRepository;
        private IPersonRegistrationRepository _personRegistrationRepository;
        private ILawyerRegistrationRepository _lawyerRegistrationRepository;
        private IConnectedCaseRepository _connectedCaseRepository;
        private ISubjectRepository _subjectRepository;
        private IIncomingDocumentRepository _incomingDocumentRepository;
        private IOutgoingDocumentRepository _outgoingDocumentRepository;
        private IBlobStorageRepository _blobStorageRepository;

        private IAttachedDocumentRepository _attachedDocumentRepository;
        private IHearingDocumentRepository _hearingDocumentRepository;
        private IUpgradeRepository upgradeRepository;

        IEntityCodeNomsRepository<ActKind, EntityCodeNomVO> _actKindRepository;
        IEntityCodeNomsRepository<AppealKind, EntityCodeNomVO> _appealKindRepository;
        IEntityCodeNomsRepository<CaseCode, EntityCodeNomVO> _caseCodeRepository;
        IEntityCodeNomsRepository<CaseKind, EntityCodeNomVO> _caseKindRepository;
        IEntityCodeNomsRepository<CaseRulingKind, EntityCodeNomVO> _caseRulingKindRepository;
        IEntityCodeNomsRepository<CaseType, EntityCodeNomVO> _caseTypeRepository;
        IEntityCodeNomsRepository<ConnectedCaseType, EntityCodeNomVO> _connectedCaseTypeRepository;
        IEntityCodeNomsRepository<Court, EntityCodeNomVO> _courtRepository;
        IEntityCodeNomsRepository<IncomingDocumentType, EntityCodeNomVO> _incomingDocumentTypeRepository;
        IEntityCodeNomsRepository<OutgoingDocumentType, EntityCodeNomVO> _outgoingDocumentTypeRepository;
        IEntityCodeNomsRepository<SideInvolvementKind, EntityCodeNomVO> _sideInvolvementKindRepository;
        IEntityCodeNomsRepository<StatisticCode, EntityCodeNomVO> _statisticCodeRepository;
        IEntityCodeNomsRepository<SummonType, EntityCodeNomVO> _summonTypeRepository;


        private bool upgradedEpep = false;
        private int upgradeUserFetch = 2000;
        private string upgradedEpepObsoleteMethod = "Методът не се поддържа. Използвайте методите за UserRegistration/UserAssignment";
        private static Sequence BlobContentSequence = new Sequence("BlobContentSequence", "DbContextMain");

        public eCaseService
            (
                IIndex<DbKey, IUnitOfWork> unitOfWorks,
                ILogger logger,
                IUserRepository userRepository,
                ICaseRepository caseRepository,
                IReporterRepository reporterRepository,
                IAssignmentRepository assignmentRepository,
                IHearingRepository hearingRepository,
                IHearingParticipantRepository hearingParticipantRepository,
                IActRepository actRepository,
                IActPreparatorRepository actPreparatorRepository,
                IAppealRepository appealRepository,
                ICaseRulingRepository caseRulingRepository,
                ISideRepository sideRepository,
                ILawyerAssignmentRepository lawyerAssignmentRepository,
                ILawyerRepository lawyerRepository,
                IScannedFileRepository scannedFileRepository,
                ISummonRepository summonRepository,
                IPersonAssignmentRepository personAssignmentRepository,
                IPersonRepository personRepository,
                IEntityRepository entityRepository,
                IPersonRegistrationRepository personRegistrationRepository,
                ILawyerRegistrationRepository lawyerRegistrationRepository,
                IConnectedCaseRepository connectedCaseRepository,
                ISubjectRepository subjectRepository,
                IIncomingDocumentRepository incomingDocumentRepository,
                IOutgoingDocumentRepository outgoingDocumentRepository,
                IBlobStorageRepository blobStorageRepository,
                IAttachedDocumentRepository attachedDocumentRepository,
                IHearingDocumentRepository hearingDocumentRepository,

                IEntityCodeNomsRepository<ActKind, EntityCodeNomVO> actKindRepository,
                IEntityCodeNomsRepository<AppealKind, EntityCodeNomVO> appealKindRepository,
                IEntityCodeNomsRepository<CaseCode, EntityCodeNomVO> caseCodeRepository,
                IEntityCodeNomsRepository<CaseKind, EntityCodeNomVO> caseKindRepository,
                IEntityCodeNomsRepository<CaseRulingKind, EntityCodeNomVO> caseRulingKindRepository,
                IEntityCodeNomsRepository<CaseType, EntityCodeNomVO> caseTypeRepository,
                IEntityCodeNomsRepository<ConnectedCaseType, EntityCodeNomVO> connectedCaseTypeRepository,
                IEntityCodeNomsRepository<Court, EntityCodeNomVO> courtRepository,
                IEntityCodeNomsRepository<IncomingDocumentType, EntityCodeNomVO> incomingDocumentTypeRepository,
                IEntityCodeNomsRepository<OutgoingDocumentType, EntityCodeNomVO> outgoingDocumentTypeRepository,
                IEntityCodeNomsRepository<SideInvolvementKind, EntityCodeNomVO> sideInvolvementKindRepository,
                IEntityCodeNomsRepository<StatisticCode, EntityCodeNomVO> statisticCodeRepository,
                IEntityCodeNomsRepository<SummonType, EntityCodeNomVO> summonTypeRepository,


                IUpgradeRepository upgradeRepository
            )
        {
            _unitOfWorks = unitOfWorks;
            _logger = logger;
            _userRepository = userRepository;
            _caseRepository = caseRepository;
            _reporterRepository = reporterRepository;
            _assignmentRepository = assignmentRepository;
            _hearingRepository = hearingRepository;
            _hearingParticipantRepository = hearingParticipantRepository;
            _actRepository = actRepository;
            _actPreparatorRepository = actPreparatorRepository;
            _appealRepository = appealRepository;
            _caseRulingRepository = caseRulingRepository;
            _sideRepository = sideRepository;
            _lawyerAssignmentRepository = lawyerAssignmentRepository;
            _lawyerRepository = lawyerRepository;
            _scannedFileRepository = scannedFileRepository;
            _summonRepository = summonRepository;
            _personAssignmentRepository = personAssignmentRepository;
            _personRepository = personRepository;
            _entityRepository = entityRepository;
            _personRegistrationRepository = personRegistrationRepository;
            _lawyerRegistrationRepository = lawyerRegistrationRepository;
            _connectedCaseRepository = connectedCaseRepository;
            _subjectRepository = subjectRepository;
            _incomingDocumentRepository = incomingDocumentRepository;
            _outgoingDocumentRepository = outgoingDocumentRepository;
            _blobStorageRepository = blobStorageRepository;
            _actKindRepository = actKindRepository;
            _appealKindRepository = appealKindRepository;
            _caseCodeRepository = caseCodeRepository;
            _caseKindRepository = caseKindRepository;
            _caseRulingKindRepository = caseRulingKindRepository;
            _caseTypeRepository = caseTypeRepository;
            _connectedCaseTypeRepository = connectedCaseTypeRepository;
            _courtRepository = courtRepository;
            _incomingDocumentTypeRepository = incomingDocumentTypeRepository;
            _outgoingDocumentTypeRepository = outgoingDocumentTypeRepository;
            _sideInvolvementKindRepository = sideInvolvementKindRepository;
            _statisticCodeRepository = statisticCodeRepository;
            _summonTypeRepository = summonTypeRepository;

            _attachedDocumentRepository = attachedDocumentRepository;
            _hearingDocumentRepository = hearingDocumentRepository;

            this.upgradeRepository = upgradeRepository;

            upgradedEpep = ConfigurationManager.AppSettings["EPEP2023"] == "true";
            if (ConfigurationManager.AppSettings["UserRegFetchCount"] != null)
            {
                upgradeUserFetch = int.Parse(ConfigurationManager.AppSettings["UserRegFetchCount"]);
            }
        }

        #region IncomingDocument

        public Guid? InsertIncomingDocument(Domain.Service.Entities.IncomingDocument incomingDocument)
        {
            ValidateIncomingDocument(incomingDocument, false);

            try
            {
                var domainIncomingDocument = new Domain.Entities.IncomingDocument()
                {
                    Gid = incomingDocument.IncomingDocumentId.HasValue ? incomingDocument.IncomingDocumentId.Value : Guid.NewGuid(),
                    CaseId = incomingDocument.CaseId.HasValue ? _caseRepository.FindByGid(incomingDocument.CaseId.Value).CaseId : (long?)null,
                    CourtId = _courtRepository.GetNomIdByCode(incomingDocument.CourtCode),
                    IncomingNumber = incomingDocument.IncomingNumber,
                    IncomingYear = incomingDocument.IncomingDate.Year,
                    IncomingDate = incomingDocument.IncomingDate,
                    IncomingDocumentTypeId = _incomingDocumentTypeRepository.GetNomIdByCode(incomingDocument.IncomingDocumentTypeCode)
                };



                #region LoadSubject

                var domainSubject = new Subject();
                var domainPerson = new Domain.Entities.Person();
                var domainEntity = new Domain.Entities.Entity();

                var hasPerson = incomingDocument.Person != null;
                var hasEntity = incomingDocument.Entity != null;

                if (hasPerson)
                {
                    domainPerson.Gid = Guid.NewGuid();
                    domainPerson.Firstname = incomingDocument.Person.Firstname;
                    domainPerson.Secondname = incomingDocument.Person.Secondname;
                    domainPerson.Lastname = incomingDocument.Person.Lastname;
                    domainPerson.EGN = incomingDocument.Person.EGN;
                    domainPerson.Address = incomingDocument.Person.Address;

                    domainSubject.Gid = domainPerson.Gid;
                    domainSubject.SubjectTypeId = (int)SubjectType.Person;
                    domainSubject.Name = string.Join(" ", domainPerson.Firstname, domainPerson.Secondname, domainPerson.Lastname);
                    domainSubject.Uin = domainPerson.EGN;
                }
                else if (hasEntity)
                {
                    domainEntity.Gid = Guid.NewGuid();
                    domainEntity.Name = incomingDocument.Entity.Name;
                    domainEntity.Bulstat = incomingDocument.Entity.Bulstat;
                    domainEntity.Address = incomingDocument.Entity.Address;

                    domainSubject.Gid = domainEntity.Gid;
                    domainSubject.SubjectTypeId = (int)SubjectType.Entity;
                    domainSubject.Name = domainEntity.Name;
                    domainSubject.Uin = domainEntity.Bulstat;
                }

                if (hasPerson || hasEntity)
                {
                    _subjectRepository.Add(domainSubject);
                    _unitOfWorks[DbKey.Main].Save();

                    domainIncomingDocument.SubjectId = domainSubject.SubjectId;
                }

                #endregion

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    if (incomingDocument.ElectronicDocumentId.HasValue)
                    {
                        var domainElectronicDocument = upgradeRepository.FindByGid<Domain.Entities.Upgrade.ElectronicDocument>(incomingDocument.ElectronicDocumentId.Value);
                        domainElectronicDocument.CourtDocumentNumber = incomingDocument.IncomingNumber.ToString();
                        domainElectronicDocument.CourtDocumentDate = incomingDocument.IncomingDate;
                        domainIncomingDocument.ElectronicDocumentId = domainElectronicDocument.Id;

                        var docInfo = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.ElectronicDocument>()
                                                .Where(x => x.Id == domainElectronicDocument.Id)
                                                .Select(x => new
                                                {
                                                    Email = x.CreateUser.Email,
                                                    DocType = x.ElectronicDocumentType.Name,
                                                    CourtName = x.Court.Name
                                                }).FirstOrDefault();

                        Domain.Emails.Email email = new Domain.Emails.Email(
                           docInfo.Email,
                           Components.EmailTemplates.DocumentRegisteredMessage,
                           JObject.FromObject(
                               new
                               {
                                   courtName = docInfo.CourtName,
                                   applyType = docInfo.DocType,
                                   applyNumber = domainElectronicDocument.NumberApply,
                                   applyDate = domainElectronicDocument.DateApply.Value.ToString("dd.MM.yyyy"),
                                   courtNumber = domainElectronicDocument.CourtDocumentNumber,
                                   courtDate = domainElectronicDocument.CourtDocumentDate.Value.ToString("dd.MM.yyyy"),
                                   courtType = _incomingDocumentTypeRepository.GetNom(domainIncomingDocument.IncomingDocumentTypeId).Name
                               }));

                        upgradeRepository.Add(email);
                    }

                    _incomingDocumentRepository.Add(domainIncomingDocument);

                    if (hasPerson)
                    {
                        domainPerson.SubjectId = domainSubject.SubjectId;
                        _personRepository.Add(domainPerson);
                    }

                    if (hasEntity)
                    {
                        domainEntity.SubjectId = domainSubject.SubjectId;
                        _entityRepository.Add(domainEntity);
                    }



                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainIncomingDocument.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateIncomingDocument(Domain.Service.Entities.IncomingDocument incomingDocument)
        {
            ValidateIncomingDocument(incomingDocument, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainIncomingDocument = _incomingDocumentRepository.FindByGid(incomingDocument.IncomingDocumentId.Value);

                    #region Load IncomingDocument

                    var domainSubject = new Subject();
                    var domainPerson = new Domain.Entities.Person();
                    var domainEntity = new Domain.Entities.Entity();

                    var currentTime = DateTime.Now;

                    #region LoadSubject

                    var hasPerson = incomingDocument.Person != null;
                    var hasEntity = incomingDocument.Entity != null;

                    var hasDomainPerson = false;
                    var hasDomainEntity = false;

                    if (hasPerson)
                    {
                        if (domainIncomingDocument.SubjectId.HasValue)
                        {
                            hasDomainPerson = _personRepository.Find(domainIncomingDocument.SubjectId.Value) != null;
                        }

                        if (hasDomainPerson)
                        {
                            domainPerson = _personRepository.Find(domainIncomingDocument.SubjectId.Value);
                            domainSubject = _subjectRepository.Find(domainPerson.SubjectId);
                        }
                        else
                        {
                            domainPerson.Gid = Guid.NewGuid();

                            domainSubject.Gid = domainPerson.Gid;
                            domainSubject.SubjectTypeId = (int)SubjectType.Person;
                        }

                        domainPerson.Firstname = incomingDocument.Person.Firstname;
                        domainPerson.Secondname = incomingDocument.Person.Secondname;
                        domainPerson.Lastname = incomingDocument.Person.Lastname;
                        domainPerson.EGN = incomingDocument.Person.EGN;
                        domainPerson.Address = incomingDocument.Person.Address;

                        domainPerson.ModifyDate = currentTime;

                        domainSubject.Name = string.Join(" ", domainPerson.Firstname, domainPerson.Secondname, domainPerson.Lastname);
                        domainSubject.Uin = domainPerson.EGN;
                    }

                    if (hasEntity)
                    {
                        if (domainIncomingDocument.SubjectId.HasValue)
                        {
                            hasDomainEntity = _entityRepository.Find(domainIncomingDocument.SubjectId.Value) != null;
                        }

                        if (hasDomainEntity)
                        {
                            domainEntity = _entityRepository.Find(domainIncomingDocument.SubjectId.Value);
                            domainSubject = _subjectRepository.Find(domainEntity.SubjectId);
                        }
                        else
                        {
                            domainEntity.Gid = Guid.NewGuid();

                            domainSubject.Gid = domainEntity.Gid;
                            domainSubject.SubjectTypeId = (int)SubjectType.Entity;
                        }

                        domainEntity.Name = incomingDocument.Entity.Name;
                        domainEntity.Bulstat = incomingDocument.Entity.Bulstat;
                        domainEntity.Address = incomingDocument.Entity.Address;

                        domainEntity.ModifyDate = currentTime;

                        domainSubject.Name = domainEntity.Name;
                        domainSubject.Uin = domainEntity.Bulstat;
                    }

                    domainSubject.ModifyDate = currentTime;

                    #endregion

                    if (!hasDomainPerson && !hasDomainEntity)
                    {
                        _subjectRepository.Add(domainSubject);
                        _unitOfWorks[DbKey.Main].Save();
                    }

                    domainIncomingDocument.CaseId = incomingDocument.CaseId.HasValue ? _caseRepository.FindByGid(incomingDocument.CaseId.Value).CaseId : (long?)null;
                    domainIncomingDocument.SubjectId = domainSubject.SubjectId;
                    domainIncomingDocument.CourtId = _courtRepository.GetNomIdByCode(incomingDocument.CourtCode);
                    domainIncomingDocument.IncomingNumber = incomingDocument.IncomingNumber;
                    domainIncomingDocument.IncomingYear = incomingDocument.IncomingDate.Year;
                    domainIncomingDocument.IncomingDate = incomingDocument.IncomingDate;
                    domainIncomingDocument.IncomingDocumentTypeId = _incomingDocumentTypeRepository.GetNomIdByCode(incomingDocument.IncomingDocumentTypeCode);

                    domainIncomingDocument.ModifyDate = DateTime.Now;

                    #endregion

                    if (hasPerson && !hasDomainPerson)
                    {
                        domainPerson.SubjectId = domainSubject.SubjectId;
                        _personRepository.Add(domainPerson);
                    }

                    if (hasEntity && !hasDomainEntity)
                    {
                        domainEntity.SubjectId = domainSubject.SubjectId;
                        _entityRepository.Add(domainEntity);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainIncomingDocument.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteIncomingDocument(Guid incomingDocumentId)
        {
            var domainIncomingDocument = _incomingDocumentRepository.FindByGid(incomingDocumentId);
            if (domainIncomingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(incomingDocumentId)));

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    if (domainIncomingDocument.SubjectId.HasValue)
                    {
                        var domainSubject = _subjectRepository.Find(domainIncomingDocument.SubjectId.Value);

                        if (domainSubject.SubjectTypeId == 1)
                        {
                            var domainPerson = _personRepository.Find(domainSubject.SubjectId);
                            _personRepository.Remove(domainPerson);
                        }

                        if (domainSubject.SubjectTypeId == 2)
                        {
                            var domainEntity = _entityRepository.Find(domainSubject.SubjectId);
                            _entityRepository.Remove(domainEntity);
                        }

                        _subjectRepository.Remove(domainSubject);
                    }

                    _incomingDocumentRepository.Remove(domainIncomingDocument);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.IncomingDocument GetIncomingDocumentById(Guid incomingDocumentId)
        {
            if (incomingDocumentId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(incomingDocumentId)));

            var domainIncomingDocument = _incomingDocumentRepository.GetIncomingDocument(incomingDocumentId);
            if (domainIncomingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(incomingDocumentId)));

            var incomingDocument = new Domain.Service.Entities.IncomingDocument()
            {
                IncomingDocumentId = domainIncomingDocument.Gid,
                CaseId = domainIncomingDocument.CaseId.HasValue ? _caseRepository.Find(domainIncomingDocument.CaseId.Value).Gid : (Guid?)null,
                CourtCode = _courtRepository.GetNom(domainIncomingDocument.CourtId).Code,
                IncomingNumber = domainIncomingDocument.IncomingNumber,
                IncomingDate = domainIncomingDocument.IncomingDate,
                IncomingDocumentTypeCode = _incomingDocumentTypeRepository.GetNom(domainIncomingDocument.IncomingDocumentTypeId).Code
            };

            var hasPerson = domainIncomingDocument.Subject.Person != null;
            var hasEntity = domainIncomingDocument.Subject.Entity != null;

            if (hasPerson)
            {
                incomingDocument.Person = new Domain.Service.Entities.Person()
                {
                    Firstname = domainIncomingDocument.Subject.Person.Firstname,
                    Secondname = domainIncomingDocument.Subject.Person.Secondname,
                    Lastname = domainIncomingDocument.Subject.Person.Lastname,
                    EGN = domainIncomingDocument.Subject.Person.EGN,
                    Address = domainIncomingDocument.Subject.Person.Address,
                };
            }
            else if (hasEntity)
            {
                incomingDocument.Entity = new Domain.Service.Entities.Entity()
                {
                    Name = domainIncomingDocument.Subject.Entity.Name,
                    Bulstat = domainIncomingDocument.Subject.Entity.Bulstat,
                    Address = domainIncomingDocument.Subject.Entity.Address
                };
            }

            return incomingDocument;
        }

        public List<Guid> GetIncomingDocumentIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.IncomingDocuments)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.IncomingDocuments.Select(i => i.Gid).ToList();
        }

        private void ValidateIncomingDocument(Domain.Service.Entities.IncomingDocument incomingDocument, bool isUpdate)
        {
            this.ValidateId(incomingDocument.IncomingDocumentId, _incomingDocumentRepository, isUpdate);

            if (incomingDocument.CaseId.HasValue)
            {
                if (incomingDocument.CaseId.Value == default(Guid))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "IncomingDocument.CaseId"));
                if (_caseRepository.FindByGid(incomingDocument.CaseId.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "IncomingDocument.CaseId"));
            }

            if (string.IsNullOrWhiteSpace(incomingDocument.CourtCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "IncomingDocument.CourtCode"));
            if (!_courtRepository.HasCode(incomingDocument.CourtCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "IncomingDocument.CourtCode"));
            if (incomingDocument.IncomingNumber == default(int))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "IncomingDocument.IncomingNumber"));
            if (incomingDocument.IncomingDate == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "IncomingDocument.IncomingDate"));
            if (string.IsNullOrWhiteSpace(incomingDocument.IncomingDocumentTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "IncomingDocument.IncomingDocumentTypeCode"));
            if (!_incomingDocumentTypeRepository.HasCode(incomingDocument.IncomingDocumentTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "IncomingDocument.IncomingDocumentTypeCode"));
            if (incomingDocument.Person != null)
                ValidatePerson(incomingDocument.Person);
            if (incomingDocument.Entity != null)
                ValidateEntity(incomingDocument.Entity);

            if (incomingDocument.ElectronicDocumentId.HasValue)
            {
                if (upgradeRepository.FindByGid<Domain.Entities.Upgrade.ElectronicDocument>(incomingDocument.ElectronicDocumentId.Value) == null)
                {
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "IncomingDocument.ElectronicDocumentId"));
                }
            }
        }

        #endregion

        #region IncomingDocumentFile

        public Guid? InsertIncomingDocumentFile(IncomingDocumentFile incomingDocumentFile)
        {
            ValidateIncomingDocumentFile(incomingDocumentFile, false);

            try
            {
                var domainIncomingDocument = _incomingDocumentRepository.FindByGid(incomingDocumentFile.IncomingDocumentId);
                return this.UploadFile(incomingDocumentFile.IncomingDocumentFileId, incomingDocumentFile.IncomingDocumentContent, incomingDocumentFile.IncomingDocumentMimeType, FileType.Incoming, domainIncomingDocument.IncomingDate, domainIncomingDocument, value => domainIncomingDocument.BlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateIncomingDocumentFile(IncomingDocumentFile incomingDocumentFile)
        {
            ValidateIncomingDocumentFile(incomingDocumentFile, true);

            try
            {
                var domainIncomingDocument = _incomingDocumentRepository.FindByGid(incomingDocumentFile.IncomingDocumentId);
                return this.UploadFile(incomingDocumentFile.IncomingDocumentFileId, incomingDocumentFile.IncomingDocumentContent, incomingDocumentFile.IncomingDocumentMimeType, FileType.Incoming, domainIncomingDocument.IncomingDate, domainIncomingDocument, value => domainIncomingDocument.BlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteIncomingDocumentFile(Guid incomingDocumentId)
        {
            var domainIncomingDocument = _incomingDocumentRepository.FindByGid(incomingDocumentId);
            if (domainIncomingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(incomingDocumentId)));

            return this.DeleteFile(domainIncomingDocument, domainIncomingDocument.BlobKey, value => domainIncomingDocument.BlobKey = value);
        }

        public IncomingDocumentFile GetIncomingDocumentFileById(Guid incomingDocumentFileId)
        {
            if (incomingDocumentFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(incomingDocumentFileId)));

            var domainIncomingDocument = _incomingDocumentRepository.SetWithoutIncludes()
                .Include(a => a.Blob)
                .FirstOrDefault(a => a.BlobKey == incomingDocumentFileId);
            if (domainIncomingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(incomingDocumentFileId)));

            var incomingDocumentFile = new IncomingDocumentFile()
            {
                IncomingDocumentFileId = domainIncomingDocument.BlobKey,
                IncomingDocumentId = domainIncomingDocument.Gid,
                IncomingDocumentMimeType = _blobStorageRepository.GetMimeType(domainIncomingDocument.Blob.FileName),
                IncomingDocumentContent = _blobStorageRepository.GetFileContent(domainIncomingDocument.BlobKey.Value),
            };

            return incomingDocumentFile;
        }

        public Guid? GetIncomingDocumentFileIdentifierByIncomingDocumentId(Guid incomingDocumentId)
        {
            var domainIncomingDocument = _incomingDocumentRepository.FindByGid(incomingDocumentId);
            if (domainIncomingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(incomingDocumentId)));

            return domainIncomingDocument.BlobKey;
        }

        private void ValidateIncomingDocumentFile(IncomingDocumentFile incomingDocumentFile, bool isUpdate)
        {
            var domainIncomingDocument = _incomingDocumentRepository.FindByGid(incomingDocumentFile.IncomingDocumentId);

            this.ValidateFile(incomingDocumentFile.IncomingDocumentFileId, incomingDocumentFile.IncomingDocumentId, incomingDocumentFile.IncomingDocumentContent, incomingDocumentFile.IncomingDocumentMimeType, domainIncomingDocument?.BlobKey, domainIncomingDocument, isUpdate);
        }

        #endregion

        #region Case

        public Guid? InsertCase(Domain.Service.Entities.Case c)
        {
            ValidateCase(c, false);

            try
            {
                var domainCase = new Domain.Entities.Case()
                {
                    Gid = c.CaseId.HasValue ? c.CaseId.Value : Guid.NewGuid(),
                    IncomingDocumentId = c.IncomingDocumentId.HasValue ? _incomingDocumentRepository.FindByGid(c.IncomingDocumentId.Value).IncomingDocumentId : (long?)null,
                    CourtId = _courtRepository.GetNomIdByCode(c.CourtCode),
                    CaseKindId = _caseKindRepository.GetNomIdByCode(c.CaseKindCode),
                    CaseTypeId = _caseTypeRepository.GetNomIdByCode(c.CaseTypeCode),
                    CaseCodeId = !string.IsNullOrWhiteSpace(c.CaseCode) ? _caseCodeRepository.GetNomIdByCode(c.CaseCode) : (long?)null,
                    StatisticCodeId = !string.IsNullOrWhiteSpace(c.StatisticCode) ? _statisticCodeRepository.GetNomIdByCode(c.StatisticCode) : (long?)null,
                    Number = c.Number,
                    CaseYear = c.CaseYear,
                    Status = c.Status,
                    FormationDate = c.FormationDate,
                    DepartmentName = c.DepartmentName,
                    PanelName = c.PanelName,
                    LegalSubject = c.LegalSubject,
                    RestrictedAccess = c.RestrictedAccess //comment for PROD
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _caseRepository.Add(domainCase);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainCase.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateCase(Domain.Service.Entities.Case c)
        {
            ValidateCase(c, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainCase = _caseRepository.FindByGid(c.CaseId.Value);

                    domainCase.IncomingDocumentId = c.IncomingDocumentId.HasValue ? _incomingDocumentRepository.FindByGid(c.IncomingDocumentId.Value).IncomingDocumentId : (long?)null;
                    domainCase.CourtId = _courtRepository.GetNomIdByCode(c.CourtCode);
                    domainCase.CaseKindId = _caseKindRepository.GetNomIdByCode(c.CaseKindCode);
                    domainCase.CaseTypeId = _caseTypeRepository.GetNomIdByCode(c.CaseTypeCode);
                    domainCase.CaseCodeId = !string.IsNullOrWhiteSpace(c.CaseCode) ? _caseCodeRepository.GetNomIdByCode(c.CaseCode) : (long?)null;
                    domainCase.StatisticCodeId = !string.IsNullOrWhiteSpace(c.StatisticCode) ? _statisticCodeRepository.GetNomIdByCode(c.StatisticCode) : (long?)null;
                    domainCase.Number = c.Number;
                    domainCase.CaseYear = c.CaseYear;
                    domainCase.Status = c.Status;
                    domainCase.FormationDate = c.FormationDate;
                    domainCase.DepartmentName = c.DepartmentName;
                    domainCase.PanelName = c.PanelName;
                    domainCase.LegalSubject = c.LegalSubject;
                    domainCase.RestrictedAccess = c.RestrictedAccess; //comment for PROD
                    domainCase.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainCase.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteCase(Guid caseId)
        {
            return this.DeleteDomainEntity(caseId, _caseRepository);
        }

        public Domain.Service.Entities.Case GetCaseById(Guid caseId)
        {
            if (caseId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            var domainCase = _caseRepository.FindByGid(caseId);
            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            var c = new Domain.Service.Entities.Case()
            {
                CaseId = domainCase.Gid,
                IncomingDocumentId = domainCase.IncomingDocumentId.HasValue ? _incomingDocumentRepository.Find(domainCase.IncomingDocumentId.Value).Gid : (Guid?)null,
                CourtCode = _courtRepository.GetNom(domainCase.CourtId).Code,
                CaseKindCode = _caseKindRepository.GetNom(domainCase.CaseKindId).Code,
                CaseTypeCode = _caseTypeRepository.GetNom(domainCase.CaseTypeId).Code,
                CaseCode = domainCase.CaseCodeId.HasValue ? _caseCodeRepository.GetNom(domainCase.CaseCodeId.Value).Code : null,
                StatisticCode = domainCase.StatisticCodeId.HasValue ? _statisticCodeRepository.GetNom(domainCase.StatisticCodeId.Value).Code : null,
                Number = domainCase.Number,
                CaseYear = domainCase.CaseYear,
                Status = domainCase.Status,
                FormationDate = domainCase.FormationDate,
                DepartmentName = domainCase.DepartmentName,
                PanelName = domainCase.PanelName,
                LegalSubject = domainCase.LegalSubject,
                RestrictedAccess = domainCase.RestrictedAccess //comment for PROD
            };

            return c;
        }

        public List<Guid> GetCaseIdentifiersByNumber(string court, string caseType, int caseNumber, int caseYear)
        {
            return upgradeRepository.All<Domain.Entities.Case>()
                                    .Where(x => x.Court.Code == court)
                                    .Where(x => x.CaseType.Code == caseType)
                                    .Where(x => x.Number == caseNumber)
                                    .Where(x => x.CaseYear == caseYear)
                                    .Select(x => x.Gid)
                                    .ToList();
        }
        public List<Guid> GetCaseIdentifiers(int? caseNumber, string caseKindCode, int? caseYear, string department)
        {
            if (ShowStackTrace)
            {
                IQueryable<Domain.Entities.Case> cases = _caseRepository.SetWithoutIncludes();

                var predicate = PredicateBuilder.True<Domain.Entities.Case>();

                if (caseNumber != null)
                    predicate = predicate.And(c => c.Number == caseNumber);

                if (!string.IsNullOrWhiteSpace(caseKindCode))
                    predicate = predicate.And(c => c.CaseKindId == _caseKindRepository.GetNomIdByCode(caseKindCode));

                if (caseYear != null)
                    predicate = predicate.And(c => c.CaseYear == caseYear);

                if (!string.IsNullOrWhiteSpace(department))
                    predicate = predicate.And(c => c.DepartmentName == department);

                if (!predicate.IsTrueLambdaExpr())
                    cases = cases.Where(predicate);

                var list = cases.Select(c => c.Gid).ToList();

                return list;
            }
            else
            {
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Deprecated"));
            }
        }

        private void ValidateCase(Domain.Service.Entities.Case c, bool isUpdate)
        {
            this.ValidateId(c.CaseId, _caseRepository, isUpdate);

            if (c.IncomingDocumentId.HasValue && _incomingDocumentRepository.FindByGid(c.IncomingDocumentId.Value) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Case.IncomingDocumentId"));
            if (string.IsNullOrWhiteSpace(c.CourtCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Case.CourtCode"));
            if (!_courtRepository.HasCode(c.CourtCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Case.CourtCode"));
            if (string.IsNullOrWhiteSpace(c.CaseKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Case.CaseKindCode"));
            if (!_caseKindRepository.HasCode(c.CaseKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Case.CaseKindCode"));
            if (string.IsNullOrWhiteSpace(c.CaseTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Case.CaseTypeCode"));
            if (!_caseTypeRepository.HasCode(c.CaseTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Case.CaseTypeCode"));
            if (!string.IsNullOrWhiteSpace(c.CaseCode) && !_caseCodeRepository.HasCode(c.CaseCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Case.CaseCode"));
            if (!string.IsNullOrWhiteSpace(c.StatisticCode) && !_statisticCodeRepository.HasCode(c.StatisticCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Case.StatisticCode"));
            if (c.Number == default(int))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Case.Number"));
            if (c.CaseYear == default(int))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Case.CaseYear"));
            if (c.FormationDate == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Case.FormationDate"));
        }

        public Guid? GetCaseId(int incDocumentNumber, int incDocumentYear, string courtCode)
        {
            try
            {
                var courtId = _courtRepository.GetNomIdByCode(courtCode);
                return _incomingDocumentRepository.GetIncomingDocumentCaseId(incDocumentNumber, incDocumentYear, courtId);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        //public Guid? GetCaseIdByNumberYearKindCourt(int caseNumber, int caseYear, string caseKindCode, string courtCode)
        //{
        //    try
        //    {
        //        var courtId = _courtRepository.GetNomIdByCode(courtCode);
        //        var caseKindId = _caseKindRepository.GetNomIdByCode(caseKindCode);
        //        return _caseRepository.GetCaseIdByNumberYearKindCourt(caseNumber, caseYear, caseKindId, courtId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw HandleCommonException(ex);
        //    }
        //}

        #endregion

        #region ConnectedCase

        public Guid? InsertConnectedCase(Domain.Service.Entities.ConnectedCase connectedCase)
        {
            ValidateConnectedCase(connectedCase, false);

            try
            {
                var domainConnectedCase = new Domain.Entities.ConnectedCase()
                {
                    CaseId = _caseRepository.FindByGid(connectedCase.CaseId).CaseId,
                    Gid = connectedCase.CaseId,
                    PredecessorCaseId = _caseRepository.FindByGid(connectedCase.PredecessorCaseId).CaseId,
                    ConnectedCaseTypeId = _connectedCaseTypeRepository.GetNomIdByCode(connectedCase.ConnectedCaseTypeCode)
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _connectedCaseRepository.Add(domainConnectedCase);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainConnectedCase.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateConnectedCase(Domain.Service.Entities.ConnectedCase connectedCase)
        {
            ValidateConnectedCase(connectedCase, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainConnectedCase = _connectedCaseRepository.FindByGid(connectedCase.CaseId);

                    domainConnectedCase.PredecessorCaseId = _caseRepository.FindByGid(connectedCase.PredecessorCaseId).CaseId;
                    domainConnectedCase.ConnectedCaseTypeId = _connectedCaseTypeRepository.GetNomIdByCode(connectedCase.ConnectedCaseTypeCode);

                    domainConnectedCase.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainConnectedCase.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteConnectedCase(Guid connectedCaseId)
        {
            return this.DeleteDomainEntity(connectedCaseId, _connectedCaseRepository);
        }

        public Domain.Service.Entities.ConnectedCase GetConnectedCaseById(Guid connectedCaseId)
        {
            if (connectedCaseId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(connectedCaseId)));

            var domainConnectedCase = _connectedCaseRepository.FindByGid(connectedCaseId);
            if (domainConnectedCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(connectedCaseId)));

            var connectedCase = new Domain.Service.Entities.ConnectedCase()
            {
                CaseId = _caseRepository.Find(domainConnectedCase.CaseId).Gid,
                PredecessorCaseId = _caseRepository.Find(domainConnectedCase.PredecessorCaseId).Gid,
                ConnectedCaseTypeCode = domainConnectedCase.ConnectedCaseTypeId.HasValue ? _connectedCaseTypeRepository.GetNom(domainConnectedCase.ConnectedCaseTypeId.Value).Code : null
            };

            return connectedCase;
        }

        public List<Guid> GetConnectedCaseIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.ConnectedCases)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.ConnectedCases.Select(i => i.Gid).ToList();
        }

        private void ValidateConnectedCase(Domain.Service.Entities.ConnectedCase connectedCase, bool isUpdate)
        {
            if (connectedCase.CaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "ConnectedCase.CaseId"));
            if (_caseRepository.FindByGid(connectedCase.CaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "ConnectedCase.CaseId"));
            if (connectedCase.PredecessorCaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "ConnectedCase.PredecessorCaseId"));
            if (_caseRepository.FindByGid(connectedCase.PredecessorCaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "ConnectedCase.PredecessorCaseId"));
            if (string.IsNullOrWhiteSpace(connectedCase.ConnectedCaseTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "ConnectedCase.ConnectedCaseTypeCode"));
            if (!_connectedCaseTypeRepository.HasCode(connectedCase.ConnectedCaseTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "ConnectedCase.ConnectedCaseTypeCode"));
        }

        #endregion

        #region Reporter

        public Guid? InsertReporter(Domain.Service.Entities.Reporter reporter)
        {
            ValidateReporter(reporter, false);

            try
            {
                var domainReporter = new Domain.Entities.Reporter()
                {
                    Gid = reporter.ReporterId.HasValue ? reporter.ReporterId.Value : Guid.NewGuid(),
                    CaseId = _caseRepository.FindByGid(reporter.CaseId).CaseId,
                    JudgeName = reporter.JudgeName,
                    DateAssigned = reporter.DateAssigned,
                    DateReplaced = reporter.DateReplaced,
                    ReasonReplaced = reporter.ReasonReplaced
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _reporterRepository.Add(domainReporter);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainReporter.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateReporter(Domain.Service.Entities.Reporter reporter)
        {
            ValidateReporter(reporter, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainReporter = _reporterRepository.FindByGid(reporter.ReporterId.Value);

                    domainReporter.CaseId = _caseRepository.FindByGid(reporter.CaseId).CaseId;
                    domainReporter.JudgeName = reporter.JudgeName;
                    domainReporter.DateAssigned = reporter.DateAssigned;
                    domainReporter.DateReplaced = reporter.DateReplaced;
                    domainReporter.ReasonReplaced = reporter.ReasonReplaced;

                    domainReporter.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainReporter.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteReporter(Guid reporterId)
        {
            return this.DeleteDomainEntity(reporterId, _reporterRepository);
        }

        public Domain.Service.Entities.Reporter GetReporterById(Guid reporterId)
        {
            if (reporterId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(reporterId)));

            var domainReporter = _reporterRepository.FindByGid(reporterId);
            if (domainReporter == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(reporterId)));

            var reporter = new Domain.Service.Entities.Reporter()
            {
                ReporterId = domainReporter.Gid,
                CaseId = _caseRepository.Find(domainReporter.CaseId).Gid,
                JudgeName = domainReporter.JudgeName,
                DateAssigned = domainReporter.DateAssigned,
                DateReplaced = domainReporter.DateReplaced,
                ReasonReplaced = domainReporter.ReasonReplaced
            };

            return reporter;
        }

        public List<Guid> GetReporterIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.Reporters)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.Reporters.Select(i => i.Gid).ToList();
        }

        private void ValidateReporter(Domain.Service.Entities.Reporter reporter, bool isUpdate)
        {
            this.ValidateId(reporter.ReporterId, _reporterRepository, isUpdate);

            if (reporter.CaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Reporter.CaseId"));
            if (_caseRepository.FindByGid(reporter.CaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Reporter.CaseId"));
            if (string.IsNullOrWhiteSpace(reporter.JudgeName))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Reporter.JudgeName"));
            if (reporter.DateAssigned == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Reporter.DateAssigned"));
            if (reporter.DateReplaced.HasValue && reporter.DateReplaced.Value == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Reporter.DateReplaced"));
        }

        #endregion

        #region Hearing

        public Guid? InsertHearing(Domain.Service.Entities.Hearing hearing)
        {
            ValidateHearing(hearing, false);

            try
            {
                var domainHearing = new Domain.Entities.Hearing()
                {
                    Gid = hearing.HearingId.HasValue ? hearing.HearingId.Value : Guid.NewGuid(),
                    CaseId = _caseRepository.FindByGid(hearing.CaseId).CaseId,
                    HearingType = hearing.HearingType,
                    HearingResult = hearing.HearingResult,
                    Date = hearing.Date,
                    SecretaryName = hearing.SecretaryName,
                    ProsecutorName = hearing.ProsecutorName,
                    CourtRoom = hearing.CourtRoom,
                    VideoUrl = hearing.VideoUrl,
                    IsCanceled = hearing.IsCanceled
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _hearingRepository.Add(domainHearing);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainHearing.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateHearing(Domain.Service.Entities.Hearing hearing)
        {
            ValidateHearing(hearing, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainHearing = _hearingRepository.FindByGid(hearing.HearingId.Value);

                    domainHearing.CaseId = _caseRepository.FindByGid(hearing.CaseId).CaseId;
                    domainHearing.HearingType = hearing.HearingType;
                    domainHearing.HearingResult = hearing.HearingResult;
                    domainHearing.Date = hearing.Date;
                    domainHearing.SecretaryName = hearing.SecretaryName;
                    domainHearing.ProsecutorName = hearing.ProsecutorName;
                    domainHearing.CourtRoom = hearing.CourtRoom;
                    domainHearing.IsCanceled = hearing.IsCanceled;
                    domainHearing.VideoUrl = hearing.VideoUrl;
                    domainHearing.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainHearing.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteHearing(Guid hearingId)
        {
            return this.DeleteDomainEntity(hearingId, _hearingRepository);
        }

        public Domain.Service.Entities.Hearing GetHearingById(Guid hearingId)
        {
            if (hearingId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingId)));

            var domainHearing = _hearingRepository.FindByGid(hearingId);
            if (domainHearing == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingId)));

            var hearing = new Domain.Service.Entities.Hearing()
            {
                HearingId = domainHearing.Gid,
                CaseId = _caseRepository.Find(domainHearing.CaseId).Gid,
                HearingType = domainHearing.HearingType,
                HearingResult = domainHearing.HearingResult,
                Date = domainHearing.Date,
                SecretaryName = domainHearing.SecretaryName,
                ProsecutorName = domainHearing.ProsecutorName,
                CourtRoom = domainHearing.CourtRoom,
                VideoUrl = domainHearing.VideoUrl,
                IsCanceled = domainHearing.IsCanceled
            };

            return hearing;
        }

        public List<Guid> GetHearingIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.Hearings)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.Hearings.Select(i => i.Gid).ToList();
        }

        private void ValidateHearing(Domain.Service.Entities.Hearing hearing, bool isUpdate)
        {
            this.ValidateId(hearing.HearingId, _hearingRepository, isUpdate);

            if (hearing.CaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Hearing.CaseId"));
            if (_caseRepository.FindByGid(hearing.CaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Hearing.CaseId"));
            if (hearing.Date == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Hearing.Date"));
        }

        #endregion

        #region PrivateProtocolFile

        public Guid? InsertPrivateProtocolFile(PrivateProtocolFile privateProtocolFile)
        {
            ValidatePrivateProtocolFile(privateProtocolFile, false);

            try
            {
                var domainHearing = _hearingRepository.FindByGid(privateProtocolFile.HearingId);
                return this.UploadFile(privateProtocolFile.PrivateProtocolFileId, privateProtocolFile.ProtocolContent, privateProtocolFile.ProtocolMimeType, FileType.PrivateProtocol, domainHearing.Date, domainHearing, value => domainHearing.PrivateBlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdatePrivateProtocolFile(PrivateProtocolFile privateProtocolFile)
        {
            ValidatePrivateProtocolFile(privateProtocolFile, true);

            try
            {
                var domainHearing = _hearingRepository.FindByGid(privateProtocolFile.HearingId);
                return this.UploadFile(privateProtocolFile.PrivateProtocolFileId, privateProtocolFile.ProtocolContent, privateProtocolFile.ProtocolMimeType, FileType.PrivateProtocol, domainHearing.Date, domainHearing, value => domainHearing.PrivateBlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeletePrivateProtocolFile(Guid hearingId)
        {
            var domainHearing = _hearingRepository.FindByGid(hearingId);
            if (domainHearing == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingId)));

            return this.DeleteFile(domainHearing, domainHearing.PrivateBlobKey, value => domainHearing.PrivateBlobKey = value);
        }

        public PrivateProtocolFile GetPrivateProtocolFileById(Guid privateProtocolId)
        {
            if (privateProtocolId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(privateProtocolId)));

            var domainHearing = _hearingRepository.SetWithoutIncludes()
                .Include(a => a.PrivateBlob)
                .FirstOrDefault(a => a.PrivateBlobKey == privateProtocolId);
            if (domainHearing == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(privateProtocolId)));

            var privateProtocolFile = new PrivateProtocolFile()
            {
                PrivateProtocolFileId = domainHearing.PrivateBlobKey,
                HearingId = domainHearing.Gid,
                ProtocolMimeType = _blobStorageRepository.GetMimeType(domainHearing.PrivateBlob.FileName),
                ProtocolContent = _blobStorageRepository.GetFileContent(domainHearing.PrivateBlobKey.Value),
            };

            return privateProtocolFile;
        }

        public Guid? GetPrivateProtocolFileIdentifierByHearingId(Guid hearingId)
        {
            var domainHearing = _hearingRepository.FindByGid(hearingId);
            if (domainHearing == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingId)));

            return domainHearing.PrivateBlobKey;
        }

        private void ValidatePrivateProtocolFile(PrivateProtocolFile privateProtocolFile, bool isUpdate)
        {
            var domainHearing = _hearingRepository.FindByGid(privateProtocolFile.HearingId);

            this.ValidateFile(privateProtocolFile.PrivateProtocolFileId, privateProtocolFile.HearingId, privateProtocolFile.ProtocolContent, privateProtocolFile.ProtocolMimeType, domainHearing?.PrivateBlobKey, domainHearing, isUpdate);
        }

        #endregion

        #region PublicProtocolFile

        public Guid? InsertPublicProtocolFile(PublicProtocolFile publicProtocolFile)
        {
            ValidatePublicProtocolFile(publicProtocolFile, false);

            try
            {
                var domainHearing = _hearingRepository.FindByGid(publicProtocolFile.HearingId);
                return this.UploadFile(publicProtocolFile.PublicProtocolFileId, publicProtocolFile.ProtocolContent, publicProtocolFile.ProtocolMimeType, FileType.PublicProtocol, domainHearing.Date, domainHearing, value => domainHearing.PublicBlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdatePublicProtocolFile(PublicProtocolFile publicProtocolFile)
        {
            ValidatePublicProtocolFile(publicProtocolFile, true);

            try
            {
                var domainHearing = _hearingRepository.FindByGid(publicProtocolFile.HearingId);
                return this.UploadFile(publicProtocolFile.PublicProtocolFileId, publicProtocolFile.ProtocolContent, publicProtocolFile.ProtocolMimeType, FileType.PublicProtocol, domainHearing.Date, domainHearing, value => domainHearing.PublicBlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeletePublicProtocolFile(Guid hearingId)
        {
            var domainHearing = _hearingRepository.FindByGid(hearingId);
            if (domainHearing == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingId)));

            return this.DeleteFile(domainHearing, domainHearing.PublicBlobKey, value => domainHearing.PublicBlobKey = value);
        }

        public PublicProtocolFile GetPublicProtocolFileById(Guid publicProtocolFileId)
        {
            if (publicProtocolFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(publicProtocolFileId)));

            var domainHearing = _hearingRepository.SetWithoutIncludes()
                .Include(a => a.PublicBlob)
                .FirstOrDefault(a => a.PublicBlobKey == publicProtocolFileId);
            if (domainHearing == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(publicProtocolFileId)));

            var publicProtocolFile = new PublicProtocolFile()
            {
                PublicProtocolFileId = domainHearing.PublicBlobKey,
                HearingId = domainHearing.Gid,
                ProtocolMimeType = _blobStorageRepository.GetMimeType(domainHearing.PublicBlob.FileName),
                ProtocolContent = _blobStorageRepository.GetFileContent(domainHearing.PublicBlobKey.Value),
            };

            return publicProtocolFile;
        }

        public Guid? GetPublicProtocolFileIdentifierByHearingId(Guid hearingId)
        {
            var domainHearing = _hearingRepository.FindByGid(hearingId);
            if (domainHearing == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingId)));

            return domainHearing.PublicBlobKey;
        }

        private void ValidatePublicProtocolFile(PublicProtocolFile publicProtocolFile, bool isUpdate)
        {
            var domainHearing = _hearingRepository.FindByGid(publicProtocolFile.HearingId);

            this.ValidateFile(publicProtocolFile.PublicProtocolFileId, publicProtocolFile.HearingId, publicProtocolFile.ProtocolContent, publicProtocolFile.ProtocolMimeType, domainHearing?.PublicBlobKey, domainHearing, isUpdate);
        }

        #endregion

        #region Assignment

        public Guid? InsertAssignment(Domain.Service.Entities.Assignment assignment)
        {
            ValidateAssignment(assignment, false);

            try
            {
                var domainAssignment = new Domain.Entities.Assignment()
                {
                    Gid = assignment.AssignmentId.HasValue ? assignment.AssignmentId.Value : Guid.NewGuid(),
                    CaseId = _caseRepository.FindByGid(assignment.CaseId).CaseId,
                    IncomingDocumentId = _incomingDocumentRepository.FindByGid(assignment.IncomingDocumentId).IncomingDocumentId,
                    JudgeName = assignment.JudgeName,
                    Date = assignment.Date,
                    Type = assignment.Type,
                    Assignor = assignment.Assignor
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _assignmentRepository.Add(domainAssignment);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainAssignment.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateAssignment(Domain.Service.Entities.Assignment assignment)
        {
            ValidateAssignment(assignment, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainAssignment = _assignmentRepository.FindByGid(assignment.AssignmentId.Value);

                    domainAssignment.CaseId = _caseRepository.FindByGid(assignment.CaseId).CaseId;
                    domainAssignment.IncomingDocumentId = _incomingDocumentRepository.FindByGid(assignment.IncomingDocumentId).IncomingDocumentId;
                    domainAssignment.JudgeName = assignment.JudgeName;
                    domainAssignment.Date = assignment.Date;
                    domainAssignment.Type = assignment.Type;
                    domainAssignment.Assignor = assignment.Assignor;

                    domainAssignment.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainAssignment.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteAssignment(Guid assignmentId)
        {
            return this.DeleteDomainEntity(assignmentId, _assignmentRepository);
        }

        public Domain.Service.Entities.Assignment GetAssignmentById(Guid assignmentId)
        {
            if (assignmentId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentId)));

            var domainAssignment = _assignmentRepository.FindByGid(assignmentId);
            if (domainAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentId)));

            var assignment = new Domain.Service.Entities.Assignment()
            {
                AssignmentId = domainAssignment.Gid,
                CaseId = _caseRepository.Find(domainAssignment.CaseId).Gid,
                IncomingDocumentId = _incomingDocumentRepository.Find(domainAssignment.IncomingDocumentId).Gid,
                Type = domainAssignment.Type,
                Date = domainAssignment.Date,
                JudgeName = domainAssignment.JudgeName,
                Assignor = domainAssignment.Assignor
            };

            return assignment;
        }

        public List<Guid> GetAssignmentIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.Assignments)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.Assignments.Select(i => i.Gid).ToList();
        }

        private void ValidateAssignment(Domain.Service.Entities.Assignment assignment, bool isUpdate)
        {
            this.ValidateId(assignment.AssignmentId, _assignmentRepository, isUpdate);

            if (assignment.CaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Assignment.CaseId"));
            if (_caseRepository.FindByGid(assignment.CaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Assignment.CaseId"));
            if (assignment.IncomingDocumentId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Assignment.IncomingDocumentId"));
            if (_incomingDocumentRepository.FindByGid(assignment.IncomingDocumentId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Assignment.IncomingDocumentId"));
            if (string.IsNullOrWhiteSpace(assignment.JudgeName))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Assignment.JudgeName"));
            if (assignment.Date == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Assignment.Date"));
            if (string.IsNullOrWhiteSpace(assignment.Type))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Assignment.Type"));
            if (string.IsNullOrWhiteSpace(assignment.Assignor))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Assignment.Assignor"));
        }

        #endregion

        #region AssignmentFile

        public Guid? InsertAssignmentFile(AssignmentFile assignmentFile)
        {
            ValidateAssignmentFile(assignmentFile, false);

            try
            {
                var domainAssignment = _assignmentRepository.FindByGid(assignmentFile.AssignmentId);
                return this.UploadFile(assignmentFile.AssignmentFileId, assignmentFile.ProtocolContent, assignmentFile.ProtocolMimeType, FileType.Assignment, domainAssignment.Date, domainAssignment, value => domainAssignment.BlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateAssignmentFile(AssignmentFile assignmentFile)
        {
            ValidateAssignmentFile(assignmentFile, true);

            try
            {
                var domainAssignment = _assignmentRepository.FindByGid(assignmentFile.AssignmentId);
                return this.UploadFile(assignmentFile.AssignmentFileId, assignmentFile.ProtocolContent, assignmentFile.ProtocolMimeType, FileType.Assignment, domainAssignment.Date, domainAssignment, value => domainAssignment.BlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteAssignmentFile(Guid assignmentId)
        {
            var domainAssignment = _assignmentRepository.FindByGid(assignmentId);
            if (domainAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentId)));

            return this.DeleteFile(domainAssignment, domainAssignment.BlobKey, value => domainAssignment.BlobKey = value);
        }

        public AssignmentFile GetAssignmentFileById(Guid assignmentFileId)
        {
            if (assignmentFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentFileId)));

            var domainAssignment = _assignmentRepository.SetWithoutIncludes()
                .Include(a => a.Blob)
                .FirstOrDefault(a => a.BlobKey == assignmentFileId);
            if (domainAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentFileId)));

            var assignmentFile = new AssignmentFile()
            {
                AssignmentFileId = domainAssignment.BlobKey,
                AssignmentId = domainAssignment.Gid,
                ProtocolMimeType = _blobStorageRepository.GetMimeType(domainAssignment.Blob.FileName),
                ProtocolContent = _blobStorageRepository.GetFileContent(domainAssignment.BlobKey.Value),
            };

            return assignmentFile;
        }

        public Guid? GetAssignmentFileIdentifiersByAssignmentId(Guid assignmentId)
        {
            var domainAssignment = _assignmentRepository.FindByGid(assignmentId);
            if (domainAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentId)));

            return domainAssignment.BlobKey;
        }

        private void ValidateAssignmentFile(AssignmentFile assignmentFile, bool isUpdate)
        {
            var domainAssignment = _assignmentRepository.FindByGid(assignmentFile.AssignmentId);

            this.ValidateFile(assignmentFile.AssignmentFileId, assignmentFile.AssignmentId, assignmentFile.ProtocolContent, assignmentFile.ProtocolMimeType, domainAssignment?.BlobKey, domainAssignment, isUpdate);
        }

        #endregion

        #region HearingParticipant

        public Guid? InsertHearingParticipant(Domain.Service.Entities.HearingParticipant hearingParticipant)
        {
            ValidateHearingParticipant(hearingParticipant, false);

            try
            {
                var domainHearingParticipant = new Domain.Entities.HearingParticipant()
                {
                    Gid = hearingParticipant.HearingParticipantId.HasValue ? hearingParticipant.HearingParticipantId.Value : Guid.NewGuid(),
                    HearingId = _hearingRepository.FindByGid(hearingParticipant.HearingId).HearingId,
                    JudgeName = hearingParticipant.JudgeName,
                    Role = hearingParticipant.Role,
                    SubstituteFor = hearingParticipant.SubstituteFor,
                    SubstituteReason = hearingParticipant.SubstituteReason
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _hearingParticipantRepository.Add(domainHearingParticipant);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainHearingParticipant.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateHearingParticipant(Domain.Service.Entities.HearingParticipant hearingParticipant)
        {
            ValidateHearingParticipant(hearingParticipant, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainHearingParticipant = _hearingParticipantRepository.FindByGid(hearingParticipant.HearingParticipantId.Value);

                    domainHearingParticipant.HearingId = _hearingRepository.FindByGid(hearingParticipant.HearingId).HearingId;
                    domainHearingParticipant.JudgeName = hearingParticipant.JudgeName;
                    domainHearingParticipant.Role = hearingParticipant.Role;
                    domainHearingParticipant.SubstituteFor = hearingParticipant.SubstituteFor;
                    domainHearingParticipant.SubstituteReason = hearingParticipant.SubstituteReason;

                    domainHearingParticipant.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainHearingParticipant.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteHearingParticipant(Guid hearingParticipantId)
        {
            return this.DeleteDomainEntity(hearingParticipantId, _hearingParticipantRepository);
        }

        public Domain.Service.Entities.HearingParticipant GetHearingParticipantById(Guid hearingParticipantId)
        {
            if (hearingParticipantId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingParticipantId)));

            var domainHearingParticipant = _hearingParticipantRepository.FindByGid(hearingParticipantId);
            if (domainHearingParticipant == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingParticipantId)));

            var hearingParticipant = new Domain.Service.Entities.HearingParticipant()
            {
                HearingParticipantId = domainHearingParticipant.Gid,
                HearingId = _hearingRepository.Find(domainHearingParticipant.HearingId).Gid,
                JudgeName = domainHearingParticipant.JudgeName,
                Role = domainHearingParticipant.Role,
                SubstituteFor = domainHearingParticipant.SubstituteFor,
                SubstituteReason = domainHearingParticipant.SubstituteReason
            };

            return hearingParticipant;
        }

        public List<Guid> GetHearingParticipantIdentifiersByHearingId(Guid hearingId)
        {
            var domainHearing = _hearingRepository.SetWithoutIncludes()
                .Include(i => i.HearingParticipants)
                .FirstOrDefault(i => i.Gid == hearingId);

            if (domainHearing == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(hearingId)));

            return domainHearing.HearingParticipants.Select(i => i.Gid).ToList();
        }

        private void ValidateHearingParticipant(Domain.Service.Entities.HearingParticipant participant, bool isUpdate)
        {
            this.ValidateId(participant.HearingParticipantId, _hearingParticipantRepository, isUpdate);

            if (participant.HearingId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "HearingParticipant.HearingId"));
            if (_hearingRepository.FindByGid(participant.HearingId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "HearingParticipant.HearingId"));
            if (string.IsNullOrWhiteSpace(participant.JudgeName))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "HearingParticipant.JudgeName"));
            if (string.IsNullOrWhiteSpace(participant.Role))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "HearingParticipant.Role"));
        }

        #endregion

        #region Act

        public Guid? InsertAct(Domain.Service.Entities.Act act)
        {
            ValidateAct(act, false);

            try
            {
                var domainAct = new Domain.Entities.Act()
                {
                    Gid = act.ActId.HasValue ? act.ActId.Value : Guid.NewGuid(),
                    CaseId = _caseRepository.FindByGid(act.CaseId).CaseId,
                    ActKindId = _actKindRepository.GetNomIdByCode(act.ActKindCode),
                    HearingId = act.HearingId.HasValue ? _hearingRepository.FindByGid(act.HearingId.Value).HearingId : (long?)null,
                    Number = act.Number,
                    DateSigned = act.DateSigned,
                    DateInPower = act.DateInPower,
                    MotiveDate = act.MotiveDate//,
                    // Finishing = act.Finishing//, //comment for PROD
                    // CanBeSubjectToAppeal = act.CanBeSubjectToAppeal //comment for PROD
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _actRepository.Add(domainAct);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainAct.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateAct(Domain.Service.Entities.Act act)
        {
            ValidateAct(act, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainAct = _actRepository.FindByGid(act.ActId.Value);

                    domainAct.CaseId = _caseRepository.FindByGid(act.CaseId).CaseId;
                    domainAct.ActKindId = _actKindRepository.GetNomIdByCode(act.ActKindCode);
                    domainAct.HearingId = act.HearingId.HasValue ? _hearingRepository.FindByGid(act.HearingId.Value).HearingId : (long?)null;
                    domainAct.Number = act.Number;
                    domainAct.DateSigned = act.DateSigned;
                    domainAct.DateInPower = act.DateInPower;
                    domainAct.MotiveDate = act.MotiveDate;
                    domainAct.ModifyDate = DateTime.Now;
                    // domainAct.Finishing = act.Finishing; //comment for PROD
                    //domainAct.CanBeSubjectToAppeal = act.CanBeSubjectToAppeal; //comment for PROD

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainAct.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteAct(Guid actId)
        {
            return this.DeleteDomainEntity(actId, _actRepository);
        }

        public Domain.Service.Entities.Act GetActById(Guid actId)
        {
            if (actId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            var act = new Domain.Service.Entities.Act()
            {
                ActId = domainAct.Gid,
                CaseId = _caseRepository.Find(domainAct.CaseId).Gid,
                ActKindCode = _actKindRepository.GetNom(domainAct.ActKindId).Code,
                HearingId = domainAct.HearingId.HasValue ? _hearingRepository.Find(domainAct.HearingId.Value).Gid : (Guid?)null,
                Number = domainAct.Number,
                DateSigned = domainAct.DateSigned,
                DateInPower = domainAct.DateInPower,
                MotiveDate = domainAct.MotiveDate,
                // Finishing = domainAct.Finishing, //comment for PROD
                //CanBeSubjectToAppeal = domainAct.CanBeSubjectToAppeal //comment for PROD
            };

            return act;
        }

        public List<Guid> GetActIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.Acts)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.Acts.Select(i => i.Gid).ToList();
        }

        private void ValidateAct(Domain.Service.Entities.Act act, bool isUpdate)
        {
            this.ValidateId(act.ActId, _actRepository, isUpdate);

            if (act.CaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Act.CaseId"));
            if (_caseRepository.FindByGid(act.CaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Act.CaseId"));
            if (string.IsNullOrWhiteSpace(act.ActKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Act.ActKindCode"));
            if (!_actKindRepository.HasCode(act.ActKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Act.ActKindCode"));
            if (act.HearingId.HasValue)
            {
                if (act.HearingId.Value == default(Guid))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Act.HearingId"));
                if (_hearingRepository.FindByGid(act.HearingId.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Act.HearingId"));
            }

            if (act.DateSigned == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Act.DateSigned"));
            if (act.DateInPower == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Act.DateInPower"));
            if (act.MotiveDate == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Act.MotiveDate"));
        }

        #endregion

        #region PrivateActFile

        public Guid? InsertPrivateActFile(PrivateActFile privateActFile)
        {
            ValidatePrivateActFile(privateActFile, false);

            try
            {
                var domainAct = _actRepository.FindByGid(privateActFile.ActId);
                return this.UploadFile(privateActFile.PrivateActFileId, privateActFile.PrivateActContent, privateActFile.PrivateActMimeType, FileType.PrivateAct, domainAct.DateSigned, domainAct, value => domainAct.PrivateActBlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdatePrivateActFile(PrivateActFile privateActFile)
        {
            ValidatePrivateActFile(privateActFile, true);

            try
            {
                var domainAct = _actRepository.FindByGid(privateActFile.ActId);
                return this.UploadFile(privateActFile.PrivateActFileId, privateActFile.PrivateActContent, privateActFile.PrivateActMimeType, FileType.PrivateAct, domainAct.DateSigned, domainAct, value => domainAct.PrivateActBlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeletePrivateActFile(Guid actId)
        {
            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return this.DeleteFile(domainAct, domainAct.PrivateActBlobKey, value => domainAct.PrivateActBlobKey = value);
        }

        public PrivateActFile GetPrivateActFileById(Guid privateActFileId)
        {
            if (privateActFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(privateActFileId)));

            var domainAct = _actRepository.SetWithoutIncludes()
                .Include(a => a.PrivateActBlob)
                .FirstOrDefault(a => a.PrivateActBlobKey == privateActFileId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(privateActFileId)));

            var privateActFile = new PrivateActFile()
            {
                PrivateActFileId = domainAct.PrivateActBlobKey,
                ActId = domainAct.Gid,
                PrivateActMimeType = _blobStorageRepository.GetMimeType(domainAct.PrivateActBlob.FileName),
                PrivateActContent = _blobStorageRepository.GetFileContent(domainAct.PrivateActBlobKey.Value),
            };

            return privateActFile;
        }

        public Guid? GetPrivateActFileIdentifiersByActId(Guid actId)
        {
            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return domainAct.PrivateActBlobKey;
        }

        private void ValidatePrivateActFile(PrivateActFile privateActFile, bool isUpdate)
        {
            var domainAct = _actRepository.FindByGid(privateActFile.ActId);

            this.ValidateFile(privateActFile.PrivateActFileId, privateActFile.ActId, privateActFile.PrivateActContent, privateActFile.PrivateActMimeType, domainAct?.PrivateActBlobKey, domainAct, isUpdate);
        }

        #endregion

        #region PublicActFile

        public Guid? InsertPublicActFile(PublicActFile publicActFile)
        {
            ValidatePublicActFile(publicActFile, false);

            try
            {
                var domainAct = _actRepository.FindByGid(publicActFile.ActId);
                return this.UploadFile(publicActFile.PublicActFileId, publicActFile.PublicActContent, publicActFile.PublicActMimeType, FileType.PublicAct, domainAct.DateSigned, domainAct, value => domainAct.PublicActBlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdatePublicActFile(PublicActFile publicActFile)
        {
            ValidatePublicActFile(publicActFile, true);

            try
            {
                var domainAct = _actRepository.FindByGid(publicActFile.ActId);
                return this.UploadFile(publicActFile.PublicActFileId, publicActFile.PublicActContent, publicActFile.PublicActMimeType, FileType.PublicAct, domainAct.DateSigned, domainAct, value => domainAct.PublicActBlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeletePublicActFile(Guid actId)
        {
            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return this.DeleteFile(domainAct, domainAct.PublicActBlobKey, value => domainAct.PublicActBlobKey = value);
        }

        public PublicActFile GetPublicActFileById(Guid publicActFileId)
        {
            if (publicActFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(publicActFileId)));

            var domainAct = _actRepository.SetWithoutIncludes()
                .Include(a => a.PublicActBlob)
                .FirstOrDefault(a => a.PublicActBlobKey == publicActFileId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(publicActFileId)));

            var publicActFile = new PublicActFile()
            {
                PublicActFileId = domainAct.PublicActBlobKey,
                ActId = domainAct.Gid,
                PublicActMimeType = _blobStorageRepository.GetMimeType(domainAct.PublicActBlob.FileName),
                PublicActContent = _blobStorageRepository.GetFileContent(domainAct.PublicActBlobKey.Value),
            };

            return publicActFile;
        }

        public Guid? GetPublicActFileIdentifierByActId(Guid actId)
        {
            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return domainAct.PublicActBlobKey;
        }

        private void ValidatePublicActFile(PublicActFile publicActFile, bool isUpdate)
        {
            var domainAct = _actRepository.FindByGid(publicActFile.ActId);

            this.ValidateFile(publicActFile.PublicActFileId, publicActFile.ActId, publicActFile.PublicActContent, publicActFile.PublicActMimeType, domainAct?.PublicActBlobKey, domainAct, isUpdate);
        }

        #endregion

        #region PrivateMotiveFile

        public Guid? InsertPrivateMotiveFile(PrivateMotiveFile privateMotiveFile)
        {
            ValidatePrivateMotiveFile(privateMotiveFile, false);

            try
            {
                var domainAct = _actRepository.FindByGid(privateMotiveFile.ActId);
                return this.UploadFile(privateMotiveFile.PrivateMotiveFileId, privateMotiveFile.PrivateMotiveContent, privateMotiveFile.PrivateMotiveMimeType, FileType.PrivateMotive, domainAct.DateSigned, domainAct, value => domainAct.PrivateMotiveBlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdatePrivateMotiveFile(PrivateMotiveFile privateMotiveFile)
        {
            ValidatePrivateMotiveFile(privateMotiveFile, true);

            try
            {
                var domainAct = _actRepository.FindByGid(privateMotiveFile.ActId);
                return this.UploadFile(privateMotiveFile.PrivateMotiveFileId, privateMotiveFile.PrivateMotiveContent, privateMotiveFile.PrivateMotiveMimeType, FileType.PrivateMotive, domainAct.DateSigned, domainAct, value => domainAct.PrivateMotiveBlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeletePrivateMotiveFile(Guid actId)
        {
            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return this.DeleteFile(domainAct, domainAct.PrivateMotiveBlobKey, value => domainAct.PrivateMotiveBlobKey = value);
        }

        public PrivateMotiveFile GetPrivateMotiveFileById(Guid privateMotiveFileId)
        {
            if (privateMotiveFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(privateMotiveFileId)));

            var domainAct = _actRepository.SetWithoutIncludes()
                .Include(a => a.PrivateMotiveBlob)
                .FirstOrDefault(a => a.PrivateMotiveBlobKey == privateMotiveFileId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(privateMotiveFileId)));

            var privateMotiveFile = new PrivateMotiveFile()
            {
                PrivateMotiveFileId = domainAct.PrivateMotiveBlobKey,
                ActId = domainAct.Gid,
                PrivateMotiveMimeType = _blobStorageRepository.GetMimeType(domainAct.PrivateMotiveBlob.FileName),
                PrivateMotiveContent = _blobStorageRepository.GetFileContent(domainAct.PrivateMotiveBlobKey.Value),
            };

            return privateMotiveFile;
        }

        public Guid? GetPrivateMotiveFileIdentifierByActId(Guid actId)
        {
            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return domainAct.PrivateMotiveBlobKey;
        }

        private void ValidatePrivateMotiveFile(PrivateMotiveFile privateMotiveFile, bool isUpdate)
        {
            var domainAct = _actRepository.FindByGid(privateMotiveFile.ActId);

            this.ValidateFile(privateMotiveFile.PrivateMotiveFileId, privateMotiveFile.ActId, privateMotiveFile.PrivateMotiveContent, privateMotiveFile.PrivateMotiveMimeType, domainAct?.PrivateMotiveBlobKey, domainAct, isUpdate);
        }

        #endregion

        #region PublicMotiveFile

        public Guid? InsertPublicMotiveFile(PublicMotiveFile publicMotiveFile)
        {
            ValidatePublicMotiveFile(publicMotiveFile, false);

            try
            {
                var domainAct = _actRepository.FindByGid(publicMotiveFile.ActId);
                return this.UploadFile(publicMotiveFile.PublicMotiveFileId, publicMotiveFile.PublicMotiveContent, publicMotiveFile.PublicMotiveMimeType, FileType.PublicMotive, domainAct.DateSigned, domainAct, value => domainAct.PublicMotiveBlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdatePublicMotiveFile(PublicMotiveFile publicMotiveFile)
        {
            ValidatePublicMotiveFile(publicMotiveFile, true);

            try
            {
                var domainAct = _actRepository.FindByGid(publicMotiveFile.ActId);
                return this.UploadFile(publicMotiveFile.PublicMotiveFileId, publicMotiveFile.PublicMotiveContent, publicMotiveFile.PublicMotiveMimeType, FileType.PublicMotive, domainAct.DateSigned, domainAct, value => domainAct.PublicMotiveBlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeletePublicMotiveFile(Guid actId)
        {
            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return this.DeleteFile(domainAct, domainAct.PublicMotiveBlobKey, value => domainAct.PublicMotiveBlobKey = value);
        }

        public PublicMotiveFile GetPublicMotiveFileById(Guid publicMotiveFileId)
        {
            if (publicMotiveFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(publicMotiveFileId)));

            var domainAct = _actRepository.SetWithoutIncludes()
                .Include(a => a.PublicMotiveBlob)
                .FirstOrDefault(a => a.PublicMotiveBlobKey == publicMotiveFileId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(publicMotiveFileId)));

            var publicMotiveFile = new PublicMotiveFile()
            {
                PublicMotiveFileId = domainAct.PublicMotiveBlobKey,
                ActId = domainAct.Gid,
                PublicMotiveMimeType = _blobStorageRepository.GetMimeType(domainAct.PublicMotiveBlob.FileName),
                PublicMotiveContent = _blobStorageRepository.GetFileContent(domainAct.PublicMotiveBlobKey.Value),
            };

            return publicMotiveFile;
        }

        public Guid? GetPublicMotiveFileIdentifierByActId(Guid actId)
        {
            var domainAct = _actRepository.FindByGid(actId);
            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return domainAct.PublicMotiveBlobKey;
        }

        private void ValidatePublicMotiveFile(PublicMotiveFile publicMotiveFile, bool isUpdate)
        {
            var domainAct = _actRepository.FindByGid(publicMotiveFile.ActId);

            this.ValidateFile(publicMotiveFile.PublicMotiveFileId, publicMotiveFile.ActId, publicMotiveFile.PublicMotiveContent, publicMotiveFile.PublicMotiveMimeType, domainAct?.PublicMotiveBlobKey, domainAct, isUpdate);
        }

        #endregion

        #region ActPreparator

        public Guid? InsertActPreparator(Domain.Service.Entities.ActPreparator actPreparator)
        {
            ValidateActPreparator(actPreparator, false);

            try
            {
                var domainActPreparator = new Domain.Entities.ActPreparator()
                {
                    Gid = actPreparator.ActPreparatorId.HasValue ? actPreparator.ActPreparatorId.Value : Guid.NewGuid(),
                    ActId = _actRepository.FindByGid(actPreparator.ActId).ActId,
                    JudgeName = actPreparator.JudgeName,
                    Role = actPreparator.Role,
                    SubstituteFor = actPreparator.SubstituteFor,
                    SubstituteReason = actPreparator.SubstituteReason
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _actPreparatorRepository.Add(domainActPreparator);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainActPreparator.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateActPreparator(Domain.Service.Entities.ActPreparator actPreparator)
        {
            ValidateActPreparator(actPreparator, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainActPreparator = _actPreparatorRepository.FindByGid(actPreparator.ActPreparatorId.Value);

                    domainActPreparator.ActId = _actRepository.FindByGid(actPreparator.ActId).ActId;
                    domainActPreparator.JudgeName = actPreparator.JudgeName;
                    domainActPreparator.Role = actPreparator.Role;
                    domainActPreparator.SubstituteFor = actPreparator.SubstituteFor;
                    domainActPreparator.SubstituteReason = actPreparator.SubstituteReason;

                    domainActPreparator.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainActPreparator.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteActPreparator(Guid actPreparatorId)
        {
            return this.DeleteDomainEntity(actPreparatorId, _actPreparatorRepository);
        }

        public Domain.Service.Entities.ActPreparator GetActPreparatorById(Guid actPreparatorId)
        {
            if (actPreparatorId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actPreparatorId)));

            var domainActPreparator = _actPreparatorRepository.FindByGid(actPreparatorId);
            if (domainActPreparator == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actPreparatorId)));

            var actPreparator = new Domain.Service.Entities.ActPreparator()
            {
                ActPreparatorId = domainActPreparator.Gid,
                ActId = _actRepository.Find(domainActPreparator.ActId).Gid,
                JudgeName = domainActPreparator.JudgeName,
                Role = domainActPreparator.Role,
                SubstituteFor = domainActPreparator.SubstituteFor,
                SubstituteReason = domainActPreparator.SubstituteReason
            };

            return actPreparator;
        }

        public List<Guid> GetActPreparatorIdentifiersByActId(Guid actId)
        {
            var domainAct = _actRepository.SetWithoutIncludes()
                .Include(i => i.ActPreparators)
                .FirstOrDefault(i => i.Gid == actId);

            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return domainAct.ActPreparators.Select(i => i.Gid).ToList();
        }

        private void ValidateActPreparator(Domain.Service.Entities.ActPreparator preparator, bool isUpdate)
        {
            this.ValidateId(preparator.ActPreparatorId, _actPreparatorRepository, isUpdate);

            if (preparator.ActId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "ActPreparator.ActId"));
            if (_actRepository.FindByGid(preparator.ActId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "ActPreparator.ActId"));
            if (string.IsNullOrWhiteSpace(preparator.JudgeName))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "ActPreparator.JudgeName"));
            if (string.IsNullOrWhiteSpace(preparator.Role))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "ActPreparator.Role"));
        }

        #endregion

        #region Appeal

        public Guid? InsertAppeal(Domain.Service.Entities.Appeal appeal)
        {
            ValidateAppeal(appeal, false);

            try
            {
                var domainAppeal = new Domain.Entities.Appeal()
                {
                    Gid = appeal.AppealId.HasValue ? appeal.AppealId.Value : Guid.NewGuid(),
                    ActId = _actRepository.FindByGid(appeal.ActId).ActId,
                    AppealKindId = _appealKindRepository.GetNomIdByCode(appeal.AppealKindCode),
                    SideId = _sideRepository.FindByGid(appeal.SideId).SideId,
                    DateFiled = appeal.DateFiled
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _appealRepository.Add(domainAppeal);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainAppeal.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateAppeal(Domain.Service.Entities.Appeal appeal)
        {
            ValidateAppeal(appeal, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainAppeal = _appealRepository.FindByGid(appeal.AppealId.Value);

                    domainAppeal.Gid = appeal.AppealId.HasValue ? appeal.AppealId.Value : Guid.NewGuid();
                    domainAppeal.ActId = _actRepository.FindByGid(appeal.ActId).ActId;
                    domainAppeal.AppealKindId = _appealKindRepository.GetNomIdByCode(appeal.AppealKindCode);
                    domainAppeal.SideId = _sideRepository.FindByGid(appeal.SideId).SideId;
                    domainAppeal.DateFiled = appeal.DateFiled;

                    domainAppeal.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainAppeal.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteAppeal(Guid appealId)
        {
            return this.DeleteDomainEntity(appealId, _appealRepository);
        }

        public Domain.Service.Entities.Appeal GetAppealById(Guid appealId)
        {
            if (appealId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(appealId)));

            var domainAppeal = _appealRepository.FindByGid(appealId);
            if (domainAppeal == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(appealId)));

            var appeal = new Domain.Service.Entities.Appeal()
            {
                AppealId = domainAppeal.Gid,
                ActId = _actRepository.Find(domainAppeal.ActId).Gid,
                AppealKindCode = _appealKindRepository.GetNom(domainAppeal.AppealKindId).Code,
                SideId = _sideRepository.Find(domainAppeal.SideId).Gid,
                DateFiled = domainAppeal.DateFiled
            };

            return appeal;
        }

        public List<Guid> GetAppealIdentifiersByActId(Guid actId)
        {
            var domainAct = _actRepository.SetWithoutIncludes()
                .Include(i => i.Appeals)
                .FirstOrDefault(i => i.Gid == actId);

            if (domainAct == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(actId)));

            return domainAct.Appeals.Select(i => i.Gid).ToList();
        }

        private void ValidateAppeal(Domain.Service.Entities.Appeal appeal, bool isUpdate)
        {
            this.ValidateId(appeal.AppealId, _appealRepository, isUpdate);

            if (appeal.ActId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Appeal.ActId"));
            if (_actRepository.FindByGid(appeal.ActId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Appeal.ActId"));
            if (string.IsNullOrWhiteSpace(appeal.AppealKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Appeal.AppealKindCode"));
            if (!_appealKindRepository.HasCode(appeal.AppealKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Appeal.AppealKindCode"));
            if (appeal.SideId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Appeal.SideId"));
            if (_sideRepository.FindByGid(appeal.SideId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Appeal.SideId"));
            if (appeal.DateFiled == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Appeal.DateFiled"));
        }

        #endregion

        #region CaseRuling

        public Guid? InsertCaseRuling(Domain.Service.Entities.CaseRuling caseRuling)
        {
            ValidateCaseRuling(caseRuling, false);

            try
            {
                var domainCaseRuling = new Domain.Entities.CaseRuling()
                {
                    Gid = caseRuling.CaseRulingId.HasValue ? caseRuling.CaseRulingId.Value : Guid.NewGuid(),
                    CaseId = _caseRepository.FindByGid(caseRuling.CaseId).CaseId,
                    HearingId = caseRuling.HearingId.HasValue ? _hearingRepository.FindByGid(caseRuling.HearingId.Value).HearingId : (long?)null,
                    ActId = caseRuling.ActId.HasValue ? _actRepository.FindByGid(caseRuling.ActId.Value).ActId : (long?)null,
                    CaseRulingKindId = _caseRulingKindRepository.GetNomIdByCode(caseRuling.CaseRulingKindCode)
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _caseRulingRepository.Add(domainCaseRuling);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainCaseRuling.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateCaseRuling(Domain.Service.Entities.CaseRuling caseRuling)
        {
            ValidateCaseRuling(caseRuling, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainCaseRuling = _caseRulingRepository.FindByGid(caseRuling.CaseRulingId.Value);

                    domainCaseRuling.CaseId = _caseRepository.FindByGid(caseRuling.CaseId).CaseId;
                    domainCaseRuling.HearingId = caseRuling.HearingId.HasValue ? _hearingRepository.FindByGid(caseRuling.HearingId.Value).HearingId : (long?)null;
                    domainCaseRuling.ActId = caseRuling.ActId.HasValue ? _actRepository.FindByGid(caseRuling.ActId.Value).ActId : (long?)null;
                    domainCaseRuling.CaseRulingKindId = _caseRulingKindRepository.GetNomIdByCode(caseRuling.CaseRulingKindCode);

                    domainCaseRuling.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainCaseRuling.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteCaseRuling(Guid caseRulingId)
        {
            return this.DeleteDomainEntity(caseRulingId, _caseRulingRepository);
        }

        public Domain.Service.Entities.CaseRuling GetCaseRulingById(Guid caseRulingId)
        {
            if (caseRulingId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseRulingId)));

            var domainCaseRuling = _caseRulingRepository.FindByGid(caseRulingId);
            if (domainCaseRuling == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseRulingId)));

            var caseRuling = new Domain.Service.Entities.CaseRuling()
            {
                CaseRulingId = domainCaseRuling.Gid,
                CaseId = _caseRepository.Find(domainCaseRuling.CaseId).Gid,
                HearingId = domainCaseRuling.HearingId.HasValue ? _hearingRepository.Find(domainCaseRuling.HearingId.Value).Gid : (Guid?)null,
                ActId = domainCaseRuling.ActId.HasValue ? _actRepository.Find(domainCaseRuling.ActId.Value).Gid : (Guid?)null,
                CaseRulingKindCode = _caseRulingKindRepository.GetNom(domainCaseRuling.CaseRulingKindId).Code
            };

            return caseRuling;
        }

        public List<Guid> GetCaseRulingIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.CaseRulings)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.CaseRulings.Select(i => i.Gid).ToList();
        }

        private void ValidateCaseRuling(Domain.Service.Entities.CaseRuling caseRuling, bool isUpdate)
        {
            this.ValidateId(caseRuling.CaseRulingId, _caseRulingRepository, isUpdate);

            if (caseRuling.CaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "CaseRuling.CaseId"));
            if (_caseRepository.FindByGid(caseRuling.CaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "CaseRuling.CaseId"));
            if (caseRuling.HearingId.HasValue)
            {
                if (caseRuling.HearingId.Value == default(Guid))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "CaseRuling.HearingId"));
                if (_hearingRepository.FindByGid(caseRuling.HearingId.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "CaseRuling.HearingId"));
            }

            if (caseRuling.ActId.HasValue)
            {
                if (caseRuling.ActId.Value == default(Guid))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "CaseRuling.ActId"));
                if (_actRepository.FindByGid(caseRuling.ActId.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "CaseRuling.ActId"));
            }

            if (string.IsNullOrWhiteSpace(caseRuling.CaseRulingKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "CaseRuling.CaseRulingKindCode"));
            if (!_caseRulingKindRepository.HasCode(caseRuling.CaseRulingKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "CaseRuling.CaseRulingKindCode"));
        }

        #endregion

        #region Side

        public Guid? InsertSide(Domain.Service.Entities.Side side)
        {
            ValidateSide(side, false);

            try
            {
                var domainSide = new Domain.Entities.Side();
                var domainSubject = new Subject();
                var domainPerson = new Domain.Entities.Person();
                var domainEntity = new Domain.Entities.Entity();

                #region LoadSubject

                var hasPerson = side.Person != null;
                var hasEntity = side.Entity != null;

                if (hasPerson)
                {
                    domainPerson.Gid = Guid.NewGuid();
                    domainPerson.Firstname = side.Person.Firstname;
                    domainPerson.Secondname = side.Person.Secondname;
                    domainPerson.Lastname = side.Person.Lastname;
                    domainPerson.EGN = side.Person.EGN;
                    domainPerson.Address = side.Person.Address;

                    domainSubject.Gid = domainPerson.Gid;
                    domainSubject.SubjectTypeId = (int)SubjectType.Person;
                    domainSubject.Name = string.Join(" ", domainPerson.Firstname, domainPerson.Secondname, domainPerson.Lastname);
                    domainSubject.Uin = domainPerson.EGN;
                }
                else if (hasEntity)
                {
                    domainEntity.Gid = Guid.NewGuid();
                    domainEntity.Name = side.Entity.Name;
                    domainEntity.Bulstat = side.Entity.Bulstat;
                    domainEntity.Address = side.Entity.Address;

                    domainSubject.Gid = domainEntity.Gid;
                    domainSubject.SubjectTypeId = (int)SubjectType.Entity;
                    domainSubject.Name = domainEntity.Name;
                    domainSubject.Uin = domainEntity.Bulstat;
                }

                if (hasPerson || hasEntity)
                {
                    _subjectRepository.Add(domainSubject);
                    _unitOfWorks[DbKey.Main].Save();

                    domainSide.SubjectId = domainSubject.SubjectId;
                }

                #endregion

                #region Load Side

                domainSide.Gid = side.SideId.HasValue ? side.SideId.Value : Guid.NewGuid();
                domainSide.CaseId = _caseRepository.FindByGid(side.CaseId).CaseId;
                domainSide.SideInvolvementKindId = _sideInvolvementKindRepository.GetNomIdByCode(side.SideInvolvementKindCode);
                domainSide.InsertDate = side.InsertDate;
                domainSide.ProceduralRelation = side.ProceduralRelation;
                domainSide.IsActive = side.IsActive;

                #endregion

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _sideRepository.Add(domainSide);

                    if (hasPerson)
                    {
                        domainPerson.SubjectId = domainSubject.SubjectId;
                        _personRepository.Add(domainPerson);
                    }

                    if (hasEntity)
                    {
                        domainEntity.SubjectId = domainSubject.SubjectId;
                        _entityRepository.Add(domainEntity);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainSide.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateSide(Domain.Service.Entities.Side side)
        {
            ValidateSide(side, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainSide = _sideRepository.FindByGid(side.SideId.Value);
                    var domainSubject = new Subject();
                    var domainPerson = new Domain.Entities.Person();
                    var domainEntity = new Domain.Entities.Entity();

                    var currentTime = DateTime.Now;

                    #region LoadSubject

                    var hasPerson = side.Person != null;
                    var hasEntity = side.Entity != null;

                    var hasDomainPerson = false;
                    var hasDomainEntity = false;

                    if (hasPerson)
                    {
                        if (domainSide.SubjectId.HasValue)
                        {
                            hasDomainPerson = _personRepository.Find(domainSide.SubjectId.Value) != null;
                        }

                        if (hasDomainPerson)
                        {
                            domainPerson = _personRepository.Find(domainSide.SubjectId.Value);
                            domainSubject = _subjectRepository.Find(domainPerson.SubjectId);
                        }
                        else
                        {
                            domainPerson.Gid = Guid.NewGuid();

                            domainSubject.Gid = domainPerson.Gid;
                            domainSubject.SubjectTypeId = (int)SubjectType.Person;
                        }

                        domainPerson.Firstname = side.Person.Firstname;
                        domainPerson.Secondname = side.Person.Secondname;
                        domainPerson.Lastname = side.Person.Lastname;
                        domainPerson.EGN = side.Person.EGN;
                        domainPerson.Address = side.Person.Address;

                        domainPerson.ModifyDate = currentTime;

                        domainSubject.Name = string.Join(" ", domainPerson.Firstname, domainPerson.Secondname, domainPerson.Lastname);
                        domainSubject.Uin = domainPerson.EGN;
                    }

                    if (hasEntity)
                    {
                        if (domainSide.SubjectId.HasValue)
                        {
                            hasDomainEntity = _entityRepository.Find(domainSide.SubjectId.Value) != null;
                        }

                        if (hasDomainEntity)
                        {
                            domainEntity = _entityRepository.Find(domainSide.SubjectId.Value);
                            domainSubject = _subjectRepository.Find(domainEntity.SubjectId);
                        }
                        else
                        {
                            domainEntity.Gid = Guid.NewGuid();

                            domainSubject.Gid = domainEntity.Gid;
                            domainSubject.SubjectTypeId = (int)SubjectType.Entity;
                        }

                        domainEntity.Name = side.Entity.Name;
                        domainEntity.Bulstat = side.Entity.Bulstat;
                        domainEntity.Address = side.Entity.Address;

                        domainEntity.ModifyDate = currentTime;

                        domainSubject.Name = domainEntity.Name;
                        domainSubject.Uin = domainEntity.Bulstat;
                    }

                    domainSubject.ModifyDate = currentTime;

                    #endregion

                    if (!hasDomainPerson && !hasDomainEntity)
                    {
                        _subjectRepository.Add(domainSubject);
                        _unitOfWorks[DbKey.Main].Save();
                    }

                    #region Load Side

                    domainSide.CaseId = _caseRepository.FindByGid(side.CaseId).CaseId;
                    domainSide.SubjectId = domainSubject.SubjectId;
                    domainSide.SideInvolvementKindId = _sideInvolvementKindRepository.GetNomIdByCode(side.SideInvolvementKindCode);
                    domainSide.InsertDate = side.InsertDate;
                    domainSide.ProceduralRelation = side.ProceduralRelation;
                    domainSide.IsActive = side.IsActive;

                    domainSide.ModifyDate = DateTime.Now;

                    #endregion

                    if (hasPerson && !hasDomainPerson)
                    {
                        domainPerson.SubjectId = domainSubject.SubjectId;
                        _personRepository.Add(domainPerson);
                    }

                    if (hasEntity && !hasDomainEntity)
                    {
                        domainEntity.SubjectId = domainSubject.SubjectId;
                        _entityRepository.Add(domainEntity);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainSide.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteSide(Guid sideId)
        {
            var domainSide = _sideRepository.FindByGid(sideId);
            if (domainSide == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(sideId)));

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    if (domainSide.SubjectId.HasValue)
                    {
                        var domainSubject = _subjectRepository.Find(domainSide.SubjectId.Value);

                        if (domainSubject.SubjectTypeId == 1)
                        {
                            var domainPerson = _personRepository.Find(domainSubject.SubjectId);
                            _personRepository.Remove(domainPerson);
                        }

                        if (domainSubject.SubjectTypeId == 2)
                        {
                            var domainEntity = _entityRepository.Find(domainSubject.SubjectId);
                            _entityRepository.Remove(domainEntity);
                        }

                        _subjectRepository.Remove(domainSubject);
                    }

                    _sideRepository.Remove(domainSide);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.Side GetSideById(Guid sideId)
        {
            if (sideId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(sideId)));

            var domainSide = _sideRepository.GetSide(sideId);

            if (domainSide == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(domainSide)));

            var side = new Domain.Service.Entities.Side()
            {
                SideId = domainSide.Gid,
                CaseId = _caseRepository.Find(domainSide.CaseId).Gid,
                SideInvolvementKindCode = _sideInvolvementKindRepository.GetNom(domainSide.SideInvolvementKindId).Code,
                ProceduralRelation = domainSide.ProceduralRelation,
                IsActive = domainSide.IsActive,
                InsertDate = domainSide.InsertDate
            };

            var hasPerson = domainSide.Subject.Person != null;
            var hasEntity = domainSide.Subject.Entity != null;

            if (hasPerson)
            {
                side.Person = new Domain.Service.Entities.Person()
                {
                    Firstname = domainSide.Subject.Person.Firstname,
                    Secondname = domainSide.Subject.Person.Secondname,
                    Lastname = domainSide.Subject.Person.Lastname,
                    EGN = domainSide.Subject.Person.EGN,
                    Address = domainSide.Subject.Person.Address
                };
            }
            else if (hasEntity)
            {
                side.Entity = new Domain.Service.Entities.Entity()
                {
                    Name = domainSide.Subject.Entity.Name,
                    Bulstat = domainSide.Subject.Entity.Bulstat,
                    Address = domainSide.Subject.Entity.Address
                };
            }

            return side;
        }

        public List<Guid> GetSideIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.Sides)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.Sides.Select(i => i.Gid).ToList();
        }

        private void ValidateSide(Domain.Service.Entities.Side side, bool isUpdate)
        {
            this.ValidateId(side.SideId, _sideRepository, isUpdate);

            if (side.CaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Side.CaseId"));
            if (_caseRepository.FindByGid(side.CaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Side.CaseId"));
            if (string.IsNullOrWhiteSpace(side.SideInvolvementKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Side.SideInvolvementKindCode"));
            if (!_sideInvolvementKindRepository.HasCode(side.SideInvolvementKindCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Side.SideInvolvementKindCode"));
            if ((side.Person == null && side.Entity == null) || (side.Person != null && side.Entity != null))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Side.Person || Side.Entity"));
            else
            {
                if (side.Person != null)
                    ValidatePerson(side.Person);
                if (side.Entity != null)
                    ValidateEntity(side.Entity);
            }
        }

        private void ValidatePerson(Domain.Service.Entities.Person person)
        {
            if (string.IsNullOrWhiteSpace(person.Firstname))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Person.Firstname"));
            if (string.IsNullOrWhiteSpace(person.Lastname))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Person.Lastname"));
        }

        private void ValidateEntity(Domain.Service.Entities.Entity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Entity.Name"));
        }

        #endregion

        #region OutgoingDocuments

        public Guid? InsertOutgoingDocument(Domain.Service.Entities.OutgoingDocument outgoingDocument)
        {
            ValidateOutgoingDocument(outgoingDocument, false);

            try
            {
                var domainOutgoingDocument = new Domain.Entities.OutgoingDocument()
                {
                    Gid = outgoingDocument.OutgoingDocumentId.HasValue ? outgoingDocument.OutgoingDocumentId.Value : Guid.NewGuid(),
                    CaseId = outgoingDocument.CaseId.HasValue ? _caseRepository.FindByGid(outgoingDocument.CaseId.Value).CaseId : (long?)null,
                    OutgoingNumber = outgoingDocument.OutgoingNumber,
                    OutgoingDate = outgoingDocument.OutgoingDate,
                    OutgoingDocumentTypeId = _outgoingDocumentTypeRepository.GetNomIdByCode(outgoingDocument.OutgoingDocumentTypeCode)
                };

                #region LoadSubject

                var domainSubject = new Subject();
                var domainPerson = new Domain.Entities.Person();
                var domainEntity = new Domain.Entities.Entity();

                var hasPerson = outgoingDocument.Person != null;
                var hasEntity = outgoingDocument.Entity != null;

                if (hasPerson)
                {
                    domainPerson.Gid = Guid.NewGuid();
                    domainPerson.Firstname = outgoingDocument.Person.Firstname;
                    domainPerson.Secondname = outgoingDocument.Person.Secondname;
                    domainPerson.Lastname = outgoingDocument.Person.Lastname;
                    domainPerson.EGN = outgoingDocument.Person.EGN;
                    domainPerson.Address = outgoingDocument.Person.Address;

                    domainSubject.Gid = domainPerson.Gid;
                    domainSubject.SubjectTypeId = (int)SubjectType.Person;
                    domainSubject.Name = string.Join(" ", domainPerson.Firstname, domainPerson.Secondname, domainPerson.Lastname);
                    domainSubject.Uin = domainPerson.EGN;
                }
                else if (hasEntity)
                {
                    domainEntity.Gid = Guid.NewGuid();
                    domainEntity.Name = outgoingDocument.Entity.Name;
                    domainEntity.Bulstat = outgoingDocument.Entity.Bulstat;
                    domainEntity.Address = outgoingDocument.Entity.Address;

                    domainSubject.Gid = domainEntity.Gid;
                    domainSubject.SubjectTypeId = (int)SubjectType.Entity;
                    domainSubject.Name = domainEntity.Name;
                    domainSubject.Uin = domainEntity.Bulstat;
                }

                if (hasPerson || hasEntity)
                {
                    _subjectRepository.Add(domainSubject);
                    _unitOfWorks[DbKey.Main].Save();

                    domainOutgoingDocument.SubjectId = domainSubject.SubjectId;
                }

                #endregion

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _outgoingDocumentRepository.Add(domainOutgoingDocument);

                    if (hasPerson)
                    {
                        domainPerson.SubjectId = domainSubject.SubjectId;
                        _personRepository.Add(domainPerson);
                    }

                    if (hasEntity)
                    {
                        domainEntity.SubjectId = domainSubject.SubjectId;
                        _entityRepository.Add(domainEntity);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainOutgoingDocument.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateOutgoingDocument(Domain.Service.Entities.OutgoingDocument outgoingDocument)
        {
            ValidateOutgoingDocument(outgoingDocument, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainOutgoingDocument = _outgoingDocumentRepository.FindByGid(outgoingDocument.OutgoingDocumentId.Value);

                    #region Load OutgoingDocument

                    domainOutgoingDocument.CaseId = outgoingDocument.CaseId.HasValue ? _caseRepository.FindByGid(outgoingDocument.CaseId.Value).CaseId : (long?)null;

                    var domainSubject = new Subject();
                    var domainPerson = new Domain.Entities.Person();
                    var domainEntity = new Domain.Entities.Entity();

                    var currentTime = DateTime.Now;

                    #region LoadSubject

                    var hasPerson = outgoingDocument.Person != null;
                    var hasEntity = outgoingDocument.Entity != null;

                    var hasDomainPerson = false;
                    var hasDomainEntity = false;

                    if (hasPerson)
                    {
                        if (domainOutgoingDocument.SubjectId.HasValue)
                        {
                            hasDomainPerson = _personRepository.Find(domainOutgoingDocument.SubjectId.Value) != null;
                        }

                        if (hasDomainPerson)
                        {
                            domainPerson = _personRepository.Find(domainOutgoingDocument.SubjectId.Value);
                            domainSubject = _subjectRepository.Find(domainPerson.SubjectId);
                        }
                        else
                        {
                            domainPerson.Gid = Guid.NewGuid();

                            domainSubject.Gid = domainPerson.Gid;
                            domainSubject.SubjectTypeId = (int)SubjectType.Person;
                        }

                        domainPerson.Firstname = outgoingDocument.Person.Firstname;
                        domainPerson.Secondname = outgoingDocument.Person.Secondname;
                        domainPerson.Lastname = outgoingDocument.Person.Lastname;
                        domainPerson.EGN = outgoingDocument.Person.EGN;
                        domainPerson.Address = outgoingDocument.Person.Address;

                        domainPerson.ModifyDate = currentTime;

                        domainSubject.Name = string.Join(" ", domainPerson.Firstname, domainPerson.Secondname, domainPerson.Lastname);
                        domainSubject.Uin = domainPerson.EGN;
                    }

                    if (hasEntity)
                    {
                        if (domainOutgoingDocument.SubjectId.HasValue)
                        {
                            hasDomainEntity = _entityRepository.Find(domainOutgoingDocument.SubjectId.Value) != null;
                        }

                        if (hasDomainEntity)
                        {
                            domainEntity = _entityRepository.Find(domainOutgoingDocument.SubjectId.Value);
                            domainSubject = _subjectRepository.Find(domainEntity.SubjectId);
                        }
                        else
                        {
                            domainEntity.Gid = Guid.NewGuid();

                            domainSubject.Gid = domainEntity.Gid;
                            domainSubject.SubjectTypeId = (int)SubjectType.Entity;
                        }

                        domainEntity.Name = outgoingDocument.Entity.Name;
                        domainEntity.Bulstat = outgoingDocument.Entity.Bulstat;
                        domainEntity.Address = outgoingDocument.Entity.Address;

                        domainEntity.ModifyDate = currentTime;

                        domainSubject.Name = domainEntity.Name;
                        domainSubject.Uin = domainEntity.Bulstat;
                    }

                    domainSubject.ModifyDate = currentTime;

                    #endregion

                    if (!hasDomainPerson && !hasDomainEntity)
                    {
                        _subjectRepository.Add(domainSubject);
                        _unitOfWorks[DbKey.Main].Save();
                    }

                    domainOutgoingDocument.SubjectId = domainSubject.SubjectId;
                    domainOutgoingDocument.OutgoingNumber = outgoingDocument.OutgoingNumber;
                    domainOutgoingDocument.OutgoingDate = outgoingDocument.OutgoingDate;
                    domainOutgoingDocument.OutgoingDocumentTypeId = _outgoingDocumentTypeRepository.GetNomIdByCode(outgoingDocument.OutgoingDocumentTypeCode);

                    domainOutgoingDocument.ModifyDate = DateTime.Now;

                    #endregion

                    if (hasPerson && !hasDomainPerson)
                    {
                        domainPerson.SubjectId = domainSubject.SubjectId;
                        _personRepository.Add(domainPerson);
                    }

                    if (hasEntity && !hasDomainEntity)
                    {
                        domainEntity.SubjectId = domainSubject.SubjectId;
                        _entityRepository.Add(domainEntity);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainOutgoingDocument.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteOutgoingDocument(Guid outgoingDocumentId)
        {
            var domainOutgoingDocument = _outgoingDocumentRepository.FindByGid(outgoingDocumentId);
            if (domainOutgoingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(outgoingDocumentId)));

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    if (domainOutgoingDocument.SubjectId.HasValue)
                    {
                        var domainSubject = _subjectRepository.Find(domainOutgoingDocument.SubjectId.Value);

                        if (domainSubject.SubjectTypeId == 1)
                        {
                            var domainPerson = _personRepository.Find(domainSubject.SubjectId);
                            _personRepository.Remove(domainPerson);
                        }

                        if (domainSubject.SubjectTypeId == 2)
                        {
                            var domainEntity = _entityRepository.Find(domainSubject.SubjectId);
                            _entityRepository.Remove(domainEntity);
                        }

                        _subjectRepository.Remove(domainSubject);
                    }

                    _outgoingDocumentRepository.Remove(domainOutgoingDocument);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.OutgoingDocument GetOutgoingDocumentById(Guid outgoingDocumentId)
        {
            if (outgoingDocumentId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(outgoingDocumentId)));

            var domainOutgoingDocument = _outgoingDocumentRepository.GetOutgoingDocument(outgoingDocumentId);
            if (domainOutgoingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(outgoingDocumentId)));

            var outGoingDocument = new Domain.Service.Entities.OutgoingDocument()
            {
                OutgoingDocumentId = domainOutgoingDocument.Gid,
                CaseId = domainOutgoingDocument.CaseId.HasValue ? _caseRepository.Find(domainOutgoingDocument.CaseId.Value).Gid : (Guid?)null,
                OutgoingNumber = domainOutgoingDocument.OutgoingNumber,
                OutgoingDate = domainOutgoingDocument.OutgoingDate,
                OutgoingDocumentTypeCode = _outgoingDocumentTypeRepository.GetNom(domainOutgoingDocument.OutgoingDocumentTypeId).Code
            };

            var hasPerson = domainOutgoingDocument.Subject.Person != null;
            var hasEntity = domainOutgoingDocument.Subject.Entity != null;

            if (hasPerson)
            {
                outGoingDocument.Person = new Domain.Service.Entities.Person()
                {
                    Firstname = domainOutgoingDocument.Subject.Person.Firstname,
                    Secondname = domainOutgoingDocument.Subject.Person.Secondname,
                    Lastname = domainOutgoingDocument.Subject.Person.Lastname,
                    EGN = domainOutgoingDocument.Subject.Person.EGN,
                    Address = domainOutgoingDocument.Subject.Person.Address
                };
            }
            else if (hasEntity)
            {
                outGoingDocument.Entity = new Domain.Service.Entities.Entity()
                {
                    Name = domainOutgoingDocument.Subject.Entity.Name,
                    Bulstat = domainOutgoingDocument.Subject.Entity.Bulstat,
                    Address = domainOutgoingDocument.Subject.Entity.Address
                };
            }

            return outGoingDocument;
        }

        public List<Guid> GetOutgoingDocumentIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.OutgoingDocuments)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.OutgoingDocuments.Select(i => i.Gid).ToList();
        }

        private void ValidateOutgoingDocument(Domain.Service.Entities.OutgoingDocument outgoingDocument, bool isUpdate)
        {
            this.ValidateId(outgoingDocument.OutgoingDocumentId, _outgoingDocumentRepository, isUpdate);

            if (outgoingDocument.CaseId.HasValue)
            {
                if (outgoingDocument.CaseId.Value == default(Guid))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "OutgoingDocument.CaseId"));
                if (_caseRepository.FindByGid(outgoingDocument.CaseId.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "OutgoingDocument.CaseId"));
            }

            if (outgoingDocument.OutgoingNumber == default(int))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "OutgoingDocument.OutgoingNumber"));
            if (outgoingDocument.OutgoingDate.HasValue && outgoingDocument.OutgoingDate.Value == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "OutgoingDocument.OutgoingDate"));
            if (string.IsNullOrWhiteSpace(outgoingDocument.OutgoingDocumentTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "OutgoingDocument.OutgoingDocumentTypeCode"));
            if (!_outgoingDocumentTypeRepository.HasCode(outgoingDocument.OutgoingDocumentTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "OutgoingDocument.OutgoingDocumentTypeCode"));
            if ((outgoingDocument.Person == null && outgoingDocument.Entity == null) || (outgoingDocument.Person != null && outgoingDocument.Entity != null))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "OutgoingDocument.Person || OutgoingDocument.Entity"));
            else
            {
                if (outgoingDocument.Person != null)
                    ValidatePerson(outgoingDocument.Person);
                if (outgoingDocument.Entity != null)
                    ValidateEntity(outgoingDocument.Entity);
            }
        }

        #endregion

        #region OutgoingDocumentFiles

        public Guid? InsertOutgoingDocumentFile(OutgoingDocumentFile outgoingDocumentFile)
        {
            ValidateOutgoingDocumentFile(outgoingDocumentFile, false);

            try
            {
                var domainOutgoingDocument = _outgoingDocumentRepository.FindByGid(outgoingDocumentFile.OutgoingDocumentId);
                return this.UploadFile(outgoingDocumentFile.OutgoingDocumentFileId, outgoingDocumentFile.OutgoingDocumentContent, outgoingDocumentFile.OutgoingDocumentMimeType, FileType.Outgoing, domainOutgoingDocument.CreateDate, domainOutgoingDocument, value => domainOutgoingDocument.BlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateOutgoingDocumentFile(OutgoingDocumentFile outgoingDocumentFile)
        {
            ValidateOutgoingDocumentFile(outgoingDocumentFile, true);

            try
            {
                var domainOutgoingDocument = _outgoingDocumentRepository.FindByGid(outgoingDocumentFile.OutgoingDocumentId);
                return this.UploadFile(outgoingDocumentFile.OutgoingDocumentFileId, outgoingDocumentFile.OutgoingDocumentContent, outgoingDocumentFile.OutgoingDocumentMimeType, FileType.Outgoing, domainOutgoingDocument.CreateDate, domainOutgoingDocument, value => domainOutgoingDocument.BlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteOutgoingDocumentFile(Guid outgoingDocumentId)
        {
            var domainOutgoingDocument = _outgoingDocumentRepository.FindByGid(outgoingDocumentId);
            if (domainOutgoingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(outgoingDocumentId)));

            return this.DeleteFile(domainOutgoingDocument, domainOutgoingDocument.BlobKey, value => domainOutgoingDocument.BlobKey = value);
        }

        public OutgoingDocumentFile GetOutgoingDocumentFileById(Guid outgoingDocumentFileId)
        {
            if (outgoingDocumentFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(outgoingDocumentFileId)));

            var domainOutgoingDocument = _outgoingDocumentRepository.SetWithoutIncludes()
                .Include(a => a.Blob)
                .FirstOrDefault(a => a.BlobKey == outgoingDocumentFileId);
            if (domainOutgoingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(outgoingDocumentFileId)));

            var outgoingDocumentFile = new OutgoingDocumentFile()
            {
                OutgoingDocumentFileId = domainOutgoingDocument.BlobKey,
                OutgoingDocumentId = domainOutgoingDocument.Gid,
                OutgoingDocumentMimeType = _blobStorageRepository.GetMimeType(domainOutgoingDocument.Blob.FileName),
                OutgoingDocumentContent = _blobStorageRepository.GetFileContent(domainOutgoingDocument.BlobKey.Value),
            };

            return outgoingDocumentFile;
        }

        public Guid? GetOutgoingDocumentFileIdentifierByOutgoingDocumentId(Guid outgoingDocumentId)
        {
            var domainOutgoingDocument = _outgoingDocumentRepository.FindByGid(outgoingDocumentId);
            if (domainOutgoingDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(outgoingDocumentId)));

            return domainOutgoingDocument.BlobKey;
        }

        private void ValidateOutgoingDocumentFile(OutgoingDocumentFile outgoingDocumentFile, bool isUpdate)
        {
            var domainOutgoingDocument = _outgoingDocumentRepository.FindByGid(outgoingDocumentFile.OutgoingDocumentId);

            this.ValidateFile(outgoingDocumentFile.OutgoingDocumentFileId, outgoingDocumentFile.OutgoingDocumentId, outgoingDocumentFile.OutgoingDocumentContent, outgoingDocumentFile.OutgoingDocumentMimeType, domainOutgoingDocument?.BlobKey, domainOutgoingDocument, isUpdate);
        }

        #endregion

        #region LawyerAssignment

        public Guid? InsertLawyerAssignment(Domain.Service.Entities.LawyerAssignment lawyerAssignment)
        {
            if (upgradedEpep)
            {
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.Unknown, upgradedEpepObsoleteMethod));
            }

            ValidateLawyerAssignment(lawyerAssignment, false);

            try
            {
                var domainLawyerAssignment = new Domain.Entities.LawyerAssignment()
                {
                    Gid = lawyerAssignment.LawyerAssignmentId.HasValue ? lawyerAssignment.LawyerAssignmentId.Value : Guid.NewGuid(),
                    Date = lawyerAssignment.Date,
                    SideId = lawyerAssignment.SideId.HasValue ? _sideRepository.FindByGid(lawyerAssignment.SideId.Value).SideId : (long?)null,
                    LawyerId = _lawyerRegistrationRepository.FindByGid(lawyerAssignment.LawyerRegistrationId).LawyerId,
                    IsActive = lawyerAssignment.IsActive
                };

                var lawyerRegistration = _lawyerRegistrationRepository.GetLawyerRegistrationByLawyerId(domainLawyerAssignment.LawyerId);

                if (lawyerRegistration != null)
                {
                    var user = _userRepository.Find(lawyerRegistration.LawyerRegistrationId);

                    if (user != null)
                    {
                        var userEmail = user.Username;

                        if (lawyerAssignment.SideId.HasValue)
                        {
                            var domainSide = _sideRepository.FindByGid(lawyerAssignment.SideId.Value);
                            var domainCase = _caseRepository.Find(domainSide.CaseId);

                            if (domainCase != null && domainCase.Court != null)
                            {
                                if (SendCaseAccessMail)
                                {
                                    domainLawyerAssignment.GetCaseAccess(userEmail, domainCase.Abbreviation, domainCase.Court.Name);
                                }
                            }
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _lawyerAssignmentRepository.Add(domainLawyerAssignment);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainLawyerAssignment.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateLawyerAssignment(Domain.Service.Entities.LawyerAssignment lawyerAssignment)
        {
            ValidateLawyerAssignment(lawyerAssignment, true);

            try
            {
                var domainLawyerAssignment = _lawyerAssignmentRepository.FindByGid(lawyerAssignment.LawyerAssignmentId.Value);

                if (lawyerAssignment.SideId == null)
                {
                    var lawyerRegistration = _lawyerRegistrationRepository.GetLawyerRegistrationByLawyerId(domainLawyerAssignment.LawyerId);

                    if (lawyerRegistration != null)
                    {
                        var user = _userRepository.Find(lawyerRegistration.LawyerRegistrationId);

                        if (user != null)
                        {
                            var userEmail = user.Username;

                            if (lawyerAssignment.SideId.HasValue)
                            {
                                var domainSide = _sideRepository.FindByGid(lawyerAssignment.SideId.Value);
                                var domainCase = _caseRepository.Find(domainSide.CaseId);

                                if (domainCase != null && domainCase.Court != null)
                                {
                                    domainLawyerAssignment.DenyCaseAccess(userEmail, domainCase.Abbreviation, domainCase.Court.Name);
                                }
                            }
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    domainLawyerAssignment.Date = lawyerAssignment.Date;
                    domainLawyerAssignment.SideId = lawyerAssignment.SideId.HasValue ? _sideRepository.FindByGid(lawyerAssignment.SideId.Value).SideId : (long?)null;
                    domainLawyerAssignment.LawyerId = _lawyerRegistrationRepository.FindByGid(lawyerAssignment.LawyerRegistrationId).LawyerId;
                    domainLawyerAssignment.IsActive = lawyerAssignment.IsActive;

                    domainLawyerAssignment.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainLawyerAssignment.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }





        public bool DeleteLawyerAssignment(Guid lawyerAssignmentId)
        {
            var domainLawyerAssignment = _lawyerAssignmentRepository.FindByGid(lawyerAssignmentId);
            if (domainLawyerAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerAssignmentId)));

            try
            {
                var lawyerRegistration = _lawyerRegistrationRepository.GetLawyerRegistrationByLawyerId(domainLawyerAssignment.LawyerId);

                if (lawyerRegistration != null)
                {
                    var user = _userRepository.Find(lawyerRegistration.LawyerRegistrationId);

                    if (user != null)
                    {
                        var userEmail = user.Username;

                        if (domainLawyerAssignment.SideId.HasValue)
                        {
                            var domainSide = _sideRepository.Find(domainLawyerAssignment.SideId.Value);
                            var domainCase = _caseRepository.Find(domainSide.CaseId);

                            if (domainCase != null && domainCase.Court != null)
                            {
                                domainLawyerAssignment.DenyCaseAccess(userEmail, domainCase.Abbreviation, domainCase.Court.Name);
                            }
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _lawyerAssignmentRepository.Remove(domainLawyerAssignment);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.LawyerAssignment GetLawyerAssignmentById(Guid lawyerAssignmentId)
        {
            if (lawyerAssignmentId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerAssignmentId)));

            var domainLawyerAssignment = _lawyerAssignmentRepository.FindByGid(lawyerAssignmentId);
            if (domainLawyerAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerAssignmentId)));

            var lawyerAssignment = new Domain.Service.Entities.LawyerAssignment()
            {
                LawyerAssignmentId = domainLawyerAssignment.Gid,
                Date = domainLawyerAssignment.Date,
                SideId = domainLawyerAssignment.SideId.HasValue ? _sideRepository.Find(domainLawyerAssignment.SideId.Value).Gid : (Guid?)null,
                LawyerRegistrationId = _lawyerRegistrationRepository.GetLawyerRegistrationByLawyerId(domainLawyerAssignment.LawyerId).Gid,
                IsActive = domainLawyerAssignment.IsActive
            };

            return lawyerAssignment;
        }

        public List<Guid> GetLawyerAssignmentIdentifiersBySideId(Guid sideId)
        {
            var domainSide = _sideRepository.SetWithoutIncludes()
                .Include(i => i.LawyerAssignments)
                .FirstOrDefault(i => i.Gid == sideId);

            if (domainSide == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(sideId)));

            return domainSide.LawyerAssignments.Select(i => i.Gid).ToList();
        }

        private void ValidateLawyerAssignment(Domain.Service.Entities.LawyerAssignment assignment, bool isUpdate)
        {
            this.ValidateId(assignment.LawyerAssignmentId, _lawyerAssignmentRepository, isUpdate);

            if (assignment.Date == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "LawyerAssignment.Date"));
            if (assignment.LawyerRegistrationId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "LawyerAssignment.LawyerRegistrationId"));
            if (_lawyerRegistrationRepository.FindByGid(assignment.LawyerRegistrationId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "LawyerAssignment.LawyerRegistrationId"));
            if (assignment.SideId.HasValue)
            {
                if (assignment.SideId == default(Guid))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "LawyerAssignment.SideId"));
                if (_sideRepository.FindByGid(assignment.SideId.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "LawyerAssignment.SideId"));
            }
        }

        #endregion

        #region Summons

        public List<Guid> GetAllServedSummonsByCourt(string courtcode, DateTime? from, DateTime? to)
        {
            try
            {
                var result = new List<Guid>();

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainSummons = _summonRepository.GetSummonsByCourtCode(courtcode).Where(s => s.ReadTime > from && s.ReadTime < to);

                    foreach (var domainSummon in domainSummons)
                    {
                        result.Add(domainSummon.Gid);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public DateTime GetSummonsServedTimestamp(Guid guid)
        {
            try
            {
                var result = new DateTime();

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var summon = _summonRepository.FindByGid(guid);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                    if (summon.ReadTime == null)
                    {
                        throw new FaultException(new FaultReason("Summon not served."));
                    }
                    result = (DateTime)summon.ReadTime;
                    return result;
                }
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? MarkSummonAsRead(Guid guid, DateTime? date)
        {
            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainSummon = _summonRepository.FindByGid(guid);
                    if (domainSummon == null)
                    {
                        throw new FaultException(new FaultReason("Summon not found."));
                    }
                    if (domainSummon.CourtReadTime == null)
                    {
                        domainSummon.CourtReadTime = date ?? DateTime.Now;
                        domainSummon.ModifyDate = DateTime.Now;

                        _unitOfWorks[DbKey.Main].Save();
                        transaction.Commit();

                        return domainSummon.Gid;
                    }
                    else
                    {
                        throw new FaultException(new FaultReason("Summon is already set as read."));
                    }
                }
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? MarkSummonAsCourtRead(Guid summonId, DateTime courtReadDate, string courtDescription)
        {
            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainSummon = _summonRepository.FindByGid(summonId);
                    if (domainSummon == null)
                    {
                        throw new FaultException(new FaultReason("Summon not found."));
                    }

                    if (domainSummon.ReadTime == null)
                    {
                        domainSummon.ReadTime = courtReadDate;
                        domainSummon.IsRead = true;
                        domainSummon.ModifyDate = DateTime.Now;
                        domainSummon.CourtReadDescription = courtDescription;

                        _unitOfWorks[DbKey.Main].Save();
                        transaction.Commit();

                        return domainSummon.Gid;
                    }
                    else
                    {
                        throw new FaultException(new FaultReason("Summon is already set as read."));
                    }
                }
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public List<Guid> GetAllReadSummonsByCourt(string courtcode, DateTime? from, DateTime? to)
        {
            try
            {
                var result = new List<Guid>();

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainSummons = _summonRepository.GetSummonsByCourtCode(courtcode).Where(s => s.ReadTime > from && s.ReadTime < to);

                    foreach (var domainSummon in domainSummons)
                    {
                        result.Add(domainSummon.Gid);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public DateTime GetSummonsReadTimestamp(Guid guid)
        {
            try
            {
                var result = new DateTime();

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var summon = _summonRepository.FindByGid(guid);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                    if (summon.ReadTime == null)
                    {
                        throw new FaultException(new FaultReason("Summon not read."));
                    }
                    result = (DateTime)summon.ReadTime;
                    return result;
                }
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public SummonReadTimeResult GetSummonsReadTimestampV3(Guid guid)
        {
            try
            {
                var result = new SummonReadTimeResult();

                var summon = _summonRepository.FindByGid(guid);
                if (summon == null)
                {
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, $"Summon.Gid={guid}"));
                }

                if (summon.ReadTime.HasValue)
                {
                    result.IsRead = true;
                    result.ReadDate = summon.ReadTime.Value;
                }
                else
                {

                    result.IsRead = false;
                    if (summon.AddresseeUserRegistrationId > 0)
                    {
                        // Проверява с какъв достъп е получателя на призовката в делото
                        var _assignmentRole = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.UserAssignment>()
                                                                                  .Where(x => x.UserRegistrationId == summon.AddresseeUserRegistrationId)
                                                                                  .Where(x => x.SideId == summon.SideId && x.IsActive)
                                                                                  .Select(x => x.AssignmentRoleId)
                                                                                  .FirstOrDefault();

                        switch (_assignmentRole)
                        {
                            case EpepConstants.UserAssignmentRoles.Lawyer:
                                {
                                    var vacationForSummon = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.UserVacation>()
                                                                               .Where(x => x.UserId == summon.AddresseeUserRegistrationId)
                                                                               .Where(x => (x.DateFrom <= summon.ArrivedTime) && (x.DateTo >= summon.ArrivedTime))
                                                                               .Where(x => x.DateExpire == null)
                                                                               .OrderBy(x => x.DateTo)
                                                                               .FirstOrDefault();
                                    if (vacationForSummon != null)
                                    {
                                        result.VacationEndDate = vacationForSummon.DateTo;
                                    }
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
                return result;
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public List<Tuple<Guid, DateTime>> GetAllReadSummons()
        {
            try
            {
                var result = new List<Tuple<Guid, DateTime>>();

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainSummons = _summonRepository.GetAllReadSummons();

                    foreach (var domainSummon in domainSummons)
                    {
                        result.Add(
                            new Tuple<Guid, DateTime>(domainSummon.Gid, domainSummon.ReadTime ?? DateTime.Now));
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public List<Guid> GetReadSummonsForCertainDay(DateTime date)
        {
            try
            {
                var summonIds = new List<Guid>();

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainSummons = _summonRepository.GetReadSummonsForCertainDay(date);

                    foreach (var domainSummon in domainSummons)
                    {
                        summonIds.Add(domainSummon.Gid);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return summonIds;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public byte[] GetSummonReportDocument(Guid summonId)
        {
            var domainSummon = _summonRepository.FindByGid(summonId);

            if (domainSummon == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "summonId"));

            ValidateReadSummon(domainSummon);

            try
            {
                var content = _blobStorageRepository.GetFileContent(domainSummon.ReportBlobKey.Value);
                return content;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        private List<string> GetNofiticationMailsForSide(Domain.Entities.Upgrade.UserRegistration user, long caseId)
        {
            var summonedUser = user;
            if (user.UserTypeId == UpgradeEpepConstants.UserTypes.Organization)
            {
                var orgUserEmails = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.UserOrganizationCase>()
                                    .Where(x => x.OrganizationUserId == user.Id)
                                    .Where(x => x.CaseId == caseId && x.IsActive && x.NotificateUser)
                                    .Select(x =>
                                        x.UserRegistration.NotificationEmail ?? x.UserRegistration.Email
                                    )
                                    .ToList();
                if (orgUserEmails.Any())
                {
                    return orgUserEmails;
                }
                var orgRepresentativesEmails = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.UserRegistration>()
                                    .Where(x => x.OrganizationUserId == user.Id && x.UserTypeId == UpgradeEpepConstants.UserTypes.OrganizationRepresentative)
                                    .Where(x => x.IsActive)
                                    .Select(x =>
                                        x.NotificationEmail ?? x.Email
                                    )
                                    .ToList();
                return orgRepresentativesEmails;
            }


            if (!string.IsNullOrEmpty(summonedUser.NotificationEmail))
            {
                return new List<string>() { summonedUser.NotificationEmail };
            }
            else
            {
                return new List<string>() { summonedUser.Email };
            }

        }

        public Guid? InsertSummon(Domain.Service.Entities.Summon summon, Guid userId)
        {
            ValidateSummon(summon, false);

            var side = _sideRepository.FindByGid(summon.SideId);

            List<string> emails;
            long? addresseeUserId = null;
            if (upgradedEpep)
            {
                var userRegistration = upgradeRepository.FindByGid<Domain.Entities.Upgrade.UserRegistration>(userId);
                if (userRegistration == null)
                {
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "userId"));
                }
                addresseeUserId = userRegistration.Id;
                emails = GetNofiticationMailsForSide(userRegistration, side.CaseId);
            }
            else
            {
                var user = _userRepository.FindByGid(userId);
                if (user == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "userId"));
                emails = new List<string>() { user.Username };
            }
            try
            {
                var domainSummon = new Domain.Entities.Summon()
                {
                    Gid = summon.SummonId.HasValue ? summon.SummonId.Value : Guid.NewGuid(),
                    SideId = side.SideId,
                    SummonTypeId = _summonTypeRepository.GetNomIdByCode(summon.SummonTypeCode),
                    SummonKind = summon.SummonKind,
                    Number = summon.Number,
                    DateCreated = summon.DateCreated,
                    DateServed = summon.DateServed,
                    Addressee = summon.Addressee,
                    AddresseeUserRegistrationId = addresseeUserId,
                    Address = summon.Address,
                    Subject = summon.Subject,
                    ArrivedTime = DateTime.Now
                };

                switch (summon.SummonTypeCode)
                {
                    case "1":
                        var act = _actRepository.FindByGid(summon.ParentId);
                        domainSummon.ActId = act.ActId;
                        break;
                    case "2":
                        var appeal = _appealRepository.FindByGid(summon.ParentId);
                        domainSummon.AppealId = appeal.AppealId;
                        break;
                    case "3":
                        var c = _caseRepository.FindByGid(summon.ParentId);
                        domainSummon.CaseId = c.CaseId;
                        break;
                    case "4":
                        var hearing = _hearingRepository.FindByGid(summon.ParentId);
                        domainSummon.HearingId = hearing.HearingId;
                        break;
                    default:
                        break;
                }

                if (emails != null)
                {
                    foreach (var email in emails)
                    {
                        domainSummon.SendSummonNotification(email);
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _summonRepository.Add(domainSummon);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainSummon.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateSummon(Domain.Service.Entities.Summon summon)
        {
            ValidateSummon(summon, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainSummon = _summonRepository.FindByGid(summon.SummonId.Value);

                    domainSummon.SideId = _sideRepository.FindByGid(summon.SideId).SideId;

                    domainSummon.SummonTypeId = _summonTypeRepository.GetNomIdByCode(summon.SummonTypeCode);
                    domainSummon.SummonKind = summon.SummonKind;
                    domainSummon.Number = summon.Number;
                    domainSummon.DateCreated = summon.DateCreated;
                    domainSummon.DateServed = summon.DateServed;
                    domainSummon.Addressee = summon.Addressee;
                    domainSummon.Address = summon.Address;
                    domainSummon.Subject = summon.Subject;

                    domainSummon.ModifyDate = DateTime.Now;

                    switch (summon.SummonTypeCode)
                    {
                        case "1":
                            var act = _actRepository.FindByGid(summon.ParentId);
                            domainSummon.ActId = act.ActId;
                            break;
                        case "2":
                            var appeal = _appealRepository.FindByGid(summon.ParentId);
                            domainSummon.AppealId = appeal.AppealId;
                            break;
                        case "3":
                            var c = _caseRepository.FindByGid(summon.ParentId);
                            domainSummon.CaseId = c.CaseId;
                            break;
                        case "4":
                            var hearing = _hearingRepository.FindByGid(summon.ParentId);
                            domainSummon.HearingId = hearing.HearingId;
                            break;
                        default:
                            break;
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainSummon.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteSummon(Guid summonId)
        {
            return this.DeleteDomainEntity(summonId, _summonRepository);
        }

        public void ActivateSummonsAccess(Guid userId, Guid caseId)
        {
            var user = _userRepository.FindByGid(userId);

            if (user != null)
            {
                var userEmail = user.Username;

                var domainCase = _caseRepository.SetWithoutIncludes()
                    .Where(e => e.Gid == caseId)
                    .Include(e => e.Court)
                    .SingleOrDefault();

                if (domainCase != null && domainCase.Court != null)
                {
                    user.ActivateSummonsAccess(userEmail, domainCase.Abbreviation, domainCase.Court.Name);
                    _unitOfWorks[DbKey.Main].Save();
                }
            }
        }

        public void DeactivateSummonsAccess(Guid userId, Guid caseId)
        {
            var user = _userRepository.FindByGid(userId);

            if (user != null)
            {
                var userEmail = user.Username;

                var domainCase = _caseRepository.SetWithoutIncludes()
                    .Where(e => e.Gid == caseId)
                    .Include(e => e.Court)
                    .SingleOrDefault();

                if (domainCase != null && domainCase.Court != null)
                {
                    user.DectivateSummonsAccess(userEmail, domainCase.Abbreviation, domainCase.Court.Name);
                    _unitOfWorks[DbKey.Main].Save();
                }
            }
        }

        public Domain.Service.Entities.Summon GetSummonById(Guid summonId)
        {
            if (summonId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(summonId)));

            var domainSummon = _summonRepository.FindByGid(summonId);
            if (domainSummon == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(summonId)));

            var summon = new Domain.Service.Entities.Summon()
            {
                SummonId = domainSummon.Gid,
                SideId = _sideRepository.Find(domainSummon.SideId).Gid,
                SummonKind = domainSummon.SummonKind,
                SummonTypeCode = _summonTypeRepository.GetNom(domainSummon.SummonTypeId).Code,
                Number = domainSummon.Number,
                DateCreated = domainSummon.DateCreated,
                DateServed = domainSummon.DateServed,
                Addressee = domainSummon.Addressee,
                Address = domainSummon.Address,
                Subject = domainSummon.Subject
            };

            if (domainSummon.CaseId.HasValue)
                summon.ParentId = _caseRepository.Find(domainSummon.CaseId.Value).Gid;
            else if (domainSummon.ActId.HasValue)
                summon.ParentId = _actRepository.Find(domainSummon.ActId.Value).Gid;
            else if (domainSummon.HearingId.HasValue)
                summon.ParentId = _hearingRepository.Find(domainSummon.HearingId.Value).Gid;
            else if (domainSummon.AppealId.HasValue)
                summon.ParentId = _appealRepository.Find(domainSummon.AppealId.Value).Gid;

            return summon;
        }

        public List<Guid> GetSummonIdentifiersByParentId(Guid parentId, string summonTypeCode)
        {
            var summonIds = new List<Guid>();

            switch (summonTypeCode)
            {
                case "1":
                    var domainAct = _actRepository.SetWithoutIncludes()
                        .Include(i => i.Summons)
                        .FirstOrDefault(i => i.Gid == parentId);
                    if (domainAct == null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(parentId)));
                    else
                        summonIds = domainAct.Summons.Select(i => i.Gid).ToList();
                    break;
                case "2":
                    var domainAppeal = _appealRepository.SetWithoutIncludes()
                        .Include(i => i.Summons)
                        .FirstOrDefault(i => i.Gid == parentId);
                    if (domainAppeal == null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(parentId)));
                    else
                        summonIds = domainAppeal.Summons.Select(i => i.Gid).ToList();
                    break;
                case "3":
                    var domainCase = _caseRepository.SetWithoutIncludes()
                        .Include(i => i.Summons)
                        .FirstOrDefault(i => i.Gid == parentId);
                    if (domainCase == null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(parentId)));
                    else
                        summonIds = domainCase.Summons.Select(i => i.Gid).ToList();
                    break;
                case "4":
                    var domainHearing = _hearingRepository.SetWithoutIncludes()
                        .Include(i => i.Summons)
                        .FirstOrDefault(i => i.Gid == parentId);
                    if (domainHearing == null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(parentId)));
                    else
                        summonIds = domainHearing.Summons.Select(i => i.Gid).ToList();
                    break;
                default:
                    break;
            }

            return summonIds;
        }

        private void ValidateSummon(Domain.Service.Entities.Summon summon, bool isUpdate)
        {
            this.ValidateId(summon.SummonId, _summonRepository, isUpdate);

            if (summon.ParentId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Summon.ParentId"));

            switch (summon.SummonTypeCode)
            {
                case "1":
                    if (_actRepository.FindByGid(summon.ParentId) == null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Summon.ParentId(ActId)"));
                    break;
                case "2":
                    if (_appealRepository.FindByGid(summon.ParentId) == null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Summon.ParentId(AppealId)"));
                    break;
                case "3":
                    if (_caseRepository.FindByGid(summon.ParentId) == null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Summon.ParentId(CaseId)"));
                    break;
                case "4":
                    if (_hearingRepository.FindByGid(summon.ParentId) == null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Summon.ParentId(HearingId)"));
                    break;
                default:
                    break;
            }

            if (summon.SideId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Summon.SideId"));
            if (_sideRepository.FindByGid(summon.SideId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Summon.Side"));
            if (string.IsNullOrWhiteSpace(summon.SummonKind))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Summon.SummonKind"));
            if (!_summonTypeRepository.HasCode(summon.SummonTypeCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Summon.SummonTypeCode"));
            if (summon.DateCreated == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Summon.DateCreated"));
            if (string.IsNullOrWhiteSpace(summon.Addressee))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Summon.Addressee"));
            if (string.IsNullOrWhiteSpace(summon.Subject))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Summon.Subject"));


        }

        private void ValidateReadSummon(Domain.Entities.Summon summon)
        {
            if (!summon.ReadTime.HasValue)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Summon.ReadTime"));
            if (string.IsNullOrWhiteSpace(summon.SummonKind))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Summon.SummonKind"));
            if (summon.DateCreated == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "Summon.DateCreated"));
            if (string.IsNullOrWhiteSpace(summon.Addressee))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "Summon.Addressee"));
            if ((!summon.ReportBlobKey.HasValue) || (summon.ReportBlobKey.Value == default(Guid)))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "Summon.Report"));
        }

        #endregion

        #region SummonFiles

        public Guid? InsertSummonFile(SummonFile summonFile)
        {
            ValidateSummonFile(summonFile, false);

            try
            {
                var domainSummon = _summonRepository.FindByGid(summonFile.SummonId);
                return this.UploadFile(summonFile.SummonFileId, summonFile.Content, summonFile.MimeType, FileType.Summon, domainSummon.DateCreated, domainSummon, value => domainSummon.SummonBlobKey = value);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateSummonFile(SummonFile summonFile)
        {
            ValidateSummonFile(summonFile, true);

            try
            {
                var domainSummon = _summonRepository.FindByGid(summonFile.SummonId);
                return this.UploadFile(summonFile.SummonFileId, summonFile.Content, summonFile.MimeType, FileType.Summon, domainSummon.DateCreated, domainSummon, value => domainSummon.SummonBlobKey = value, true);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteSummonFile(Guid summonId)
        {
            var domainSummon = _summonRepository.FindByGid(summonId);
            if (domainSummon == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(summonId)));

            return this.DeleteFile(domainSummon, domainSummon.SummonBlobKey, value => domainSummon.SummonBlobKey = value);
        }

        public SummonFile GetSummonFileById(Guid summonFileId)
        {
            if (summonFileId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(summonFileId)));

            var domainSummon = _summonRepository.SetWithoutIncludes()
                .Include(a => a.SummonBlob)
                .FirstOrDefault(a => a.SummonBlobKey == summonFileId);
            if (domainSummon == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(summonFileId)));

            var summonFile = new SummonFile()
            {
                SummonFileId = domainSummon.SummonBlobKey,
                SummonId = domainSummon.Gid,
                MimeType = _blobStorageRepository.GetMimeType(domainSummon.SummonBlob.FileName),
                Content = _blobStorageRepository.GetFileContent(domainSummon.SummonBlobKey.Value),
            };

            return summonFile;
        }

        public Guid? GetSummonFileIdentifierBySummonId(Guid summonId)
        {
            var domainSummon = _summonRepository.FindByGid(summonId);
            if (domainSummon == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(summonId)));

            return domainSummon.SummonBlobKey;
        }

        private void ValidateSummonFile(SummonFile summonFile, bool isUpdate)
        {
            var domainSummon = _summonRepository.FindByGid(summonFile.SummonId);

            this.ValidateFile(summonFile.SummonFileId, summonFile.SummonId, summonFile.Content, summonFile.MimeType, domainSummon?.SummonBlobKey, domainSummon, isUpdate);
        }

        #endregion

        #region PersonAssignment

        public Guid? InsertPersonAssignment(Domain.Service.Entities.PersonAssignment personAssignment)
        {
            if (upgradedEpep)
            {
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.Unknown, upgradedEpepObsoleteMethod));
            }

            ValidatePersonAssignment(personAssignment, false);

            try
            {
                var domainPersonAssignment = new Domain.Entities.PersonAssignment()
                {
                    Gid = personAssignment.PersonAssignmentId.HasValue ? personAssignment.PersonAssignmentId.Value : Guid.NewGuid(),
                    Date = personAssignment.Date,
                    SideId = personAssignment.SideId.HasValue ? _sideRepository.FindByGid(personAssignment.SideId.Value).SideId : (long?)null,
                    PersonRegistrationId = _personRegistrationRepository.FindByGid(personAssignment.PersonRegistrationId).PersonRegistrationId,
                    IsActive = personAssignment.IsActive
                };

                var personRegistration = _personRegistrationRepository.Find(domainPersonAssignment.PersonRegistrationId);

                if (personRegistration != null)
                {
                    var user = _userRepository.Find(personRegistration.PersonRegistrationId);

                    if (user != null)
                    {
                        var userEmail = user.Username;

                        if (personAssignment.SideId.HasValue)
                        {
                            var domainSide = _sideRepository.FindByGid(personAssignment.SideId.Value);
                            var domainCase = _caseRepository.Find(domainSide.CaseId);

                            if (domainCase != null && domainCase.Court != null)
                            {
                                if (SendCaseAccessMail)
                                {
                                    domainPersonAssignment.GetCaseAccess(userEmail, domainCase.Abbreviation, domainCase.Court.Name);
                                }
                            }
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _personAssignmentRepository.Add(domainPersonAssignment);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainPersonAssignment.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdatePersonAssignment(Domain.Service.Entities.PersonAssignment personAssignment)
        {
            ValidatePersonAssignment(personAssignment, true);

            try
            {
                var domainPersonAssignment = _personAssignmentRepository.FindByGid(personAssignment.PersonAssignmentId.Value);

                if (personAssignment.SideId == null)
                {
                    var personRegistration = _personRegistrationRepository.Find(domainPersonAssignment.PersonRegistrationId);

                    if (personRegistration != null)
                    {
                        var user = _userRepository.Find(personRegistration.PersonRegistrationId);

                        if (user != null)
                        {
                            var userEmail = user.Username;

                            if (personAssignment.SideId.HasValue)
                            {
                                var domainSide = _sideRepository.FindByGid(personAssignment.SideId.Value);
                                var domainCase = _caseRepository.Find(domainSide.CaseId);

                                if (domainCase != null && domainCase.Court != null)
                                {
                                    domainPersonAssignment.DenyCaseAccess(userEmail, domainCase.Abbreviation, domainCase.Court.Name);
                                }
                            }
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    domainPersonAssignment.Date = personAssignment.Date;
                    domainPersonAssignment.SideId = personAssignment.SideId.HasValue ? _sideRepository.FindByGid(personAssignment.SideId.Value).SideId : (long?)null;
                    domainPersonAssignment.PersonRegistrationId = _personRegistrationRepository.FindByGid(personAssignment.PersonRegistrationId).PersonRegistrationId;
                    domainPersonAssignment.IsActive = personAssignment.IsActive;

                    domainPersonAssignment.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainPersonAssignment.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeletePersonAssignment(Guid personAssignmentId)
        {
            var domainPersonAssignment = _personAssignmentRepository.FindByGid(personAssignmentId);
            if (domainPersonAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(personAssignmentId)));

            try
            {
                var personRegistration = _personRegistrationRepository.Find(domainPersonAssignment.PersonRegistrationId);

                if (personRegistration != null)
                {
                    var user = _userRepository.Find(personRegistration.PersonRegistrationId);

                    if (user != null)
                    {
                        var userEmail = user.Username;

                        if (domainPersonAssignment.SideId.HasValue)
                        {
                            var domainSide = _sideRepository.Find(domainPersonAssignment.SideId.Value);

                            if (domainSide != null)
                            {
                                var domainCase = _caseRepository.Find(domainSide.CaseId);

                                if (domainCase != null && domainCase.Court != null)
                                {
                                    domainPersonAssignment.DenyCaseAccess(userEmail, domainCase.Abbreviation, domainCase.Court.Name);
                                }
                            }
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _personAssignmentRepository.Remove(domainPersonAssignment);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.PersonAssignment GetPersonAssignmentById(Guid personAssignmentId)
        {
            if (personAssignmentId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(personAssignmentId)));

            var domainPersonAssignment = _personAssignmentRepository.FindByGid(personAssignmentId);
            if (domainPersonAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(personAssignmentId)));

            var personAssignment = new Domain.Service.Entities.PersonAssignment()
            {
                PersonAssignmentId = domainPersonAssignment.Gid,
                Date = domainPersonAssignment.Date,
                SideId = domainPersonAssignment.SideId.HasValue ? _sideRepository.Find(domainPersonAssignment.SideId.Value).Gid : (Guid?)null,
                PersonRegistrationId = _personRegistrationRepository.Find(domainPersonAssignment.PersonRegistrationId).Gid,
                IsActive = domainPersonAssignment.IsActive.HasValue ? domainPersonAssignment.IsActive.Value : false
            };

            return personAssignment;
        }

        public List<Guid> GetPersonAssignmentIdentifiersBySideId(Guid sideId)
        {
            var domainSide = _sideRepository.SetWithoutIncludes()
                .Include(i => i.PersonAssignments)
                .FirstOrDefault(i => i.Gid == sideId);

            if (domainSide == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(sideId)));

            return domainSide.PersonAssignments.Select(i => i.Gid).ToList();
        }

        private void ValidatePersonAssignment(Domain.Service.Entities.PersonAssignment assignment, bool isUpdate)
        {
            this.ValidateId(assignment.PersonAssignmentId, _personAssignmentRepository, isUpdate);

            if (assignment.Date == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "PersonAssignment.Date"));
            if (assignment.SideId.HasValue)
            {
                if (assignment.SideId == default(Guid))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "PersonAssignment.SideId"));
                if (_sideRepository.FindByGid(assignment.SideId.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "PersonAssignment.SideId"));
            }

            if (assignment.PersonRegistrationId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "PersonAssignment.RegisteredPersonId"));
            if (_personRegistrationRepository.FindByGid(assignment.PersonRegistrationId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "PersonAssignment.RegisteredPersonId"));
        }

        #endregion

        #region ScannedDocument

        public Guid? InsertScannedDocument(ScannedDocument scannedDocument)
        {
            ValidateScannedDocument(scannedDocument, false);

            try
            {
                Guid blobKey = _blobStorageRepository.UploadFileToBlobStorage(Guid.NewGuid(), scannedDocument.ScannedDocumentContent, scannedDocument.ScannedDocumentMimeType, FileType.ScannedDocument, DateTime.Now);

                var domainScannedFile = new ScannedFile()
                {
                    Gid = scannedDocument.ScannedDocumentId.HasValue ? scannedDocument.ScannedDocumentId.Value : Guid.NewGuid(),
                    CaseId = _caseRepository.FindByGid(scannedDocument.CaseId).CaseId,
                    Description = scannedDocument.Description,
                    MimeType = scannedDocument.ScannedDocumentMimeType,
                    BlobKey = blobKey
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _scannedFileRepository.Add(domainScannedFile);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainScannedFile.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateScannedDocument(ScannedDocument scannedDocument)
        {
            ValidateScannedDocument(scannedDocument, true);

            try
            {
                var domainScannedFile = _scannedFileRepository.FindByGid(scannedDocument.ScannedDocumentId.Value);

                Guid blobKey = _blobStorageRepository.UploadFileToBlobStorage(Guid.NewGuid(), scannedDocument.ScannedDocumentContent, scannedDocument.ScannedDocumentMimeType, FileType.ScannedDocument, domainScannedFile.CreateDate);

                domainScannedFile.CaseId = _caseRepository.FindByGid(scannedDocument.CaseId).CaseId;
                domainScannedFile.Description = scannedDocument.Description;
                domainScannedFile.MimeType = scannedDocument.ScannedDocumentMimeType;
                domainScannedFile.BlobKey = blobKey;

                domainScannedFile.ModifyDate = DateTime.Now;

                return domainScannedFile.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteScannedDocument(Guid scannedDocumentId)
        {
            var domainScannedDocument = _scannedFileRepository.FindByGid(scannedDocumentId);
            if (domainScannedDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(scannedDocumentId)));

            try
            {
                MarkFileToBeDeleted(domainScannedDocument.BlobKey);

                _scannedFileRepository.Remove(domainScannedDocument);

                _unitOfWorks[DbKey.Main].Save();

                return true;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public ScannedDocument GetScannedDocumentById(Guid scannedDocumentId)
        {
            if (scannedDocumentId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(scannedDocumentId)));

            var domainScannedDocument = _scannedFileRepository.FindByGid(scannedDocumentId);
            if (domainScannedDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(scannedDocumentId)));

            var scannedDocument = new ScannedDocument()
            {
                ScannedDocumentId = domainScannedDocument.Gid,
                CaseId = _caseRepository.Find(domainScannedDocument.CaseId).Gid,
                ScannedDocumentMimeType = domainScannedDocument.MimeType,
                ScannedDocumentContent = _blobStorageRepository.GetFileContent(domainScannedDocument.BlobKey),
                Description = domainScannedDocument.Description
            };

            return scannedDocument;
        }

        public List<Guid> GetScannedDocumentIdentifiersByCaseId(Guid caseId)
        {
            var domainCase = _caseRepository.SetWithoutIncludes()
                .Include(i => i.ScannedFiles)
                .FirstOrDefault(i => i.Gid == caseId);

            if (domainCase == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(caseId)));

            return domainCase.ScannedFiles.Select(i => i.Gid).ToList();
        }

        private void ValidateScannedDocument(ScannedDocument scannedDocument, bool isUpdate)
        {
            this.ValidateId(scannedDocument.ScannedDocumentId, _scannedFileRepository, isUpdate);

            if (scannedDocument.CaseId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "ScannedDocument.CaseId"));
            if (_caseRepository.FindByGid(scannedDocument.CaseId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "ScannedDocument.CaseId"));
            if (string.IsNullOrWhiteSpace(scannedDocument.ScannedDocumentMimeType))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "ScannedDocument.ScannedDocumentMimeType"));
            if (scannedDocument.ScannedDocumentContent == null && scannedDocument.ScannedDocumentContent.Length <= 0)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "ScannedDocument.ScannedDocumentContent"));
        }

        #endregion

        #region Lawyer

        public List<Domain.Service.Entities.Lawyer> GetAllLawyers()
        {
            try
            {
                var lawyers = new List<Domain.Service.Entities.Lawyer>();

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domanLawyers = _lawyerRepository.GetAllLawyers();

                    foreach (var domainLawyer in domanLawyers)
                    {
                        var lawyer = new Domain.Service.Entities.Lawyer()
                        {
                            LawyerId = domainLawyer.Gid,
                            Number = domainLawyer.Number,
                            Name = domainLawyer.Name,
                            College = domainLawyer.College
                        };

                        lawyers.Add(lawyer);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return lawyers;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public List<Domain.Service.Entities.Lawyer> GetAllNewLawyers(DateTime from)
        {
            try
            {
                var lawyers = new List<Domain.Service.Entities.Lawyer>();

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domanLawyers = _lawyerRepository.GetAllLawyersFromCertainDate(from);

                    foreach (var domainLawyer in domanLawyers)
                    {
                        var lawyer = new Domain.Service.Entities.Lawyer()
                        {
                            LawyerId = domainLawyer.Gid,
                            Number = domainLawyer.Number,
                            Name = domainLawyer.Name,
                            College = domainLawyer.College
                        };

                        lawyers.Add(lawyer);
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return lawyers;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.Lawyer GetLawyerByNumber(string number)
        {
            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainLawyer = _lawyerRepository.GetLawyerByNumber(number);

                    var lawyer = new Domain.Service.Entities.Lawyer()
                    {
                        LawyerId = domainLawyer.Gid,
                        Number = domainLawyer.Number,
                        Name = domainLawyer.Name,
                        College = domainLawyer.College
                    };

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return lawyer;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        #endregion

        #region LawyerRegistration

        public Guid? InsertLawyerRegistration(Domain.Service.Entities.LawyerRegistration lr)
        {
            if (upgradedEpep)
            {
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.Unknown, upgradedEpepObsoleteMethod));
            }

            ValidateLawyerRegistration(lr, false);

            try
            {
                var user = _userRepository.Find(lr.Email);

                if (user != null)
                {
                    return user.Gid;
                }

                var domainLawyerRegistration = new Domain.Entities.LawyerRegistration()
                {
                    Gid = lr.LawyerRegistrationId.HasValue ? lr.LawyerRegistrationId.Value : Guid.NewGuid(),
                    LawyerId = _lawyerRepository.FindByGid(lr.LawyerId).LawyerId,
                    BirthDate = lr.BirthDate,
                    Description = lr.Description,
                };

                var domainUser = new User
                    (
                        domainLawyerRegistration.Gid,
                        UserGroup.Lawyer,
                        _lawyerRepository.FindByGid(lr.LawyerId).Name,
                        lr.Email
                    );

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _lawyerRegistrationRepository.Add(domainLawyerRegistration);
                    _userRepository.Add(domainUser);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainLawyerRegistration.Gid;
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateLawyerRegistration(Domain.Service.Entities.LawyerRegistration lawyerRegistration)
        {
            ValidateLawyerRegistration(lawyerRegistration, true);

            try
            {
                var existingusername = _userRepository.Find(lawyerRegistration.Email);

                var domainLawyerRegistration = _lawyerRegistrationRepository.FindByGid(lawyerRegistration.LawyerRegistrationId.Value);

                if (domainLawyerRegistration == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerRegistration)));

                var domainUser = _userRepository.Find(domainLawyerRegistration.LawyerRegistrationId);
                if (existingusername != null && existingusername.Username != domainUser.Username)
                {
                    throw new FaultException(new FaultReason("This Email is in use."));
                }

                if (domainUser == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(domainUser)));

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    domainLawyerRegistration.User.Username = lawyerRegistration.Email.Trim();
                    domainLawyerRegistration.BirthDate = lawyerRegistration.BirthDate;
                    domainLawyerRegistration.Description = lawyerRegistration.Description;
                    domainLawyerRegistration.ModifyDate = DateTime.Now;

                    if (domainLawyerRegistration.User.Username.Trim() != lawyerRegistration.Email.Trim())
                    {
                        domainUser.UpdateUsername(domainLawyerRegistration.User.Username, lawyerRegistration.Email.Trim(), null);
                        domainUser.ModifyDate = DateTime.Now;
                    }
                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainLawyerRegistration.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }


        public Guid? ActivateLawyerRegistration(Guid lawyerRegistrationId)
        {
            ValidateLawyerRegistrationId(lawyerRegistrationId);

            try
            {
                var user = _userRepository.FindByGid(lawyerRegistrationId);

                if (user.IsActive)
                {
                    return user.Gid;
                }

                user.IsActive = true;

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return user.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? DeactivateLawyerRegistration(Guid lawyerRegistrationId)
        {
            ValidateLawyerRegistrationId(lawyerRegistrationId);

            try
            {
                var user = _userRepository.FindByGid(lawyerRegistrationId);

                if (!user.IsActive)
                {
                    return user.Gid;
                }

                user.IsActive = false;

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return user.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.LawyerRegistration GetLawyerRegistrationById(Guid lawyerRegistrationId)
        {
            if (lawyerRegistrationId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerRegistrationId)));

            var domainLawyerRegistration = _lawyerRegistrationRepository.FindByGid(lawyerRegistrationId);
            if (domainLawyerRegistration == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerRegistrationId)));

            var lawyerRegistration = new Domain.Service.Entities.LawyerRegistration()
            {
                LawyerRegistrationId = domainLawyerRegistration.Gid,
                LawyerId = _lawyerRepository.Find(domainLawyerRegistration.LawyerId).Gid,
                BirthDate = domainLawyerRegistration.BirthDate,
                Email = _userRepository.Find(domainLawyerRegistration.LawyerRegistrationId).Username,
                Description = domainLawyerRegistration.Description
            };

            return lawyerRegistration;
        }


        public List<Guid> GetLawyerRegistrationIdentifiersByLawyerId(Guid lawyerId)
        {
            var domainLawyer = _lawyerRepository.SetWithoutIncludes()
                .Include(i => i.LawyerRegistrations)
                .FirstOrDefault(i => i.Gid == lawyerId);

            if (domainLawyer == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerId)));

            return domainLawyer.LawyerRegistrations.Select(i => i.Gid).ToList();
        }

        private void ValidateLawyerRegistrationId(Guid lawyerRegistrationId)
        {
            if (_lawyerRegistrationRepository.FindByGid(lawyerRegistrationId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "LawyerRegistrationId"));
            if (_userRepository.FindByGid(lawyerRegistrationId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "LawyerRegistrationId"));
        }

        private void ValidateLawyerRegistration(Domain.Service.Entities.LawyerRegistration lr, bool isUpdate)
        {
            this.ValidateId(lr.LawyerRegistrationId, _lawyerRegistrationRepository, isUpdate);

            if (isUpdate)
            {
                if (string.IsNullOrWhiteSpace(lr.Email) && _userRepository.Find(lr.Email) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "LawyerRegistration.Email"));
            }
            if (!isUpdate)
            {
                if (lr.LawyerId == default(Guid))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "LawyerRegistration.LawyerId"));
                if (_lawyerRepository.FindByGid(lr.LawyerId) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "LawyerRegistration.LawyerId"));

                if (string.IsNullOrWhiteSpace(lr.Email))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "LawyerRegistration.Email"));
                if (!new Regex(EMAIL_PATTERN).IsMatch(lr.Email))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "LawyerRegistration.Email"));
                if (lr.BirthDate == DateTime.MinValue)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "LawyerRegistration.BirthDate"));
            }
        }

        public Domain.Service.Entities.LawyerRegistration SelectLawyerRegistration(String lawyerNumber)
        {
            if (lawyerNumber == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerNumber)));


            var domainLawyerRegistration = _lawyerRegistrationRepository.SetWithoutIncludes()
                     .Include(i => i.Lawyer)
                     .FirstOrDefault(i => i.Lawyer.Number == lawyerNumber);

            if (domainLawyerRegistration == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(lawyerNumber)));


            var lawyerRegistration = new Domain.Service.Entities.LawyerRegistration()
            {
                LawyerRegistrationId = domainLawyerRegistration.Gid,
                LawyerId = _lawyerRepository.Find(domainLawyerRegistration.LawyerId).Gid,
                BirthDate = domainLawyerRegistration.BirthDate,
                Email = _userRepository.Find(domainLawyerRegistration.LawyerRegistrationId).Username,
                Description = domainLawyerRegistration.Description
            };

            return lawyerRegistration;


        }

        #endregion

        #region PersonRegistration

        public Guid? InsertPersonRegistration(Domain.Service.Entities.PersonRegistration pr)
        {
            if (upgradedEpep)
            {
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.Unknown, upgradedEpepObsoleteMethod));
            }

            ValidatePersonRegistration(pr, false);

            try
            {
                var user = _userRepository.Find(pr.Email);

                if (user != null && user.IsActive)
                {
                    return user.Gid;
                }

                var domainPersonRegistratoin = new Domain.Entities.PersonRegistration()
                {
                    Gid = pr.PersonRegistrationId.HasValue ? pr.PersonRegistrationId.Value : Guid.NewGuid(),
                    Name = pr.Name,
                    EGN = pr.EGN,
                    BirthDate = pr.BirthDate,
                    Email = pr.Email,
                    Address = pr.Address,
                    Description = pr.Description
                };

                var domainUser = new User
                    (
                        domainPersonRegistratoin.Gid,
                        UserGroup.Person,
                        pr.Name,
                        pr.Email
                    );

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _personRegistrationRepository.Add(domainPersonRegistratoin);
                    _userRepository.Add(domainUser);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainPersonRegistratoin.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdatePersonRegistration(Domain.Service.Entities.PersonRegistration pr)
        {
            ValidatePersonRegistration(pr, true);

            try
            {
                var existingusername = _userRepository.Find(pr.Email);

                var domainPersonRegistration = _personRegistrationRepository.FindByGid(pr.PersonRegistrationId.Value);
                //var domainUser = domainPersonRegistration.User;

                var oldUsername = domainPersonRegistration.Email.Trim();
                if (domainPersonRegistration == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(pr)));

                var domainUser = _userRepository.Find(domainPersonRegistration.PersonRegistrationId);

                if (domainUser == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(domainUser)));

                if (existingusername != null && existingusername.Username != domainUser.Username)
                {
                    throw new FaultException(new FaultReason("This Email is in use."));
                }
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    domainPersonRegistration.User.Username = pr.Email;
                    domainPersonRegistration.BirthDate = pr.BirthDate;
                    domainPersonRegistration.Name = pr.Name;
                    domainPersonRegistration.EGN = pr.EGN;
                    domainPersonRegistration.Email = pr.Email.Trim();
                    domainPersonRegistration.Address = pr.Address;
                    domainPersonRegistration.Description = pr.Description;
                    domainPersonRegistration.ModifyDate = DateTime.Now;

                    if (domainPersonRegistration.User.Username != pr.Email)
                    {
                        domainUser.UpdateUsername(domainPersonRegistration.User.Username, pr.Email, null);
                        domainUser.ModifyDate = DateTime.Now;
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                    return domainPersonRegistration.Gid;
                }
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? ActivatePersonRegistration(Guid personRegistrationId)
        {
            ValidatePersonRegistrationId(personRegistrationId);

            try
            {
                var user = _userRepository.FindByGid(personRegistrationId);

                if (user.IsActive)
                {
                    return user.Gid;
                }

                user.IsActive = true;

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return user.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? DeactivatePersonRegistration(Guid personRegistrationId)
        {
            ValidatePersonRegistrationId(personRegistrationId);

            try
            {
                var user = _userRepository.FindByGid(personRegistrationId);

                if (!user.IsActive)
                {
                    return user.Gid;
                }

                user.IsActive = false;

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return user.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.PersonRegistration GetPersonRegistrationById(Guid personRegistrationId)
        {
            if (personRegistrationId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(personRegistrationId)));

            var domainPersonRegistration = _personRegistrationRepository.FindByGid(personRegistrationId);
            if (domainPersonRegistration == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(personRegistrationId)));

            var personRegistration = new Domain.Service.Entities.PersonRegistration()
            {
                PersonRegistrationId = domainPersonRegistration.Gid,
                Email = _userRepository.Find(domainPersonRegistration.PersonRegistrationId).Username,
                Name = domainPersonRegistration.Name,
                EGN = domainPersonRegistration.EGN,
                BirthDate = domainPersonRegistration.BirthDate,
                Address = domainPersonRegistration.Address,
                Description = domainPersonRegistration.Description
            };

            return personRegistration;
        }

        public Guid? GetPersonRegistrationIdentifierByPersonAssignmentId(Guid personassignmentId)
        {
            var domainPersonAssignment = _personAssignmentRepository.SetWithoutIncludes()
                .Include(i => i.PersonRegistration)
                .FirstOrDefault(i => i.Gid == personassignmentId);

            if (domainPersonAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(personassignmentId)));

            return domainPersonAssignment.PersonRegistration.Gid;
        }

        private void ValidatePersonRegistrationId(Guid personRegistrationId)
        {
            if (_personRegistrationRepository.FindByGid(personRegistrationId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "PersonRegistrationId"));
            if (_userRepository.FindByGid(personRegistrationId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "PersonRegistrationId"));
        }

        private void ValidatePersonRegistration(Domain.Service.Entities.PersonRegistration pr, bool isUpdate)
        {
            this.ValidateId(pr.PersonRegistrationId, _personRegistrationRepository, isUpdate);

            if (isUpdate)
            {
                if (string.IsNullOrWhiteSpace(pr.Email) && _userRepository.Find(pr.Email) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "PersonRegistration.Email"));
            }
            if (!isUpdate)
            {
                if (string.IsNullOrWhiteSpace(pr.Email))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "PersonRegistration.Email"));
                if (!new Regex(EMAIL_PATTERN).IsMatch(pr.Email))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "PersonRegistration.Email"));
                if (pr.BirthDate == DateTime.MinValue)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "PersonRegistration.BirthDate"));
                if (string.IsNullOrWhiteSpace(pr.EGN))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "PersonRegistration.EGN"));
                if (string.IsNullOrWhiteSpace(pr.Name))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "PersonRegistration.Name"));
            }
        }

        public Domain.Service.Entities.PersonRegistration SelectPersonRegistration(String EGN)
        {
            if (EGN == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(EGN)));


            var domainPersonRegistration = _personRegistrationRepository.SetWithoutIncludes()
                     .FirstOrDefault(i => i.EGN == EGN);

            if (domainPersonRegistration == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(EGN)));

            var personRegistration = new Domain.Service.Entities.PersonRegistration()
            {
                PersonRegistrationId = domainPersonRegistration.Gid,
                Email = _userRepository.Find(domainPersonRegistration.PersonRegistrationId).Username,
                Name = domainPersonRegistration.Name,
                EGN = domainPersonRegistration.EGN,
                BirthDate = domainPersonRegistration.BirthDate,
                Address = domainPersonRegistration.Address,
                Description = domainPersonRegistration.Description
            };

            return personRegistration;


        }
        #endregion

        #region Users
        public Domain.Service.Entities.UserRegistrationInfo GetUserRegistrationInfoByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "username"));

            try
            {
                Domain.Service.Entities.UserRegistrationInfo userRegistrationInfo = new Domain.Service.Entities.UserRegistrationInfo();
                var user = _userRepository.Find(username);
                var gid = user?.Gid;
                userRegistrationInfo.IsRegistered = gid == Guid.Empty || gid == null ? false : true;

                if (userRegistrationInfo.IsRegistered)
                {
                    var personRegistration = _personRegistrationRepository.FindByGid((Guid)gid);
                    userRegistrationInfo.PersonRegistrationId = personRegistration?.Gid;

                    var lawyerRegistration = _lawyerRegistrationRepository.FindByGid((Guid)gid);
                    userRegistrationInfo.LawyerRegistrationId = lawyerRegistration?.Gid;
                }

                return userRegistrationInfo;
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }

        }

        public Guid? UpdateUsername(string oldUsername, string newUsername, string courtCode)
        {
            ValidateUpdateUsernameParameters(oldUsername, newUsername, courtCode);

            string exceptionInfo = string.Empty;
            try
            {
                var currentTime = DateTime.Now;

                var domainUser = _userRepository.Find(oldUsername);

                var courtId = _courtRepository.GetNomIdByCode(courtCode);
                var courtNom = _courtRepository.GetNom(courtId);

                var existingusername = _userRepository.Find(newUsername);

                if (existingusername != null)
                {
                    throw new FaultException(new FaultReason("This Email is in use."));
                }

                domainUser.UpdateUsername(oldUsername, newUsername, courtNom.Name);
                domainUser.ModifyDate = currentTime;

                switch (domainUser.UserGroupId)
                {
                    case UserGroup.Lawyer:
                        break;
                    case UserGroup.Person:
                        var person = _personRegistrationRepository.Find(domainUser.UserId);
                        person.Email = newUsername;
                        break;
                    case UserGroup.CourtAdmin:
                        break;
                    default:
                        throw new NotSupportedException();
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainUser.Gid;
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        private void ValidateUpdateUsernameParameters(string oldUsername, string newUsername, string courtCode)
        {
            if (string.IsNullOrWhiteSpace(oldUsername))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "oldUsername"));

            if (string.IsNullOrWhiteSpace(newUsername))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, "newUsername"));

            var currentUser = _userRepository.Find(oldUsername);

            if (!string.IsNullOrWhiteSpace(oldUsername) && currentUser == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "oldUsername"));

            if (currentUser.UserGroupId == UserGroup.SystemAdmin || currentUser.UserGroupId == UserGroup.SuperAdmin)
                if (currentUser.UserGroupId == UserGroup.SuperAdmin)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "oldUsername"));

            if (!string.IsNullOrWhiteSpace(newUsername) && _userRepository.Find(newUsername) != null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "newUsername"));

            if (!_courtRepository.HasCode(courtCode))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "courtCode"));


        }

        #endregion

        #region CommonLogic

        private void MarkFileToBeDeleted(Guid blobKey)
        {
            var mainContext = ((UnitOfWork)_unitOfWorks[DbKey.Main]).DbContext;
            var blobContext = ((UnitOfWork)_unitOfWorks[DbKey.BlobStorage]).DbContext;

            var blobLocation = (from b in mainContext.Set<Blob>()
                                join c in mainContext.Set<BlobContentLocation>() on b.BlobContentLocationId equals c.BlobContentLocationId
                                where b.Key == blobKey
                                select c).SingleOrDefault();

            if (blobLocation != null)
            {
                var blobContent = blobContext.Set<Domain.BlobStorage.BlobContent>().Find(blobLocation.BlobContentId);

                if (blobContent != null)
                {
                    blobContent.IsDeleted = true;
                    _unitOfWorks[DbKey.BlobStorage].Save();
                }
            }
        }

        private Guid UploadFile(Guid? fileId, byte[] content, string mimeType, FileType fileType, DateTime date, IAggregateRoot parent, Action<Guid?> parentBlobKey, bool isUpdate = false, string originalFileName = null)
        {
            Guid blobKey;

            if (isUpdate)
            {
                MarkFileToBeDeleted(fileId.Value);
                blobKey = fileId.Value;
            }
            else
                blobKey = fileId ?? Guid.NewGuid();

            blobKey = _blobStorageRepository.UploadFileToBlobStorage(blobKey, content, mimeType, fileType, date, isUpdate, originalFileName);

            if (parent != null)
            {
                parentBlobKey(blobKey);
                parent.ModifyDate = DateTime.Now;
            }

            using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
            {
                _unitOfWorks[DbKey.Main].Save();
                transaction.Commit();
            }

            return blobKey;
        }

        private bool DeleteFile(IAggregateRoot parent, Guid? key, Action<Guid?> blobKey)
        {
            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    if (key.HasValue)
                    {
                        MarkFileToBeDeleted(key.Value);

                        blobKey(null);
                        parent.ModifyDate = DateTime.Now;
                    }

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        private bool DeleteDomainEntity<TEntity>(Guid domainEntityId, IAggregateRepository<TEntity> repository) where TEntity : class, IAggregateRoot
        {
            var domainEntity = repository.FindByGid(domainEntityId);
            if (domainEntity == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(domainEntityId)));

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    repository.Remove(domainEntity);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        #endregion

        #region ExceptionLogic

        private void ValidateFile(Guid? fileId, Guid parentId, byte[] content, string mimeType, Guid? blobKey, IAggregateRoot parent, bool isUpdate)
        {
            if (parentId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(parentId)));
            if (parent == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(parentId)));

            if (isUpdate)
            {
                if (!fileId.HasValue)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, nameof(fileId)));
                else
                {
                    if (!blobKey.HasValue)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(blobKey)));
                    else
                    {
                        if (blobKey.Value != fileId.Value)
                            throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(fileId)));
                    }
                }
            }
            else
            {
                if (blobKey.HasValue)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(blobKey)));
                if (fileId.HasValue && _blobStorageRepository.HasKey(fileId.Value))
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(fileId)));
            }

            if (string.IsNullOrWhiteSpace(mimeType))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, nameof(mimeType)));
            if (content == null || content.Length == 0)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, nameof(content)));
        }

        private void ValidateId<TEntity>(Guid? id, IAggregateRepository<TEntity> repository, bool isUpdate) where TEntity : class, IAggregateRoot
        {
            if (isUpdate)
            {
                if (!id.HasValue)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, nameof(id)));
                if (id.HasValue && repository.FindByGid(id.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(id)));
            }
            else
            {
                if (id.HasValue)
                {
                    if (id.Value == default(Guid))
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(id)));
                    if (repository.FindByGid(id.Value) != null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(id)));
                }
            }
        }
        private void ValidateId<TEntity>(Guid? id, bool isUpdate) where TEntity : class, IGidRoot
        {
            if (isUpdate)
            {
                if (!id.HasValue)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.RequiredField, nameof(id)));
                if (id.HasValue && upgradeRepository.FindByGid<TEntity>(id.Value) == null)
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(id)));
            }
            else
            {
                if (id.HasValue)
                {
                    if (id.Value == default(Guid))
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(id)));
                    if (upgradeRepository.FindByGid<TEntity>(id.Value) != null)
                        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, nameof(id)));
                }
            }
        }

        private static bool ShowStackTrace
        {
            get
            {
                string showStackTrace = ConfigurationManager.AppSettings["eCase.Service:ShowStackTrace"];

                if (!string.IsNullOrWhiteSpace(showStackTrace) && showStackTrace.ToLower().Equals("true"))
                    return true;
                else
                    return false;
            }
        }

        private FaultException<InfocaseFault> HandleCommonException(Exception ex)
        {
            var exceptionInfo = Helper.GetDetailedExceptionInfo(ex);
            _logger.Error(exceptionInfo);

            if (ShowStackTrace)
                return new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.Unknown, exceptionInfo));
            else
                return new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.Unknown, "Unknown"));
        }

        #endregion

        #region MailLogic

        private static bool SendCaseAccessMail
        {
            get
            {
                string sendCaseAccessMail = ConfigurationManager.AppSettings["eCase.Service:SendCaseAccessMail"];

                if (!string.IsNullOrWhiteSpace(sendCaseAccessMail) && sendCaseAccessMail.ToLower().Equals("true"))
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region AttachedDocument

        public List<Guid> GetAttachedDocumentIdentifiers(int type, Guid parentId)
        {
            var domainParentId = _attachedDocumentRepository.GetParentIdByGid(type, parentId);
            if (domainParentId <= 0)
            {
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(parentId)));
            }

            return _attachedDocumentRepository.GetAttachedDocuments(type, domainParentId)
                        .Select(x => x.Gid)
                        .ToList();
        }

        public Domain.Service.Entities.AttachedDocument GetAttachedDocumentById(Guid attachedDocumentId)
        {
            return _attachedDocumentRepository.GetAttachedDocumentById(attachedDocumentId);
        }

        public Guid? InsertAttachedDocument(Domain.Service.Entities.AttachedDocument attachedDocument)
        {
            long domainParentId = 0;
            ValidateAttachedDocument(attachedDocument, out domainParentId);

            try
            {
                Guid blobKey = _blobStorageRepository.UploadFileToBlobStorage(Guid.NewGuid(), attachedDocument.FileContent, attachedDocument.MimeType, FileType.AttachedDocument, attachedDocument.FileDate, false, attachedDocument.FileName);

                var domainAttachedDocument = new Domain.Entities.AttachedDocument()
                {
                    Gid = attachedDocument.AttachedDocumentId.HasValue ? attachedDocument.AttachedDocumentId.Value : Guid.NewGuid(),
                    AttachmentType = attachedDocument.Type,
                    ParentId = domainParentId,
                    FileName = attachedDocument.FileName,
                    FileTitle = attachedDocument.FileTitle,
                    BlobKey = blobKey
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _attachedDocumentRepository.Add(domainAttachedDocument);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainAttachedDocument.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        //public Guid? UpdateAssignmentFile(AssignmentFile assignmentFile)
        //{
        //    ValidateAssignmentFile(assignmentFile, true);

        //    try
        //    {
        //        var domainAssignment = _assignmentRepository.FindByGid(assignmentFile.AssignmentId);
        //        return this.UploadFile(assignmentFile.AssignmentFileId, assignmentFile.ProtocolContent, assignmentFile.ProtocolMimeType, FileType.Assignment, domainAssignment.Date, domainAssignment, value => domainAssignment.BlobKey = value, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw HandleCommonException(ex);
        //    }
        //}

        public bool DeleteAttachedDocument(Guid attachedDocumentId)
        {
            var domainAttachedDocument = _attachedDocumentRepository.FindByGid(attachedDocumentId);
            if (domainAttachedDocument == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(attachedDocumentId)));

            try
            {
                MarkFileToBeDeleted(domainAttachedDocument.BlobKey);

                _attachedDocumentRepository.Remove(domainAttachedDocument);

                _unitOfWorks[DbKey.Main].Save();

                return true;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        //public Domain.Service.Entities.AttachedDocument GetAttachedDocumentById(Guid assignmentFileId)
        //{
        //    if (assignmentFileId == Guid.Empty)
        //        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentFileId)));

        //    var domainAttachedDocument = _assignmentRepository.SetWithoutIncludes()
        //        .Include(a => a.Blob)
        //        .FirstOrDefault(a => a.BlobKey == assignmentFileId);
        //    if (domainAttachedDocument == null)
        //        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentFileId)));

        //    var assignmentFile = new Domain.Service.Entities.AttachedDocument()
        //    {
        //        AttachedDocumentId = domainAttachedDocument.BlobKey,
        //        Type = domainAttachedDocument.Gid,
        //        ProtocolMimeType = _blobStorageRepository.GetMimeType(domainAttachedDocument.Blob.FileName),
        //        ProtocolContent = _blobStorageRepository.GetFileContent(domainAttachedDocument.BlobKey.Value),
        //    };

        //    return assignmentFile;
        //}

        //public Guid? GetAttachedDocumentIdentifiersByAssignmentId(Guid assignmentId)
        //{
        //    var domainAssignment = _assignmentRepository.FindByGid(assignmentId);
        //    if (domainAssignment == null)
        //        throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(assignmentId)));

        //    return domainAssignment.BlobKey;
        //}

        private void ValidateAttachedDocument(Domain.Service.Entities.AttachedDocument attachedDocument, out long parentId)
        {
            var domainParentId = _attachedDocumentRepository.GetParentIdByGid(attachedDocument.Type, attachedDocument.ParentId);
            if (domainParentId <= 0)
            {
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(attachedDocument.ParentId)));
            }
            parentId = domainParentId;
        }
        #endregion

        #region Hearing Document

        public Guid? InsertHearingDocument(Domain.Service.Entities.HearingDocument hearingDocument)
        {
            ValidateHearingDocument(hearingDocument, false);

            try
            {
                var domainHearingDocument = new Domain.Entities.HearingDocument()
                {
                    Gid = hearingDocument.HearingDocumentId.HasValue ? hearingDocument.HearingDocumentId.Value : Guid.NewGuid(),
                    HearingId = _hearingRepository.FindByGid(hearingDocument.HearingId).HearingId,
                    SideId = _sideRepository.FindByGid(hearingDocument.SideId).SideId,
                    HearingDocumentKind = hearingDocument.HearingDocumentKind
                };

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    _hearingDocumentRepository.Add(domainHearingDocument);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainHearingDocument.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Guid? UpdateHearingDocument(Domain.Service.Entities.HearingDocument hearingDocument)
        {
            ValidateHearingDocument(hearingDocument, true);

            try
            {
                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    var domainHearingDocument = _hearingDocumentRepository.FindByGid(hearingDocument.HearingDocumentId.Value);

                    domainHearingDocument.HearingId = _hearingRepository.FindByGid(hearingDocument.HearingId).HearingId;
                    domainHearingDocument.SideId = _sideRepository.FindByGid(hearingDocument.SideId).SideId;
                    domainHearingDocument.HearingDocumentKind = hearingDocument.HearingDocumentKind;

                    domainHearingDocument.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return domainHearingDocument.Gid;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public bool DeleteHearingDocument(Guid hearingDocumentId)
        {
            return this.DeleteDomainEntity(hearingDocumentId, _hearingDocumentRepository);
        }

        private void ValidateHearingDocument(Domain.Service.Entities.HearingDocument hearingDocument, bool isUpdate)
        {
            this.ValidateId(hearingDocument.HearingDocumentId, _hearingDocumentRepository, isUpdate);

            if (hearingDocument.HearingId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "HearingDocument.HearingId"));
            if (_hearingRepository.FindByGid(hearingDocument.HearingId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "HearingDocument.HearingId"));

            if (hearingDocument.SideId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "HearingDocument.SideId"));
            if (_sideRepository.FindByGid(hearingDocument.SideId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "HearingDocument.SideId"));
        }

        public List<Guid> GetHearingDocumentIdentifiersByHearingId(Guid hearingId)
        {
            return upgradeRepository.AllReadonly<Domain.Entities.HearingDocument>()
                                    .Where(x => x.Hearing.Gid == hearingId)
                                    .Select(x => x.Gid)
                                    .ToList();
        }

        public Domain.Service.Entities.HearingDocument GetHearingDocumentById(Guid hearingDocumentId)
        {
            return upgradeRepository.AllReadonly<Domain.Entities.HearingDocument>()
                                    .Where(x => x.Gid == hearingDocumentId)
                                    .Select(x => new Domain.Service.Entities.HearingDocument
                                    {
                                        HearingDocumentKind = x.HearingDocumentKind,
                                        HearingId = x.Hearing.Gid,
                                        SideId = x.Side.Gid
                                    })
                                    .FirstOrDefault();
        }
        #endregion

        #region Upgrade entities methods

        public List<UserRegistration> GetUserRegistrations(DateTime? modifyFromDate = null)
        {
            long?[] activeStates = { 1, 2 };
            var activeLawyers = upgradeRepository.AllReadonly<Domain.Entities.Lawyer>()
                                                    .Where(x => activeStates.Contains(x.LawyerStateId))
                                                    .Where(x => x.Uic != null && x.Uic != "")
                                                    .GroupBy(x => new { x.Uic, x.Number })
                                                    .Select(x => new
                                                    {
                                                        x.Key.Number,
                                                        x.Key.Uic
                                                    }).ToList();

            List<string> moodifiedLawyersEgn = new List<string>();
            if (modifyFromDate.HasValue)
            {
                moodifiedLawyersEgn = upgradeRepository.AllReadonly<Domain.Entities.Lawyer>()
                                                        .Where(x => activeStates.Contains(x.LawyerStateId))
                                                        .Where(x => x.Uic != null && x.Uic != "")
                                                        .Where(x => x.ModifyDate > (modifyFromDate ?? x.ModifyDate))
                                                        .Select(x => x.Uic)
                                                        .Take(upgradeUserFetch / 5)
                                                        .ToList();
            }

            var result = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.UserRegistration>()
                                    .Where(x => EpepConstants.UserTypes.eCaseUserTypes.Contains(x.UserTypeId))
                                    .Where(x => ((x.ComfirmedDate != null) || (x.UserTypeId == EpepConstants.UserTypes.Person)) && x.IsActive)
                                    .Where(x => (x.ModifyDate > (modifyFromDate ?? x.ModifyDate)) || moodifiedLawyersEgn.Contains(x.EGN))
                                    .OrderBy(x => x.ModifyDate)
                                    .Select(x => new UserRegistration
                                    {
                                        UserRegistrationId = x.Gid,
                                        Email = x.Email,
                                        UIC = x.EGN ?? x.UIC,
                                        Name = x.FullName,
                                        ModifyDate = x.ModifyDate,
                                        UserType = x.UserTypeId
                                    }).Take(upgradeUserFetch)
                                    .ToList();

            foreach (var item in result)
            {
                if (item.UserType != UpgradeEpepConstants.UserTypes.Person)
                {
                    continue;
                }

                var lawyerNumbers = activeLawyers.Where(x => x.Uic == item.UIC).Select(x => x.Number).ToArray();
                if (lawyerNumbers.Any())
                {
                    string value = string.Join(";", lawyerNumbers);
                    //item.UIC = null;
                    item.LawyerNumber = value;
                }
            }

            return result;
        }

        public ElectronicDocument GetElectronicDocument(Guid electronicDocumentId)
        {
            try
            {
                ElectronicDocument result = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.ElectronicDocument>()
                                        .Include(x => x.Court)
                                        .Include(x => x.ElectronicDocumentType)
                                        .Include(x => x.User)
                                        .Include(x => x.Currency)
                                        .Include(x => x.MoneyPricelist)
                                        .Include(x => x.Case)
                                        .Where(x => x.Gid == electronicDocumentId)
                                        .Select(
                                            x => new ElectronicDocument
                                            {
                                                ElectronicDocumentId = x.Gid,
                                                CaseId = (x.CaseId > 0) ? x.Case.Gid : (Guid?)null,
                                                SideId = (x.SideId > 0) ? x.Side.Gid : (Guid?)null,
                                                CurrencyCode = (x.MoneyCurrencyId > 0) ? x.Currency.Code : "",
                                                PricelistCode = (x.MoneyPricelistId > 0) ? x.MoneyPricelist.Code : "",
                                                BaseAmount = (int?)Math.Round((x.BaseAmount ?? 0M) * 100),
                                                TaxAmount = (int?)Math.Round((x.TaxAmount ?? 0M) * 100),
                                                CourtCode = x.Court.Code,
                                                DateApply = x.DateApply.Value,
                                                NumberApply = x.NumberApply,
                                                DatePaid = x.DatePaid.Value,
                                                Description = x.Description,
                                                DocumentType = x.ElectronicDocumentType.Code,
                                                DocumentKind = x.ElectronicDocumentType.DocumentKind.ToString(),
                                                UserRegistrationId = x.User.Gid
                                            }).FirstOrDefault();
                if (result == null)
                {
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "electronicDocumentId"));
                }

                result.Sides = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.ElectronicDocumentSide>()
                                                .Include(x => x.ElectronicDocument)
                                                .Include(x => x.Subject)
                                                .Include(x => x.Subject.Entity)
                                                .Include(x => x.Subject.Person)
                                                .Include(x => x.SideInvolvementKind)
                                                .Where(x => x.ElectronicDocument.Gid == electronicDocumentId)
                                                .Select(s => new ElectronicDocumentSide
                                                {
                                                    ElectronicDocumentSideId = s.Gid,
                                                    SideInvolvementKind = s.SideInvolvementKind.Code,
                                                    Person = (s.Subject.Person != null) ? new Domain.Service.Entities.Person
                                                    {
                                                        EGN = s.Subject.Person.EGN,
                                                        Firstname = s.Subject.Person.Firstname,
                                                        Secondname = s.Subject.Person.Secondname,
                                                        Lastname = s.Subject.Person.Lastname
                                                    } : null,
                                                    Entity = (s.Subject.Entity != null) ? new Domain.Service.Entities.Entity
                                                    {
                                                        Bulstat = s.Subject.Entity.Bulstat,
                                                        Name = s.Subject.Entity.Name
                                                    } : null
                                                }).ToArray();

                var docModel = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.ElectronicDocument>()
                                        .Where(x => x.Gid == electronicDocumentId)
                                        .Select(x => new
                                        {
                                            x.NumberApply,
                                            x.BlobKeyDocumentApply,
                                            FileNameApply = x.BlobDocumentApply.FileName,
                                            FileSizeApply = x.BlobDocumentApply.BlobContentLocation.Size,
                                            x.BlobKeyTimeApply,
                                            FileNameTime = x.BlobTimeApply.FileName,
                                            FileSizeTime = x.BlobTimeApply.BlobContentLocation.Size,
                                            x.Id
                                        })
                                        .FirstOrDefault();

                var attachedFiles = upgradeRepository.AllReadonly<Domain.Entities.AttachedDocument>()
                                                .Where(x => x.ParentId == docModel.Id)
                                                .Where(x => x.AttachmentType == EpepConstants.AttachedTypes.ElectronicDocument)
                                                .Select(x => new ElectronicDocumentFile
                                                {
                                                    AttachmentType = x.AttachmentType,
                                                    Title = x.FileTitle,
                                                    FileName = x.FileName,
                                                    FileId = x.BlobKey,
                                                    FileSize = x.AttachedBlob.BlobContentLocation.Size
                                                }).ToList();

                if (docModel.BlobKeyDocumentApply != null)
                {
                    attachedFiles.Add(new ElectronicDocumentFile()
                    {
                        AttachmentType = AttachedTypes.ElectronicDocumentMain,
                        Title = $"Електронен пакет {docModel.NumberApply}",
                        FileName = docModel.FileNameApply,
                        FileSize = docModel.FileSizeApply,
                        FileId = docModel.BlobKeyDocumentApply.Value
                    });
                }
                if (docModel.BlobKeyTimeApply != null)
                {
                    attachedFiles.Add(new ElectronicDocumentFile()
                    {
                        AttachmentType = AttachedTypes.ElectronicDocumentTimestamp,
                        Title = "Удостоверение за време на подаване",
                        FileName = docModel.FileNameTime,
                        FileSize = docModel.FileSizeTime,
                        FileId = docModel.BlobKeyTimeApply.Value
                    });
                }

                foreach (var file in attachedFiles)
                {
                    file.Content = downloadFileFormPortal(file.FileId.ToString());
                    if (file.Content != null && file.AttachmentType == AttachedTypes.ElectronicDocumentMain 
                        && result.DateApply < new DateTime(2023,7,1))
                        file.FileSize = file.Content.Length;
                }

                result.Files = attachedFiles.ToArray();

                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Guid> GetNewElectronicDocumentIdentifiers(string courtCode)
        {
            return upgradeRepository.AllReadonly<Domain.Entities.Upgrade.ElectronicDocument>()
                                   .Include(x => x.Court)
                                   .Where(x => x.Court.Code == courtCode)
                                   .Where(x => x.DatePaid != null && x.DateCourtAccept == null)
                                   .Select(x => x.Gid)
                                   .ToList();
        }

        public bool UpdateElectronicDocumentSetDateCourtAccept(Guid electronicDocumentId, DateTime dateCourtAccept)
        {
            try
            {
                var model = upgradeRepository.FindByGid<Domain.Entities.Upgrade.ElectronicDocument>(electronicDocumentId);

                if (model != null && model.DateCourtAccept == null)
                {

                    using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                    {
                        model.DateCourtAccept = dateCourtAccept;

                        _unitOfWorks[DbKey.Main].Save();
                        transaction.Commit();
                    }
                    return true;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }

        public Domain.Service.Entities.UserAssignment GetUserAssignmentById(Guid userAssignmentId)
        {
            if (userAssignmentId == Guid.Empty)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(userAssignmentId)));

            var domainUserAssignment = upgradeRepository.FindByGid<eCase.Domain.Entities.Upgrade.UserAssignment>(userAssignmentId);
            if (domainUserAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(userAssignmentId)));

            var userAssignment = new Domain.Service.Entities.UserAssignment()
            {
                UserAssignmentId = domainUserAssignment.Gid,
                Date = domainUserAssignment.Date,
                AssignmentRole = domainUserAssignment.AssignmentRoleId ?? EpepConstants.UserAssignmentRoles.Side,
                SideId = _sideRepository.Find(domainUserAssignment.SideId).Gid,
                UserRegistrationId = upgradeRepository.GetById<eCase.Domain.Entities.Upgrade.UserRegistration>(domainUserAssignment.UserRegistrationId).Gid,
                IsActive = domainUserAssignment.IsActive
            };

            return userAssignment;
        }

        public Guid? InsertUserAssignment(UserAssignment userAssignment)
        {
            ValidateUserAssignment(userAssignment, false);
            int assignmentRole = (userAssignment.AssignmentRole > 0) ? userAssignment.AssignmentRole : EpepConstants.UserAssignmentRoles.Side;
            var existingAssignment = upgradeRepository.AllReadonly<Domain.Entities.Upgrade.UserAssignment>()
                                        .Include(x => x.UserRegistration)
                                        .Include(x => x.Side)
                                        .Where(x => x.UserRegistration.Gid == userAssignment.UserRegistrationId)
                                        .Where(x => x.Side.Gid == userAssignment.SideId)
                                        .Where(x => x.AssignmentRoleId == assignmentRole)
                                        .FirstOrDefault();

            if (existingAssignment != null)
            {
                return existingAssignment.Gid;
            }

            try
            {
                var _side = _sideRepository.FindByGid(userAssignment.SideId);
                var domainUserAssignment = new Domain.Entities.Upgrade.UserAssignment()
                {
                    Gid = userAssignment.UserAssignmentId.HasValue ? userAssignment.UserAssignmentId.Value : Guid.NewGuid(),
                    Date = userAssignment.Date,
                    CaseId = _side.CaseId,
                    SideId = _side.SideId,
                    AssignmentRoleId = assignmentRole,
                    UserRegistrationId = upgradeRepository.FindByGid<Domain.Entities.Upgrade.UserRegistration>(userAssignment.UserRegistrationId).Id,
                    IsActive = userAssignment.IsActive,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                };

                var userRegistration = upgradeRepository.GetById<Domain.Entities.Upgrade.UserRegistration>(domainUserAssignment.UserRegistrationId);

                if (userRegistration != null)
                {
                    var domainCase = _caseRepository.Find(_side.CaseId);

                    if (domainCase != null)
                    {
                        var courtName = _courtRepository.GetNom(domainCase.CourtId).Name;
                        if (SendCaseAccessMail)
                        {
                            domainUserAssignment.GetCaseAccess(userRegistration.NotificationEmail ?? userRegistration.Email, domainCase.Abbreviation, courtName);
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    upgradeRepository.Add(domainUserAssignment);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainUserAssignment.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }
        public Guid? UpdateUserAssignment(UserAssignment userAssignment)
        {
            ValidateUserAssignment(userAssignment, true);

            try
            {
                var _side = _sideRepository.FindByGid(userAssignment.SideId);
                var domainUserAssignment = upgradeRepository.FindByGid<Domain.Entities.Upgrade.UserAssignment>(userAssignment.UserAssignmentId.Value);


                var userRegistration = upgradeRepository.GetById<Domain.Entities.Upgrade.UserRegistration>(domainUserAssignment.UserRegistrationId);

                if (userRegistration != null)
                {
                    var domainCase = _caseRepository.Find(_side.CaseId);

                    if (domainCase != null)
                    {
                        var courtName = _courtRepository.GetNom(domainCase.CourtId).Name;
                        if (SendCaseAccessMail)
                        {
                            domainUserAssignment.ChangeCaseAccess(userRegistration.NotificationEmail ?? userRegistration.Email, domainCase.Abbreviation, courtName);
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    domainUserAssignment.Date = userAssignment.Date;
                    domainUserAssignment.CaseId = _side.CaseId;
                    domainUserAssignment.SideId = _side.SideId;
                    domainUserAssignment.AssignmentRoleId = (userAssignment.AssignmentRole > 0) ? userAssignment.AssignmentRole : EpepConstants.UserAssignmentRoles.Side;
                    domainUserAssignment.IsActive = userAssignment.IsActive;
                    domainUserAssignment.ModifyDate = DateTime.Now;

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();
                }

                return domainUserAssignment.Gid;
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }
        public bool DeleteUserAssignment(Guid userAssignmentId)
        {
            var domainUserAssignment = upgradeRepository.FindByGid<Domain.Entities.Upgrade.UserAssignment>(userAssignmentId);
            if (domainUserAssignment == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, nameof(userAssignmentId)));

            try
            {
                var userRegistration = upgradeRepository.GetById<Domain.Entities.Upgrade.UserRegistration>(domainUserAssignment.UserRegistrationId);

                if (userRegistration != null)
                {

                    var domainSide = _sideRepository.Find(domainUserAssignment.SideId);

                    if (domainSide != null)
                    {
                        var domainCase = _caseRepository.Find(domainSide.CaseId);

                        if (domainCase != null && domainCase.Court != null)
                        {
                            domainUserAssignment.ChangeCaseAccess(userRegistration.NotificationEmail ?? userRegistration.Email, domainCase.Abbreviation, domainCase.Court.Name);
                        }
                    }
                }

                using (var transaction = _unitOfWorks[DbKey.Main].BeginTransaction())
                {
                    upgradeRepository.Delete(domainUserAssignment);

                    _unitOfWorks[DbKey.Main].Save();
                    transaction.Commit();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw HandleCommonException(ex);
            }
        }
        private void ValidateUserAssignment(UserAssignment assignment, bool isUpdate)
        {
            this.ValidateId<Domain.Entities.Upgrade.UserAssignment>(assignment.UserAssignmentId, isUpdate);

            if (assignment.Date == default(DateTime))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "UserAssignment.Date"));

            if (assignment.SideId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "UserAssignment.SideId"));
            if (_sideRepository.FindByGid(assignment.SideId) == null)
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "UserAssignment.SideId"));


            if (assignment.UserRegistrationId == default(Guid))
                throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "UserAssignment.UserRegistrationId"));
            if (upgradeRepository.FindByGid<Domain.Entities.Upgrade.UserRegistration>(assignment.UserRegistrationId) == null)
            {
                var forException = true;
                if (isUpdate)
                {
                    forException = !upgradeRepository.AllReadonly<Domain.Entities.Upgrade.UserAssignment>()
                                        .Where(x => x.Gid == assignment.UserAssignmentId && x.LegacyGid == assignment.UserRegistrationId)
                                        .Any();
                }
                if (forException)
                {
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.NotExists, "UserAssignment.UserRegistrationId"));
                }
            }


            if (assignment.AssignmentRole > 0)
            {
                if (!EpepConstants.UserAssignmentRoles.AcceptedRoles.Contains(assignment.AssignmentRole))
                {
                    throw new FaultException<InfocaseFault>(new InfocaseFault(FaultCode.InvalidValue, "UserAssignment.AssignmentRole"));
                }
            }
            else
            {
                assignment.AssignmentRole = 0;
            }
        }

        private byte[] downloadFileFormPortal(string fileId)
        {
            var baseUrl = ConfigurationManager.AppSettings["eCase.Service:PortalFileDownloadUrl"];
            using (var cln = new WebClient())
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
                byte[] fileBytes = cln.DownloadData(baseUrl + fileId);
                return fileBytes;
            }
        }

    }

    #endregion
}
