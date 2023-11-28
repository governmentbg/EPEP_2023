using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using eCase.Data.Core;
using eCase.Domain.Entities;
using eCase.Domain.Service;

namespace eCase.Data.Repositories
{
    public interface IAttachedDocumentRepository : IAggregateRepository<AttachedDocument>
    {
        Domain.Service.Entities.AttachedDocument GetAttachedDocumentById(Guid attachedDocumentId);
        IQueryable<AttachedDocument> GetAttachedDocuments(int type, long parentId);
        IQueryable<AttachedDocument> GetAttachedDocuments(int type, long[] parentIds);
        long GetParentIdByGid(int type, Guid parentGid);
    }

    internal class AttachedDocumentRepository : AggregateRepository<AttachedDocument>, IAttachedDocumentRepository
    {
        private readonly IIncomingDocumentRepository _inDocRepository;
        private readonly IOutgoingDocumentRepository _outDocRepository;
        private readonly IActRepository _actRepository;
        private readonly IHearingDocumentRepository _hearingDocumentRepository;
        private readonly IBlobStorageRepository _blobRepository;

        public AttachedDocumentRepository(IUnitOfWork unitOfWork,
            IIncomingDocumentRepository inDocRepository,
            IOutgoingDocumentRepository outDocRepository,
            IActRepository actRepository,
            IHearingDocumentRepository hearingDocumentRepository,
            IBlobStorageRepository blobRepository)
            : base(unitOfWork)
        {
            _inDocRepository = inDocRepository;
            _outDocRepository = outDocRepository;
            _actRepository = actRepository;
            _hearingDocumentRepository = hearingDocumentRepository;
            _blobRepository = blobRepository;
        }

        public IQueryable<AttachedDocument> GetAttachedDocuments(int type, long parentId)
        {
            return this.Set().Where(t => t.AttachmentType == type && t.ParentId == parentId);
        }

        public IQueryable<AttachedDocument> GetAttachedDocuments(int type, long[] parentIds)
        {
            return this.Set().Where(t => t.AttachmentType == type && parentIds.Contains(t.ParentId));
        }

        public long GetParentIdByGid(int type, Guid parentGid)
        {
            long? result = null;
            switch (type)
            {
                case AttachedTypes.IncommingDocument:
                    result = _inDocRepository.FindByGid(parentGid)?.IncomingDocumentId;
                    break;
                case AttachedTypes.OutgoingDocument:
                    result = _outDocRepository.FindByGid(parentGid)?.OutgoingDocumentId;
                    break;
                case AttachedTypes.ActCoordination:
                case AttachedTypes.ActCoordinationPublic:
                case AttachedTypes.ActFile:
                    result = _actRepository.FindByGid(parentGid)?.ActId;
                    break;
                case AttachedTypes.SessionFastDocument:
                    result = _hearingDocumentRepository.FindByGid(parentGid)?.HearingDocumentId;
                    break;
            }
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                return -1;
            }
        }

        public Guid? GetParentGidById(int type, long parentId)
        {
            Guid? result = null;
            switch (type)
            {
                case AttachedTypes.IncommingDocument:
                    result = _inDocRepository.Find(parentId)?.Gid;
                    break;
                case AttachedTypes.OutgoingDocument:
                    result = _outDocRepository.Find(parentId)?.Gid;
                    break;
                case AttachedTypes.ActCoordination:
                case AttachedTypes.ActCoordinationPublic:
                case AttachedTypes.ActFile:
                    result = _actRepository.Find(parentId)?.Gid;
                    break;
                case AttachedTypes.SessionFastDocument:
                    result = _hearingDocumentRepository.Find(parentId)?.Gid;
                    break;
            }
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                return null;
            }
        }

        public Domain.Service.Entities.AttachedDocument GetAttachedDocumentById(Guid attachedDocumentId)
        {
            var result = this.Set().Where(x => x.Gid == attachedDocumentId).Select(x => new
            {
                x.AttachmentType,
                x.ParentId,
                x.FileTitle,
                x.FileName,
                x.BlobKey,
                x.AttachedBlob.BlobContentLocation.ContentDbConnectionStringName,
                x.AttachedBlob.BlobContentLocation.BlobContentId
            }).FirstOrDefault();

            if(result == null)
            {
                throw new FaultException<InfocaseFault>(new InfocaseFault(Domain.Service.FaultCode.NotExists, nameof(attachedDocumentId)));
            }

            var responseStream = new MemoryStream();
            _blobRepository.CopyBlobContentToResponseStream(responseStream, result.BlobContentId,result.ContentDbConnectionStringName);
            responseStream.Position = 0;
            return new Domain.Service.Entities.AttachedDocument()
            {
                AttachedDocumentId = attachedDocumentId,
                FileName = result.FileName,
                FileTitle = result.FileTitle,
                FileContent = responseStream.ToArray(),
                ParentId = GetParentGidById(result.AttachmentType, result.ParentId) ?? Guid.Empty,
                Type = result.AttachmentType
            };
        }


    }

    public static class AttachedTypes
    {
        public const int IncommingDocument = 1;
        public const int OutgoingDocument = 2;
        public const int ActCoordination = 3;
        public const int ActCoordinationPublic = 4;
        public const int ActFile = 41;
        public const int SessionFastDocument = 5;
        public const int ElectronicDocument = 7;
        public const int ElectronicDocumentMain = 71;
        public const int ElectronicDocumentTimestamp = 72;
    }
}