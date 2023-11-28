using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Document;
using IO.Timestamp.Client.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Net;

namespace Epep.Core.Services
{
    public class DocumentService : BaseService, IDocumentService
    {
        private readonly IUserContext userContext;
        private readonly IBlobService blobService;
        private readonly ICaseService caseService;
        private readonly IPricelistService pricelistService;
        private readonly ITimestampClient timestampClient;
        private readonly IPaymentService paymentService;

        public DocumentService(
           IRepository _repo,
           IBlobService _blobService,
           ICaseService caseService,
           IPricelistService pricelistService,
           IPaymentService paymentService,
           ITimestampClient timestampClient,
           ILogger<DocumentService> _logger,
           IUserContext userContext)
        {
            this.repo = _repo;
            blobService = _blobService;
            this.logger = _logger;
            this.caseService = caseService;
            this.pricelistService = pricelistService;
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
            this.timestampClient = timestampClient;
            this.userContext = userContext;
            this.paymentService = paymentService;
        }

        public async Task<bool> CanAccess(Guid gid)
        {
            var model = await GetByGidAsync<ElectronicDocument>(gid);
            if (model == null)
            {
                return false;
            }

            return CanAccess(model);
        }

        public bool CanAccess(ElectronicDocument document)
        {
            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                    return document.UserId == userContext.OrganizationUserId;
                default:
                    return document.CreateUserId == userContext.UserId;
            }
        }

        public IQueryable<DocumentListVM> Select(FilterDocumentListVM filter)
        {
            Expression<Func<ElectronicDocument, bool>> whereMyDocuments = x => x.UserId == userContext.UserId;
            if (NomenclatureConstants.UserTypes.OrganizationTypes.Contains(userContext.UserType))
            {
                whereMyDocuments = x => x.UserId == userContext.OrganizationUserId;
            }

            filter.Sanitize();
            Expression<Func<ElectronicDocument, bool>> whereCourt = x => true;
            if (filter.CourtId > 0)
            {
                whereCourt = x => x.CourtId == filter.CourtId;
            }
            Expression<Func<ElectronicDocument, bool>> whereDocumentType = x => true;
            if (filter.ElectronicDocumentTypeId > 0)
            {
                whereDocumentType = x => x.ElectronicDocumentTypeId == filter.ElectronicDocumentTypeId;
            }
            Expression<Func<ElectronicDocument, bool>> whereApplyNumber = x => true;
            if (!string.IsNullOrEmpty(filter.NumberApply))
            {
                whereApplyNumber = x => EF.Functions.Like(x.NumberApply, filter.NumberApply.ToPaternSearch());
            }
            Expression<Func<ElectronicDocument, bool>> whereApplyFrom = x => true;
            if (filter.DateApplyFrom.HasValue)
            {
                whereApplyFrom = x => x.DateApply >= filter.DateApplyFrom.Value;
            }
            Expression<Func<ElectronicDocument, bool>> whereApplyTo = x => true;
            if (filter.DateApplyTo.HasValue)
            {
                whereApplyTo = x => x.DateApply <= filter.DateApplyTo.MakeEndDate();
            }

            Expression<Func<ElectronicDocument, bool>> whereCaseNumber = x => true;
            if (filter.CaseNumber > 0)
            {
                whereCaseNumber = x => x.Case.Number == filter.CaseNumber;
            }
            Expression<Func<ElectronicDocument, bool>> whereCaseYear = x => true;
            if (filter.CaseYear > 0)
            {
                whereCaseYear = x => x.Case.CaseYear == filter.CaseYear;
            }

            return repo.AllReadonly<ElectronicDocument>()
                            .AsSplitQuery()
                            .Where(whereMyDocuments)
                            .Where(whereCourt)
                            .Where(whereDocumentType)
                            .Where(whereApplyNumber)
                            .Where(whereApplyFrom)
                            .Where(whereApplyTo)
                            .Where(whereCaseNumber)
                            .Where(whereCaseYear)
                            .OrderByDescending(x => x.DateApply)
                            .OrderByDescending(x => x.ModifyDate)
                            .Select(x => new DocumentListVM
                            {
                                Gid = x.Gid,
                                CourtName = x.CourtId > 0 ? x.Court.Name : "",
                                DocumentTypeName = x.ElectronicDocumentTypeId > 0 ? x.ElectronicDocumentType.Name : "",
                                NumberApply = x.NumberApply,
                                ModifyDate = x.ModifyDate,
                                DateApply = x.DateApply,
                                DatePaid = x.DatePaid,
                                DateCourtAccept = x.DateCourtAccept,
                                Sides = x.Sides.Select(s => new ElectronicDocumentSideListVM
                                {
                                    FullName = s.Subject.Name,
                                    SideInvolvementKindName = s.SideInvolvementKind.Name
                                }).ToArray(),
                                DocInfo = (x.CourtDocumentDate != null) ? new DocumentItemInfoVM()
                                {
                                    NumberText = x.CourtDocumentNumber,
                                    //Gid = d.Gid,
                                    Date = x.CourtDocumentDate
                                } : null

                                //,
                                //CaseInfo = (x.CaseId > 0) ? new DocumentItemInfoVM
                                //{
                                //    Gid = x.Case.Gid,
                                //    TypeName = x.Case.CaseKind.Label,
                                //    Number = x.Case.Number,
                                //    Year = x.Case.CaseYear
                                //} : x.IncomingDocuments.SelectMany(d => d.Cases).Select(c => new DocumentItemInfoVM
                                //{
                                //    Gid = c.Gid,
                                //    TypeName = c.CaseKind.Label,
                                //    Number = c.Number,
                                //    Year = c.CaseYear
                                //}

                                //).FirstOrDefault()
                            });
        }

