using eCase.Domain.Service.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace eCase.Domain.Service
{
    [ServiceContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public interface IeCaseService
    {
        /// <summary>
        /// Въвеждане на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertIncomingDocument(IncomingDocument incomingDocument);

        /// <summary>
        /// Редактиране на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateIncomingDocument(IncomingDocument incomingDocument);

        /// <summary>
        /// Въвеждане на файл на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertIncomingDocumentFile(IncomingDocumentFile incomingDocumentFile);

        /// <summary>
        /// Редактиране на файл на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateIncomingDocumentFile(IncomingDocumentFile incomingDocumentFile);

        /// <summary>
        /// Въвеждане на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertOutgoingDocument(OutgoingDocument outgoingDocument);

        /// <summary>
        /// Редактиране на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateOutgoingDocument(OutgoingDocument outgoingDocument);

        /// <summary>
        /// Въвеждане на файл на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertOutgoingDocumentFile(OutgoingDocumentFile outgoingDocumentFile);

        /// <summary>
        /// Редактиране на файл на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateOutgoingDocumentFile(OutgoingDocumentFile outgoingDocumentFile);

        /// <summary>
        /// Въвеждане на дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertCase(Case c);

        /// <summary>
        /// Редакция на дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateCase(Case c);

        /// <summary>
        /// Извеждане ID на дело по съд, номер и година на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetCaseId(int incDocumentNumber, int incDocumentYear, string courtCode);

        /// <summary>
        /// Въвеждане на свързано дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertConnectedCase(ConnectedCase connectedCase);

        /// <summary>
        /// Редактиране на свързано дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateConnectedCase(ConnectedCase connectedCase);

        /// <summary>
        /// Въвеждане на съдия докладчик
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertReporter(Reporter reporter);

        /// <summary>
        /// Редакция на съдия докладчик
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateReporter(Reporter reporter);

        /// <summary>
        /// Въвеждане на протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertAssignment(Assignment assignment);

        /// <summary>
        /// Редакция на заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateAssignment(Assignment assignment);

        /// <summary>
        /// Въвеждане на файл на протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertAssignmentFile(AssignmentFile assignmentFile);

        /// <summary>
        /// Редактиране на файл на протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateAssignmentFile(AssignmentFile assignmentFile);

        /// <summary>
        /// Въвеждане на заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertHearing(Hearing hearing);

        /// <summary>
        /// Редакция на заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateHearing(Hearing hearing);

        /// <summary>
        /// Въвеждане на файл на протокол от заседание без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertPrivateProtocolFile(PrivateProtocolFile privateProtocolFile);

        /// <summary>
        /// Реактиране на файл на протокол от заседание без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdatePrivateProtocolFile(PrivateProtocolFile privateProtocolFile);

        /// <summary>
        /// Въвеждане на файл на протокол от заседание със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertPublicProtocolFile(PublicProtocolFile publicProtocolFile);

        /// <summary>
        /// Редактиране на файл на протокол от заседание със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdatePublicProtocolFile(PublicProtocolFile publicProtocolFile);

        /// <summary>
        /// Въвеждане на участник в заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertHearingParticipant(HearingParticipant hearingParticipant);

        /// <summary>
        /// Редакция на участник в заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateHearingParticipant(HearingParticipant hearingParticipant);

        /// <summary>
        /// Въвеждане на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertAct(Act act);

        /// <summary>
        /// Редакция на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateAct(Act act);

        /// <summary>
        /// Въвеждане на файл на съдебен акт със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertPublicActFile(PublicActFile publicActFile);

        /// <summary>
        /// Редактиране на файл на съдебен акт със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdatePublicActFile(PublicActFile publicActFile);

        /// <summary>
        /// Въвеждане на файл на съдебен акт без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertPrivateActFile(PrivateActFile privateActFile);

        /// <summary>
        /// Редактиране на файл на съдебен акт без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdatePrivateActFile(PrivateActFile privateActFile);

        /// <summary>
        /// Въвеждане на файл на мотив със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertPublicMotiveFile(PublicMotiveFile publicMotiveFile);

        /// <summary>
        /// Редактиране на файл на мотив със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdatePublicMotiveFile(PublicMotiveFile publicMotiveFile);

        /// <summary>
        /// Въвеждане на файл на мотив без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertPrivateMotiveFile(PrivateMotiveFile privateMotiveFile);

        /// <summary>
        /// Редактиране на файл на мотив без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdatePrivateMotiveFile(PrivateMotiveFile privateMotiveFile);

        /// <summary>
        /// Въвеждане на подготовчик на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertActPreparator(ActPreparator actPreparator);

        /// <summary>
        /// Редакция на подготовчик на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateActPreparator(ActPreparator actPreparator);

        /// <summary>
        /// Въвеждане на обжалване на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertAppeal(Appeal appeal);

        /// <summary>
        /// Редакция на обжалване на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateAppeal(Appeal appeal);

        /// <summary>
        /// Въвеждане на произнасяне на състава с отражение по хода на делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertCaseRuling(CaseRuling caseRuling);

        /// <summary>
        /// Редакция на произнасяне на състава с отражение по хода на делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateCaseRuling(CaseRuling caseRuling);

        /// <summary>
        /// Въвеждане на страна по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertSide(Side side);

        /// <summary>
        /// Редакция на страна по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateSide(Side side);

        /// <summary>
        /// Въвеждане на назначение на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertLawyerAssignment(LawyerAssignment lawyerAssignment);

        /// <summary>
        /// Редакция на назначение на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateLawyerAssignment(LawyerAssignment lawyerAssignment);

        /// <summary>
        /// Въвеждане на призовка/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertSummon(Summon summon, Guid userId);

        /// <summary>
        /// Редакция на призовка/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateSummon(Summon summon);

        /// <summary>
        /// Въвеждане на файл на призовка/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertSummonFile(SummonFile summonFile);

        /// <summary>
        /// Редактиране на файл на призовка/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateSummonFile(SummonFile summonFile);

        /// <summary>
        /// Извличане на всички адвокати
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Lawyer> GetAllLawyers();

        /// <summary>
        /// Извличане на всички адвокати, добавени след определена дата
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Lawyer> GetAllNewLawyers(DateTime from);

        /// <summary>
        /// Извличане на адвокат чрез номер
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Lawyer GetLawyerByNumber(string number);

        /// <summary>
        /// Извличане на връчените призовки по съд и дати от-до / съобщения
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetAllServedSummonsByCourt(string courtcode, DateTime? from, DateTime? to);

        /// <summary>
        /// Проверка на дата на връчване призовка по GUID
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        DateTime GetSummonsServedTimestamp(Guid guid);

        /// <summary>
        /// Извличане на прочетените призовки по съд и дати от-до / съобщения
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetAllReadSummonsByCourt(string courtcode, DateTime? from, DateTime? to);

        /// <summary>
        /// Проверка на дата на прочит призовка по GUID
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        DateTime GetSummonsReadTimestamp(Guid guid);

        /// <summary>
        /// Маркиране на призовка като прочетена
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? MarkSummonAsRead(Guid guid, DateTime? date);

        /// <summary>
        /// Извличане на прочетените призовки / съобщения за определен ден
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetReadSummonsForCertainDay(DateTime date);

        /// <summary>
        /// Извилачане на отчет за доставено съобщение по електронен път
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        byte[] GetSummonReportDocument(Guid summonId);

        /// <summary>
        /// Активиране на достъп до електронни призовки и съобщения
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        void ActivateSummonsAccess(Guid userId, Guid caseId);

        /// <summary>
        /// Деактивиране на достъп до електронни призовки и съобщения
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        void DeactivateSummonsAccess(Guid userId, Guid caseId);

        /// <summary>
        /// Въвеждане на назначение на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertPersonAssignment(PersonAssignment personAssignment);

        /// <summary>
        /// Редакция на назначение на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdatePersonAssignment(PersonAssignment personAssignment);

        /// <summary>
        /// Въвеждане на сканиран документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertScannedDocument(ScannedDocument scannedDocument);

        /// <summary>
        /// Редактиране на сканиран документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateScannedDocument(ScannedDocument scannedDocument);

        /// <summary>
        /// Въвеждане на регистрация на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertLawyerRegistration(LawyerRegistration lawyerRegistration);

        /// <summary>
        /// Редакция на регистрация на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateLawyerRegistration(LawyerRegistration lawyerRegistration);

        /// <summary>
        /// Активиране на регистрация на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? ActivateLawyerRegistration(Guid lawyerRegistrationId);

        /// <summary>
        /// Деактивиране на регистрация на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? DeactivateLawyerRegistration(Guid lawyerRegistrationId);

        /// <summary>
        /// Въвеждане на регистрация на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertPersonRegistration(PersonRegistration personRegistration);

        /// <summary>
        /// Редакция на регистрация на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdatePersonRegistration(PersonRegistration personRegistration);

        /// <summary>
        /// Активиране на регистрация на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? ActivatePersonRegistration(Guid personRegistrationId);

        /// <summary>
        /// Деактивиране на регистрация на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? DeactivatePersonRegistration(Guid personRegistrationId);

        /// <summary>
        /// Извличане на информация за тип потребител
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        UserRegistrationInfo GetUserRegistrationInfoByUsername(string username);

        /// <summary>
        /// Подмяна на потребителско име / електронна поща
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateUsername(string oldUsername, string newUsername, string courtCode);

        /// <summary>
        /// Извличане на уникални идентификатори на дела
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetCaseIdentifiers(int? caseNumber, string caseKindCode, int? caseYear, string department);

        ///////////////////////////////////////////////////////
        ///////////     Delete methods     ////////////////////
        ///////////////////////////////////////////////////////

        /// <summary>
        /// Изтриване на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteIncomingDocument(Guid incomingDocumentId);

        /// <summary>
        /// Изтриване на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteOutgoingDocument(Guid outgoingDocumentId);

        /// <summary>
        /// Изтриване на дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteCase(Guid caseId);

        /// <summary>
        /// Изтриване на свързано дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteConnectedCase(Guid connectedCaseId);

        /// <summary>
        /// Изтриване на съдия докладчик
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteReporter(Guid reporterId);

        /// <summary>
        /// Изтриване на Протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteAssignment(Guid assignmentId);

        /// <summary>
        /// Изтриване на заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteHearing(Guid hearingId);

        /// <summary>
        /// Изтриване на участник в заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteHearingParticipant(Guid hearingParticipantId);

        /// <summary>
        /// Изтриване на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteAct(Guid actId);

        /// <summary>
        /// Изтриване на подготовчик на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteActPreparator(Guid actPreparatorId);

        /// <summary>
        /// Изтриване на обжалване на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteAppeal(Guid appealId);

        /// <summary>
        /// Изтриване на произнасяне на състава с отражение по хода на делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteCaseRuling(Guid caseRulingId);

        /// <summary>
        /// Изтриване на страна по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteSide(Guid sideId);

        /// <summary>
        /// Изтриване на назначение на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteLawyerAssignment(Guid lawyerAssignmentId);

        /// <summary>
        /// Изтриване на призовката/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteSummon(Guid summonId);

        /// <summary>
        /// Изтриване на назначение на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeletePersonAssignment(Guid personAssignmentId);

        /// <summary>
        /// Изтриване на сканиран документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteScannedDocument(Guid scannedDocumentId);

        /// <summary>
        /// Изтриване на протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteAssignmentFile(Guid assignmentId);

        /// <summary>
        /// Изтриване на файл на протокол от заседание без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeletePrivateProtocolFile(Guid hearingId);

        /// <summary>
        /// Изтриване на файл на протокол от заседание със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeletePublicProtocolFile(Guid hearingId);

        /// <summary>
        /// Изтриване на файл на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteIncomingDocumentFile(Guid incomingDocumentId);

        /// <summary>
        /// Изтриване на файл на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteOutgoingDocumentFile(Guid outgoingDocumentId);

        /// <summary>
        /// Изтриване на файл на призовка/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteSummonFile(Guid summonId);

        /// <summary>
        /// Изтриване на файл на съдебен акт без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeletePrivateActFile(Guid actId);

        /// <summary>
        /// Изтриване на файл на съдебен акт със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeletePublicActFile(Guid actId);

        /// <summary>
        /// Изтриване на файл на мотив без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeletePrivateMotiveFile(Guid actId);

        /// <summary>
        /// Изтриване на файл на мотив със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeletePublicMotiveFile(Guid actId);

        ///////////////////////////////////////////////////////
        ///////////     Get methods        ////////////////////
        ///////////////////////////////////////////////////////

        /// <summary>
        /// Извличане на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        IncomingDocument GetIncomingDocumentById(Guid incomingDocumentId);

        /// <summary>
        /// Извличане на списък с идентификатори на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetIncomingDocumentIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        OutgoingDocument GetOutgoingDocumentById(Guid outgoingDocumentId);

        /// <summary>
        /// Извличане на списък с идентификатори на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetOutgoingDocumentIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Case GetCaseById(Guid caseId);

        /// <summary>
        /// Извличане на свързано дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        ConnectedCase GetConnectedCaseById(Guid connectedCaseId);

        /// <summary>
        /// Извличане на списък с идентификатори на свързано дело
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetConnectedCaseIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на съдия докладчик
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Reporter GetReporterById(Guid reporterId);

        /// <summary>
        /// Извличане на списък с идентификатори на съдия докладчик
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetReporterIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на Протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Assignment GetAssignmentById(Guid assignmentId);

        /// <summary>
        /// Извличане на списък с идентификатори на Протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetAssignmentIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Hearing GetHearingById(Guid hearingId);

        /// <summary>
        /// Извличане на списък с идентификатори на заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetHearingIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на участник в заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        HearingParticipant GetHearingParticipantById(Guid hearingParticipantId);

        /// <summary>
        /// Извличане списък с идентификатори на участник в заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetHearingParticipantIdentifiersByHearingId(Guid hearingId);

        /// <summary>
        /// Извличане на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Act GetActById(Guid actId);

        /// <summary>
        /// Извличане на списък с идентификатори на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetActIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на подготовчик на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        ActPreparator GetActPreparatorById(Guid actPreparatorId);

        /// <summary>
        /// Извличане на списък с идентификатори на подготовчик на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetActPreparatorIdentifiersByActId(Guid actId);

        /// <summary>
        /// Извличане на обжалване на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Appeal GetAppealById(Guid appealId);

        /// <summary>
        /// Извличане на списък с идентификатори на обжалване на съдебен акт
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetAppealIdentifiersByActId(Guid actId);

        /// <summary>
        /// Извличане на произнасяне на състава с отражение по хода на делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        CaseRuling GetCaseRulingById(Guid caseRulingId);

        /// <summary>
        /// Извличане на списък с идентификатори на произнасяне на състава с отражение по хода на делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetCaseRulingIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на страна по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Side GetSideById(Guid sideId);

        /// <summary>
        /// Извличане на списък с идентификатори на страна по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetSideIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на назначение на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        LawyerAssignment GetLawyerAssignmentById(Guid lawyerAssignmentId);

        /// <summary>
        /// Извличане на списък с идентификатори на назначение на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetLawyerAssignmentIdentifiersBySideId(Guid sideId);

        /// <summary>
        /// Извличане на назначение на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PersonAssignment GetPersonAssignmentById(Guid personAssignmentId);

        /// <summary>
        /// Извличане на списък с идентификатори на назначение на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetPersonAssignmentIdentifiersBySideId(Guid sideId);

        /// <summary>
        /// Извличане на регистрация на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        LawyerRegistration GetLawyerRegistrationById(Guid lawyerRegistrationId);

        /// <summary>
        /// Извличане на регистрация на адвокат по номер
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        LawyerRegistration SelectLawyerRegistration(String lawyerNumber);


        /// <summary>
        /// Извличане на списък с идентификатори на регистрация на адвокат
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetLawyerRegistrationIdentifiersByLawyerId(Guid lawyerId);

        /// <summary>
        /// Извличане на регистрация на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PersonRegistration GetPersonRegistrationById(Guid personRegistrationId);

        /// <summary>
        /// Извличане на регистрация на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PersonRegistration SelectPersonRegistration(String EGN);


        /// <summary>
        /// Извличане на идентификатор на регистрация на физическо лице
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetPersonRegistrationIdentifierByPersonAssignmentId(Guid personassignmentId);

        /// <summary>
        /// Извличане на призовката/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Summon GetSummonById(Guid summonId);

        /// <summary>
        /// Извличане на списък с идентификатори на призовката/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetSummonIdentifiersByParentId(Guid parentId, string summonTypeCode);

        /// <summary>
        /// Извличане на сканиран документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        ScannedDocument GetScannedDocumentById(Guid scannedDocumentId);

        /// <summary>
        /// Извличане на списък с идентификатори на сканиран документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetScannedDocumentIdentifiersByCaseId(Guid caseId);

        /// <summary>
        /// Извличане на протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        AssignmentFile GetAssignmentFileById(Guid assignmentFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на протокол за разпределение по делото
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetAssignmentFileIdentifiersByAssignmentId(Guid assignmentId);

        /// <summary>
        /// Извличане на файл на протокол от заседание без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PrivateProtocolFile GetPrivateProtocolFileById(Guid privateProtocolId);

        /// <summary>
        /// Извличане на идентификатор на файл на протокол от заседание без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetPrivateProtocolFileIdentifierByHearingId(Guid hearingId);

        /// <summary>
        /// Извличане на файл на протокол от заседание със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PublicProtocolFile GetPublicProtocolFileById(Guid publicProtocolFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на протокол от заседание със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetPublicProtocolFileIdentifierByHearingId(Guid hearingId);

        /// <summary>
        /// Извличане на файл на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        IncomingDocumentFile GetIncomingDocumentFileById(Guid incomingDocumentFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на входящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetIncomingDocumentFileIdentifierByIncomingDocumentId(Guid incomingDocumentId);

        /// <summary>
        /// Извличане на файл на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        OutgoingDocumentFile GetOutgoingDocumentFileById(Guid outgoingDocumentFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на изходящ документ
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetOutgoingDocumentFileIdentifierByOutgoingDocumentId(Guid outgoingDocumentId);

        /// <summary>
        /// Извличане на файл на призовка/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        SummonFile GetSummonFileById(Guid summonFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на призовка/съобщението
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetSummonFileIdentifierBySummonId(Guid summonId);

        /// <summary>
        /// Извличане на файл на съдебен акт без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PrivateActFile GetPrivateActFileById(Guid privateActFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на съдебен акт без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetPrivateActFileIdentifiersByActId(Guid actId);

        /// <summary>
        /// Извличане на файл на съдебен акт със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PublicActFile GetPublicActFileById(Guid publicActFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на съдебен акт със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetPublicActFileIdentifierByActId(Guid actId);

        /// <summary>
        /// Извличане на файл на мотив без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PrivateMotiveFile GetPrivateMotiveFileById(Guid privateMotiveFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на мотив без заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetPrivateMotiveFileIdentifierByActId(Guid actId);

        /// <summary>
        /// Извличане на файл на мотив със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        PublicMotiveFile GetPublicMotiveFileById(Guid publicActFileId);

        /// <summary>
        /// Извличане на идентификатор на файл на мотив със заличени данни
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? GetPublicMotiveFileIdentifierByActId(Guid actId);

        /// <summary>
        /// Въвеждане на файл към основен обект
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertAttachedDocument(AttachedDocument attachedDocument);

        /// <summary>
        /// Изтриване на файл към основен обект
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteAttachedDocument(Guid attachedDocumentId);

        /// <summary>
        /// Въвеждане на документ, приложен в заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertHearingDocument(HearingDocument hearingDocument);

        /// <summary>
        /// Редактиране на документ, приложен в заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateHearingDocument(HearingDocument hearingDocument);

        /// <summary>
        /// Премахване на документ, приложен в заседание
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))] 
        bool DeleteHearingDocument(Guid hearingDocumentId);

        /// <summary>
        /// eCase2023, Добавяне на достъп до дело на потребител
        /// </summary>
        /// <param name="userAssignment"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? InsertUserAssignment(UserAssignment userAssignment);
        
        /// <summary>
        /// eCase2023, Редакция на достъп до дело на потребител
        /// </summary>
        /// <param name="userAssignment"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? UpdateUserAssignment(UserAssignment userAssignment);

        /// <summary>
        /// eCase2023, Премахване на достъп до дело на потребител
        /// </summary>
        /// <param name="userAssignmentId"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool DeleteUserAssignment(Guid userAssignmentId);

        /// <summary>
        /// eCase2023, извлича данни за всички потребители, модифицирани след modifyFromDate
        /// </summary>
        /// <param name="modifyFromDate"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))] 
        List<UserRegistration> GetUserRegistrations(DateTime? modifyFromDate = null);

        /// <summary>
        /// Извлича данни да дата на връчване/прочитане на призовка, като проверява за наличие на отсъствия на адвокати ЕПЕП2023
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        SummonReadTimeResult GetSummonsReadTimestampV3(Guid guid);

        /// <summary>
        /// Извлича данни да дата на връчване/прочитане на призовка, като проверява за наличие на отсъствия на адвокати ЕПЕП2023
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        bool UpdateElectronicDocumentSetDateCourtAccept(Guid electronicDocumentId, DateTime dateCourtAccept);
        
        /// <summary>
        /// Връща списък от всички идентификатори на електронно подадени документи, които още не са приети за обработка
        /// </summary>
        /// <param name="courtCode"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetNewElectronicDocumentIdentifiers(string courtCode);

        /// <summary>
        /// Извлича данни електронно подаден документ по идентификатор ЕПЕП2023
        /// </summary>
        /// <param name="electronicDocumentId"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        ElectronicDocument GetElectronicDocument(Guid electronicDocumentId);

        /// <summary>
        /// Маркира призовка като автоматично отбелязана като прочетена след изтичане на определен срок
        /// </summary>
        /// <param name="summonId"></param>
        /// <param name="courtReadDate"></param>
        /// <param name="courtDescription"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Guid? MarkSummonAsCourtRead(Guid summonId, DateTime courtReadDate, string courtDescription);

        /// <summary>
        /// Извлича данни за достъп на потребител по Gid
        /// </summary>
        /// <param name="userAssignmentId">UserAssignmentId</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        Domain.Service.Entities.UserAssignment GetUserAssignmentById(Guid userAssignmentId);

        /// <summary>
        /// Извлича данни за достъп на потребител по Gid
        /// </summary>
        /// <param name="court">Код на съд</param>
        /// <param name="caseType">Основен вид дело</param>
        /// <param name="caseNumber">Номер дело</param>
        /// <param name="caseYear">Година дело</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetCaseIdentifiersByNumber(string court, string caseType, int caseNumber, int caseYear);

        /// <summary>
        /// Извлича идентификатори на прикачени документи по тип и идентификатор на основен обект
        /// </summary>
        /// <param name="type">Тип обект</param>
        /// <param name="parentId">идентификатор на основен обект</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetAttachedDocumentIdentifiers(int type, Guid parentId);

        /// <summary>
        /// Връща данни и съдържание на прикачен документ
        /// </summary>
        /// <param name="attachedDocumentId"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        AttachedDocument GetAttachedDocumentById(Guid attachedDocumentId);

        /// <summary>
        /// Изтегля списък на идентификатори на документи, приложени в заседанието по негов идентификатор
        /// </summary>
        /// <param name="hearingId">Идентификатор на заседание</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        List<Guid> GetHearingDocumentIdentifiersByHearingId(Guid hearingId);

        /// <summary>
        /// Изтегля данни за документ, приложен в заседание
        /// </summary>
        /// <param name="hearingDocumentId"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(InfocaseFault))]
        HearingDocument GetHearingDocumentById(Guid hearingDocumentId);
    }

    /// <summary>
    /// code:0; reason: Exception.Message
    /// code:1; reason: Полето е задължително
    /// code:2; reason: Невалидна стойност
    /// code:3; reason: Не съществува
    /// </summary>
    [DataContract(Namespace = "http://www.abbaty.com/eCase/v2")]
    public class InfocaseFault
    {
        int _code;
        string _reason;
        static string[] _msgs = { "Неидентифицирана грешка", "Полето е задължително", "Невалидна стойност", "Не съществува" };
        public string _errorField;

        public InfocaseFault(string message)
        {
            _code = 0;
            _reason = message;
            this._errorField = "";
        }

        public InfocaseFault(FaultCode code, string field)
        {
            this._code = (int)code;
            this._errorField = field;
            this._reason = _msgs[_code];
        }

        public InfocaseFault(int code, string message, string field)
        {
            this._code = code;
            this._errorField = field;
            this._reason = message;
        }

        [DataMember]
        public int ErrorCode
        {
            get { return _code; }
            set { _code = value; }
        }

        [DataMember]
        public string ErrorField
        {
            get { return _errorField; }
            set { _errorField = value; }
        }

        [DataMember]
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }
    }

    public enum FaultCode
    {
        Unknown = 0,
        RequiredField = 1,
        InvalidValue = 2,
        NotExists = 3
    }
}