        public async Task<SaveResultVM> InitDocument(Guid? caseGid = null, Guid? sideGid = null, string description = null)
        {
            var newDocument = new ElectronicDocument()
            {
                Gid = Guid.NewGuid(),
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                UserId = userContext.UserId,
                CreateUserId = userContext.UserId,
                Description = description
            };
            if (caseGid == null)
            {
                //Иницииращ документ
                newDocument.DocumentKind = NomenclatureConstants.DocumentKinds.Initial;
            }
            else
            {
                if (sideGid != null)
                {
                    newDocument.DocumentKind = NomenclatureConstants.DocumentKinds.SideDoc;
                }
                else
                {
                    newDocument.DocumentKind = NomenclatureConstants.DocumentKinds.Compliant;
                }
            }
            if (NomenclatureConstants.UserTypes.OrganizationTypes.Contains(userContext.UserType))
            {
                newDocument.UserId = userContext.OrganizationUserId.Value;
            }
            if (caseGid != null)
            {
                var caseModel = await GetByGidAsync<Case>(caseGid.Value);
                //if (!await caseService.CheckCaseAccess(caseModel.Gid))
                //{
                //    return new SaveResultVM(false, "Нямате достъп до избраното дело!");
                //}
                //else
                //{
                newDocument.CaseId = caseModel.CaseId;
                newDocument.CourtId = caseModel.CourtId;
                //}
                if (sideGid != null)
                {
                    var sideModel = await GetByGidAsync<Side>(sideGid.Value);
                    if (sideModel == null)
                    {
                        return new SaveResultVM(false, "Невалиден идентификатор на страна!");
                    }
                    else
                    {
                        newDocument.SideId = sideModel.SideId;
                    }
                }
            }

            newDocument.Sides = await InitSidesFromUser();


            try
            {
                await repo.AddAsync(newDocument);
                await repo.SaveChangesAsync();
                return new SaveResultVM()
                {
                    Result = true,
                    ObjectId = newDocument.Gid
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "InitDocument");
                return new SaveResultVM(false);
            }
        }

        private async Task<IList<ElectronicDocumentSide>> InitSidesFromUser()
        {
            var result = new List<ElectronicDocumentSide>();
            var dtNow = DateTime.Now;
            var userSide = new ElectronicDocumentSide()
            {
                Gid = Guid.NewGuid(),
                SideInvolvementKindId = NomenclatureConstants.SideInvolmentTypes.Vnositel,
                Subject = new Subject()
                {
                    Gid = Guid.NewGuid(),
                    Uin = userContext.Identifier,
                    Name = userContext.FullName,
                    CreateDate = dtNow,
                    ModifyDate = dtNow,
                    Person = new Person()
                    {
                        Gid = Guid.NewGuid(),
                        EGN = userContext.Identifier,
                        Firstname = userContext.FullName.GetSplitedName(1),
                        Secondname = userContext.FullName.GetSplitedName(2),
                        Lastname = userContext.FullName.GetSplitedName(3, "."),
                        CreateDate = dtNow,
                        ModifyDate = dtNow,
                    }
                }
            };

            if (NomenclatureConstants.UserTypes.OrganizationTypes.Contains(userContext.UserType))
            {
                var userOrg = await repo.GetByIdAsync<UserRegistration>(userContext.OrganizationUserId);
                var userOrganization = new ElectronicDocumentSide()
                {
                    Gid = Guid.NewGuid(),
                    SideInvolvementKindId = NomenclatureConstants.SideInvolmentTypes.Vazlojitel,
                    Subject = new Subject()
                    {
                        Gid = Guid.NewGuid(),
                        Uin = userOrg.UIC,
                        Name = userOrg.FullName,
                        CreateDate = dtNow,
                        ModifyDate = dtNow,
                        Entity = new Entity()
                        {
                            Gid = Guid.NewGuid(),
                            Bulstat = userOrg.UIC,
                            Name = userOrg.FullName,
                            CreateDate = dtNow,
                            ModifyDate = dtNow,
                        }
                    }
                };
                result.Add(userOrganization);
            }

            result.Add(userSide);
            return result;
        }

        public async Task<ElectronicDocumentVM> GetById(Guid gid)
        {
            if (gid == Guid.Empty)
            {
                return null;
            }

            var result = await repo.AllReadonly<ElectronicDocument>()
                                .AsSplitQuery()
                                .Where(x => x.Gid == gid)
                                .Select(x => new ElectronicDocumentVM
                                {
                                    Id = x.Id,
                                    Gid = x.Gid,
                                    CourtId = x.CourtId,
                                    CourtName = (x.CourtId > 0) ? x.Court.Name : "",
                                    CaseGid = x.Case.Gid,
                                    CaseInfo = (x.CaseId > 0) ? $"{x.Case.CaseKind.Name} {x.Case.Number}/{x.Case.CaseYear}" : "",
                                    SideGid = x.Side.Gid,
                                    SideInfo = (x.SideId > 0) ? $"{x.Side.Subject.Name} ({x.Side.SideInvolvementKind.Name})" : "",
                                    DocumentKind = x.DocumentKind,
                                    ElectronicDocumentTypeId = x.ElectronicDocumentTypeId,
                                    BaseAmount = x.BaseAmount,
                                    MoneyPricelistId = x.MoneyPricelistId,
                                    Description = x.Description,
                                    DocumentApplyBlobKey = x.BlobKeyDocumentApply
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return result;
        }

        public async Task<DocumentListVM> ReadForPrint(Guid gid)
        {
            if (gid == Guid.Empty)
            {
                return null;
            }
            if (!await CanAccess(gid))
            {
                return null;
            }


            var result = await repo.AllReadonly<ElectronicDocument>()
                                .Where(x => x.Gid == gid)
                                .Where(x => x.CourtId > 0)
                                .Where(x => x.ElectronicDocumentTypeId > 0)
                                .Select(x => new DocumentListVM
                                {
                                    Id = x.Id,
                                    Gid = x.Gid,
                                    CourtName = x.Court.Name,
                                    DocumentKind = x.ElectronicDocumentType.DocumentKind,
                                    DocumentTypeName = x.ElectronicDocumentType.Name,
                                    Description = x.Description,
                                    DateApply = x.DateApply,
                                    NumberApply = x.NumberApply,
                                    PricelistName = (x.MoneyPricelistId > 0) ? x.MoneyPricelist.Name : null,
                                    DatePaid = x.DatePaid,
                                    DocumentApplyBlobKey = x.BlobKeyDocumentApply,
                                    TimeApplyBlobKey = x.BlobKeyTimeApply,
                                    BaseAmount = x.BaseAmount,
                                    TaxAmount = x.TaxAmount,
                                    Currency = x.Currency.Code,
                                    Sides = x.Sides.Select(s => new ElectronicDocumentSideListVM
                                    {
                                        FullName = s.Subject.Name,
                                        UIC = s.Subject.Uin,
                                        SideInvolvementKindName = s.SideInvolvementKind.Name
                                    }).ToArray(),
                                    IncomingDocumentId = x.IncomingDocuments.Select(d => d.IncomingDocumentId).FirstOrDefault(),
                                    //DocInfo = x.IncomingDocuments.Select(d => new DocumentItemInfoVM
                                    //{
                                    //    Number = d.IncomingNumber,
                                    //    Gid = d.Gid,
                                    //    Date = d.IncomingDate,
                                    //    TypeName = d.IncomingDocumentType.Name
                                    //}).FirstOrDefault(),
                                    CaseId = x.CaseId,
                                    //CaseInfo = (x.CaseId > 0) ? new DocumentItemInfoVM
                                    //{
                                    //    Gid = x.Case.Gid,
                                    //    TypeName = x.Case.CaseKind.Name,
                                    //    Number = x.Case.Number,
                                    //    Year = x.Case.CaseYear
                                    //} : x.IncomingDocuments.SelectMany(d => d.Cases).Select(c => new DocumentItemInfoVM
                                    //{
                                    //    Gid = c.Gid,
                                    //    TypeName = c.CaseKind.Name,
                                    //    Number = c.Number,
                                    //    Year = c.CaseYear
                                    //}).FirstOrDefault()
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            if (result.IncomingDocumentId > 0)
            {
                result.DocInfo = await repo.AllReadonly<IncomingDocument>()
                                             .Where(x => x.IncomingDocumentId == result.IncomingDocumentId.Value)
                                             .Select(d => new DocumentItemInfoVM
                                             {
                                                 Number = d.IncomingNumber,
                                                 Gid = d.Gid,
                                                 Date = d.IncomingDate,
                                                 TypeName = d.IncomingDocumentType.Name
                                             }).FirstOrDefaultAsync();

                if ((result.CaseId ?? 0) == 0)
                {
                    result.CaseInfo = await repo.AllReadonly<Case>()
                                            .Where(x => x.IncomingDocumentId == result.IncomingDocumentId.Value)
                                            .Select(x => new DocumentItemInfoVM
                                            {
                                                Gid = x.Gid,
                                                TypeName = x.CaseKind.Name,
                                                Number = x.Number,
                                                Year = x.CaseYear
                                            }).FirstOrDefaultAsync();
                }
            }

            if (result.CaseId > 0)
            {
                result.CaseInfo = await repo.AllReadonly<Case>()
                                            .Where(x => x.CaseId == result.CaseId.Value)
                                            .Select(x => new DocumentItemInfoVM
                                            {
                                                Gid = x.Gid,
                                                TypeName = x.CaseKind.Name,
                                                Number = x.Number,
                                                Year = x.CaseYear
                                            }).FirstOrDefaultAsync();

            }

            result.Files = (await GetFileList(gid)).ToArray();
            result.Obligations = await paymentService.ObligationSelect(NomenclatureConstants.AttachedTypes.ElectronicDocument, result.Id).ToArrayAsync();

            return result;
        }

        public async Task<SaveResultVM> UploadFile(Guid documentGid, IFormFile file)
        {
            var docModel = await GetByGidAsync<ElectronicDocument>(documentGid);
            if (docModel == null)
            {
                return new SaveResultVM(false, SaveResultVM.MessageNotFound);
            }
            if (!CanAccess(docModel))
            {
                return new SaveResultVM(false, SaveResultVM.MessageAccessDenied);
            }

            BlobInfo fileResult;
            var fileName = Path.GetFileName(file.FileName);
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileResult = await blobService.AppendUpdateAttachedDocumentFile(NomenclatureConstants.AttachedTypes.ElectronicDocument, docModel.Id, ms.ToArray(), fileName, false);
            }
            if (fileResult != null)
            {
                return new SaveResultVM(true);
            }
            else
            {
                return new SaveResultVM(false);
            }
        }

        public async Task<SaveResultVM> SaveDocumentApply(Guid documentGid, byte[] pdfBytes)
        {
            var docModel = await GetByGidAsync<ElectronicDocument>(documentGid);
            if (docModel == null)
            {
                return new SaveResultVM(false, SaveResultVM.MessageNotFound);
            }
            if (!CanAccess(docModel))
            {
                return new SaveResultVM(false, SaveResultVM.MessageAccessDenied);
            }

            var fileName = "documentApply.pdf";
            var blobKey = await blobService.UploadFileToBlobStorage(docModel.BlobKeyDocumentApply ?? Guid.NewGuid(), pdfBytes, blobService.GetMimeType(fileName), BlobServiceBase.FileType.ElectronicDocument, DateTime.Now, docModel.BlobKeyDocumentApply.HasValue, fileName);
            if (blobKey != Guid.Empty && !docModel.BlobKeyDocumentApply.HasValue)
            {
                docModel.BlobKeyDocumentApply = blobKey;
                await repo.SaveChangesAsync();
                return new SaveResultVM(true);
            }

            return new SaveResultVM(false);
        }

        public async Task<SaveResultVM> SaveDocumentApplyTime(Guid documentGid)
        {
            var docModel = await GetByGidAsync<ElectronicDocument>(documentGid);
            if (docModel == null)
            {
                return new SaveResultVM(false, SaveResultVM.MessageNotFound);
            }
            if (!CanAccess(docModel))
            {
                return new SaveResultVM(false, SaveResultVM.MessageAccessDenied);
            }

            if (docModel.DateApply != null)
            {
                return new SaveResultVM(true);
            }

            try
            {
                var pdfBytes = await blobService.GetFileContent(docModel.BlobKeyDocumentApply.Value);

                byte[] tsr = null;
                DateTime dateApply = DateTime.Now;
                try
                {
                    (tsr, dateApply) = await timestampClient.GetTimestampAsync(pdfBytes);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $" timestampClient.GetTimestampAsync, Gid={documentGid}");
                }

                var localTime = dateApply.ToLocalTime();
                var applyNumber = localTime.ToString("yyyyMMddHHmmss");
                var tsFileName = $"{applyNumber}.cer";

                if (tsr != null)
                {
                    var tsGid = await blobService.UploadFileToBlobStorage(Guid.NewGuid(), tsr, "application/x-x509-ca-cert", BlobServiceBase.FileType.ElectronicDocumentStamp, localTime, false, tsFileName);
                    if (tsGid != Guid.Empty)
                    {
                        docModel.DateApply = localTime;
                        docModel.NumberApply = applyNumber;
                        if ((docModel.TaxAmount ?? 0M) < 0.0001M)
                        {
                            docModel.DatePaid = docModel.DateApply;
                        }
                        docModel.BlobKeyTimeApply = tsGid;
                        await repo.SaveChangesAsync();
                        await initMoneyObligationForDocument(docModel);
                        return new SaveResultVM(true);
                    }
                    else
                    {
                        return new SaveResultVM(false);
                    }
                }
                else
                {
                    docModel.DateApply = localTime;
                    docModel.NumberApply = applyNumber;
                    if ((docModel.TaxAmount ?? 0M) < 0.0001M)
                    {
                        docModel.DatePaid = docModel.DateApply;
                    }
                    //docModel.BlobKeyTimeApply = tsGid;
                    await repo.SaveChangesAsync();
                    await initMoneyObligationForDocument(docModel);
                    return new SaveResultVM(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"SaveDocumentApplyTime, Gid={documentGid}");
                return new SaveResultVM(false);
            }
        }

        public async Task<SaveResultVM> TestTimeStampt()
        {
            var pdfBytes = await blobService.GetFileContent(Guid.Parse("C2DB4783-D077-40BB-84CC-AA7CDD0F186D"));

            byte[] tsr = null;
            DateTime dateApply = DateTime.MinValue;
            try
            {
                (tsr, dateApply) = await timestampClient.GetTimestampAsync(pdfBytes);
                return new SaveResultVM(true, $"Timestamp: {dateApply}");
            }
            catch (Exception ex)
            {
                return new SaveResultVM(false, $"{ex.Message}; {ex.InnerException?.Message}");
            }
        }


        async Task<SaveResultVM> initMoneyObligationForDocument(ElectronicDocument model)
        {
            if (model == null || model.TaxAmount == null || model.TaxAmount == 0M)
            {
                return new SaveResultVM(false);
            }

            if (await repo.AllReadonly<MoneyObligation>()
                            .Where(x => x.AttachmentType == NomenclatureConstants.AttachedTypes.ElectronicDocument
                                    && x.ParentId == model.Id
                                    && x.MoneyObligationTypeId == NomenclatureConstants.MoneyObligationTypes.CourtTax)
                            .AnyAsync())
            {
                return new SaveResultVM(true);
            }

            var docInfo = await repo.AllReadonly<ElectronicDocument>()
                                    .Where(x => x.Id == model.Id)
                                    .Select(x => new
                                    {
                                        courtCode = x.Court.Code,
                                        courtName = x.Court.Name,
                                        docType = x.ElectronicDocumentType.Name,
                                    }).FirstOrDefaultAsync();

            var obligation = new MoneyObligation()
            {
                Gid = Guid.NewGuid(),
                AttachmentType = NomenclatureConstants.AttachedTypes.ElectronicDocument,
                ParentId = model.Id,
                UserRegistrationId = userContext.UserId,
                ClientCode = docInfo.courtCode,
                ParentDescription = docInfo.courtName,
                Description = $"{docInfo.docType} {model.NumberApply}",
                MoneyObligationTypeId = NomenclatureConstants.MoneyObligationTypes.CourtTax,
                MoneyCurrencyId = NomenclatureConstants.Currencies.BGN,
                MoneyAmount = model.TaxAmount.Value,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
            };
            if (userContext.OrganizationUserId > 0)
            {
                obligation.UserRegistrationId = userContext.OrganizationUserId;
            }

            await repo.AddAsync(obligation);
            await repo.SaveChangesAsync();
            return new SaveResultVM(true);
        }

        public async Task<SaveResultVM> RemoveFile(Guid gid)
        {
            var attModel = await GetByGidAsync<AttachedDocument>(gid);
            if (attModel == null)
                return new SaveResultVM(false, SaveResultVM.MessageNotFound);

            var docModel = await GetByIdAsync<ElectronicDocument>(attModel.ParentId);
            if (docModel == null)
                return new SaveResultVM(false);

            if (!CanAccess(docModel))
            {
                return new SaveResultVM(false, SaveResultVM.MessageAccessDenied);
            }


            if (docModel.UserId != userContext.UserId && docModel.UserId != userContext.OrganizationUserId)
            {
                return new SaveResultVM(false, "Нямате достъп до документа!");
            }
            if (docModel.DateApply.HasValue)
            {
                return new SaveResultVM(false, "Документът е потвърден!");
            }


            if (await blobService.DeleteFileFromStorage(attModel.BlobKey))
            {
                repo.Delete(attModel);
                try
                {
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true, "Премахването на файла премина успешно");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "SaveSideData.Add");
                }
            }
            return new SaveResultVM(false);
        }

        public async Task<SaveResultVM> CorrectDocument(Guid documentGid)
        {
            var docModel = await GetByGidAsync<ElectronicDocument>(documentGid);
            if (docModel == null)
            {
                return new SaveResultVM(false, SaveResultVM.MessageNotFound);
            }

            if (!CanAccess(docModel))
            {
                return new SaveResultVM(false, SaveResultVM.MessageAccessDenied);
            }

            if (!docModel.BlobKeyDocumentApply.HasValue)
            {
                return new SaveResultVM(false, SaveResultVM.MessageNotFound);
            }

            if (await blobService.DeleteFileFromStorage(docModel.BlobKeyDocumentApply.Value))
            {
                docModel.BlobKeyDocumentApply = null;
                await repo.SaveChangesAsync();
                return new SaveResultVM(true);
            }
            else
            {
                return new SaveResultVM(false);
            }
        }


        public async Task<List<FileListVM>> GetFileList(Guid documentGid)
        {
            var docModel = await GetByGidAsync<ElectronicDocument>(documentGid);
            if (docModel == null)
            {
                return null;
            }

            return await repo.AllReadonly<AttachedDocument>()
                                .Where(x => x.AttachmentType == NomenclatureConstants.AttachedTypes.ElectronicDocument)
                                .Where(x => x.ParentId == docModel.Id)
                                .Select(x => new FileListVM
                                {
                                    Gid = x.Gid,
                                    FileName = x.AttachedBlob.FileName,
                                    Hash = x.AttachedBlob.BlobContentLocation.Hash,
                                    BlobKey = x.BlobKey
                                }).ToListAsync();
        }



        public async Task<IQueryable<ElectronicDocumentSideVM>> SelectSides(Guid documentGid, Guid? sideGid)
        {

            Expression<Func<ElectronicDocumentSide, bool>> where = x => false;
            if (sideGid.HasValue)
            {
                where = x => x.Gid == sideGid.Value;
            }
            else
            {
                var docModel = await GetByGidAsync<ElectronicDocument>(documentGid);
                if (docModel == null)
                {
                    return null;
                }
                where = x => x.ElectronicDocumentId == docModel.Id;
            }
            int _stp = NomenclatureConstants.SubjectTypes.Person;
            int _ste = NomenclatureConstants.SubjectTypes.Entity;
            return repo.AllReadonly<ElectronicDocumentSide>()
                        .Where(where)
                        .OrderBy(x => x.SideInvolvementKind.ViewOrder)
                        .Select(x => new ElectronicDocumentSideVM
                        {
                            DocumentGid = x.ElectronicDocument.Gid,
                            SideGid = x.Gid,
                            SideInvolvementKindId = x.SideInvolvementKindId,
                            SideInvolvementKindName = x.SideInvolvementKind.Name,
                            SubjectTypeId = x.Subject.SubjectTypeId ?? 0,
                            Uic = (x.Subject.SubjectTypeId == _stp) ? x.Subject.Person.EGN : x.Subject.Entity.Bulstat,
                            Firstname = (x.Subject.SubjectTypeId == _stp) ? x.Subject.Person.Firstname : "",
                            Secondname = (x.Subject.SubjectTypeId == _stp) ? x.Subject.Person.Secondname : "",
                            Lastname = (x.Subject.SubjectTypeId == _stp) ? x.Subject.Person.Lastname : "",
                            EntityName = (x.Subject.SubjectTypeId == _ste) ? x.Subject.Entity.Name : "",
                            FullName = x.Subject.Name
                        });
        }

        public async Task<SaveResultVM> SaveDocumentData(ElectronicDocumentVM model)
        {
            var docModel = await GetByGidAsync<ElectronicDocument>(model.Gid);
            if (docModel == null)
                return new SaveResultVM(false);

            model.Sanitize();

            if (docModel.DocumentKind == NomenclatureConstants.DocumentKinds.Initial)
            {
                //Съда се променя само при иницииращи документи
                docModel.CourtId = model.CourtId;
            }
            docModel.ElectronicDocumentTypeId = model.ElectronicDocumentTypeId;
            docModel.MoneyPricelistId = model.MoneyPricelistId;
            docModel.BaseAmount = model.BaseAmount;
            docModel.Description = model.Description;
            docModel.ModifyDate = DateTime.Now;

            if (model.MoneyPricelistId > 0)
            {
                var calcPrice = await pricelistService.GetPrice(model.MoneyPricelistId.Value, model.BaseAmount ?? 0M);
                if (calcPrice > 0M)
                {
                    docModel.TaxAmount = calcPrice;
                }
            }

            try
            {
                await repo.SaveChangesAsync();
                return new SaveResultVM(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SaveDocumentData");
                return new SaveResultVM(false);
            }
        }

        public async Task<SaveResultVM> SaveSideData(ElectronicDocumentSideVM model)
        {
            //TODO Validate

            var docModel = await GetByGidAsync<ElectronicDocument>(model.DocumentGid);
            if (docModel == null)
                return new SaveResultVM(false);

            model.Sanitize();

            if (model.SideGid == Guid.Empty)
            {
                //Добавяне на страна
                var newSide = new ElectronicDocumentSide()
                {
                    Gid = Guid.NewGuid(),
                    SideInvolvementKindId = model.SideInvolvementKindId,
                    ElectronicDocumentId = docModel.Id
                    //SubjectId=1
                };

                var newSubject = new Subject()
                {
                    Gid = newSide.Gid,
                    SubjectTypeId = (int)model.SubjectTypeId,
                    Name = model.FullName,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                };

                switch (model.SubjectTypeId)
                {
                    case NomenclatureConstants.SubjectTypes.Person:
                        newSubject.Person = new Person()
                        {
                            Gid = newSide.Gid,
                            EGN = model.Uic,
                            Firstname = model.Firstname,
                            Secondname = model.Secondname,
                            Lastname = model.Lastname,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };
                        break;
                    case NomenclatureConstants.SubjectTypes.Entity:
                        newSubject.Entity = new Entity()
                        {
                            Gid = newSide.Gid,
                            Bulstat = model.Uic,
                            Name = model.EntityName,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now
                        };
                        break;
                }

                newSide.Subject = newSubject;

                try
                {
                    await repo.AddAsync(newSide);
                    await repo.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "SaveSideData.Add");
                    return new SaveResultVM(false);
                }
            }
            else
            {
                var sideModel = await GetByGidAsync<ElectronicDocumentSide>(model.SideGid);
                if (sideModel == null)
                {
                    return new SaveResultVM(false);
                }
                var subjectModel = await GetByIdAsync<Subject>(sideModel.SubjectId);
                switch (subjectModel.SubjectTypeId)
                {
                    case NomenclatureConstants.SubjectTypes.Person:
                        {
                            var _person = await GetByIdAsync<Person>(subjectModel.SubjectId);
                            _person.Firstname = model.Firstname;
                            _person.Secondname = model.Secondname;
                            _person.Lastname = model.Lastname;
                            _person.EGN = model.Uic;
                            _person.ModifyDate = DateTime.Now;
                            break;
                        }
                    case NomenclatureConstants.SubjectTypes.Entity:
                        {
                            var _entity = await GetByIdAsync<Entity>(subjectModel.SubjectId);
                            _entity.Name = model.EntityName;
                            _entity.Bulstat = model.Uic;
                            _entity.ModifyDate = DateTime.Now;
                            break;
                        }
                }
                subjectModel.Name = model.FullName;
                sideModel.SideInvolvementKindId = model.SideInvolvementKindId;
                try
                {
                    await repo.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "SaveSideData.Edit");
                    return new SaveResultVM(false);
                }
            }
            return new SaveResultVM(true);
        }

        public async Task<SaveResultVM> RemoveSide(Guid gid)
        {
            var sideModel = await GetByGidAsync<ElectronicDocumentSide>(gid);
            if (sideModel == null)
                return new SaveResultVM(false);

            var docModel = await GetByIdAsync<ElectronicDocument>(sideModel.ElectronicDocumentId);
            if (docModel == null)
                return new SaveResultVM(false);

            if (docModel.UserId != userContext.UserId && docModel.UserId != userContext.OrganizationUserId)
            {
                return new SaveResultVM(false, "Нямате достъп до документа!");
            }
            if (docModel.DateApply.HasValue)
            {
                return new SaveResultVM(false, "Документът е потвърден!");
            }

            var subjectModel = await GetByIdAsync<Subject>(sideModel.SubjectId);
            switch (subjectModel.SubjectTypeId)
            {
                case NomenclatureConstants.SubjectTypes.Person:
                    {
                        var _person = await GetByIdAsync<Person>(subjectModel.SubjectId);
                        repo.Delete(_person);
                        break;
                    }
                case NomenclatureConstants.SubjectTypes.Entity:
                    {
                        var _entity = await GetByIdAsync<Entity>(subjectModel.SubjectId);
                        repo.Delete(_entity);
                        break;
                    }
            }
            repo.Delete(subjectModel);
            repo.Delete(sideModel);

            try
            {
                await repo.SaveChangesAsync();
                return new SaveResultVM(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RemoveSide");
                return new SaveResultVM(false);
            }
        }
    }
}
