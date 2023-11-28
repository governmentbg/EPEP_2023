using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using Autofac.Features.Indexed;

using eCase.Common.Crypto;
using eCase.Common.Db;
using eCase.Common.Helpers;
using eCase.Data.Core;
using eCase.Domain.BlobStorage;
using eCase.Domain.Entities;

namespace eCase.Data.Repositories
{
    public interface IBlobStorageRepository
    {
        Guid UploadFileToBlobStorage(Guid blobKey, byte[] content, string mimeType, FileType fileType, DateTime fileDate, bool isUpdate = false, string originalFileName = null);

        byte[] GetFileContent(Guid blobKey);

        string GetMimeType(string fileName);

        bool HasKey(Guid blobKey);
        void CopyBlobContentToResponseStream(Stream responseStream, long blobContentId, string locationConnectionString);
    }

    internal class BlobStorageRepository : IBlobStorageRepository
    {
        private IIndex<DbKey, IUnitOfWork> _unitOfWorks;
        private static Sequence BlobContentSequence = new Sequence("BlobContentSequence", "DbContextMain");

        public BlobStorageRepository(IIndex<DbKey, IUnitOfWork> unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public Guid UploadFileToBlobStorage(Guid blobKey, byte[] content, string mimeType, FileType fileType, DateTime fileDate, bool isUpdate = false, string originalFileName = null)
        {
            using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
            {
                var hash = CryptoUtils.GetHexString(sha256.ComputeHash(content));
                var size = content.Length;
                long blobContentId = 0;

                BlobContent blobContent = null;

                //blobContent = ((UnitOfWork)_unitOfWorks[DbKey.BlobStorage]).DbContext.Set<BlobContent>().
                //     SingleOrDefault(e => e.Hash == hash);

                if (blobContent == null)
                {
                    blobContentId = BlobContentSequence.NextValue();

                    ((UnitOfWork)_unitOfWorks[DbKey.BlobStorage]).DbContext.Set<BlobContent>()
                    .Add(new BlobContent
                    {
                        BlobContentId = blobContentId,
                        Content = content,
                        Hash = hash,
                        Size = size,
                        IsDeleted = false
                    });

                    _unitOfWorks[DbKey.BlobStorage].Save();
                }
                else
                {
                    blobContentId = blobContent.BlobContentId;
                }

                var blobContentLocation = new BlobContentLocation()
                {
                    BlobContentId = blobContentId,
                    ContentDbConnectionStringName = ConfigurationManager.AppSettings["eCase.Service:CurrentBlobStorageConnectionString"],
                    Hash = hash,
                    Size = size
                };

                ((UnitOfWork)_unitOfWorks[DbKey.Main]).DbContext.Set<BlobContentLocation>().Add(blobContentLocation);

                var fileName = GenerateFileName(fileType, fileDate, mimeType);
                if (fileType == FileType.AttachedDocument && !string.IsNullOrEmpty(originalFileName))
                {
                    fileName = originalFileName;
                }
                if (isUpdate)
                {
                    var blob = ((UnitOfWork)_unitOfWorks[DbKey.Main]).DbContext.Set<Blob>()
                                .SingleOrDefault(e => e.Key == blobKey);

                    //blob.FileName = GenerateFileName(fileType, fileDate, mimeType);
                    blob.FileName = fileName;
                    blob.BlobContentLocationId = blobContentLocation.BlobContentLocationId;
                }
                else
                {
                    ((UnitOfWork)_unitOfWorks[DbKey.Main]).DbContext.Set<Blob>().Add(new Blob
                    {
                        Key = blobKey,
                        //FileName = GenerateFileName(fileType, fileDate, mimeType),
                        FileName = fileName,
                        BlobContentLocationId = blobContentLocation.BlobContentLocationId
                    });
                }



                _unitOfWorks[DbKey.Main].Save();
            }

            return blobKey;
        }

        public byte[] GetFileContent(Guid blobKey)
        {
            var blobContentLocationId = ((UnitOfWork)_unitOfWorks[DbKey.Main]).DbContext.Set<Blob>()
                .Where(m => m.Key == blobKey).Select(m => m.BlobContentLocationId)
                .SingleOrDefault();

            var blobContentId = ((UnitOfWork)_unitOfWorks[DbKey.Main]).DbContext.Set<BlobContentLocation>()
                .Where(m => m.BlobContentLocationId == blobContentLocationId).Select(m => m.BlobContentId)
                .SingleOrDefault();

            var content = ((UnitOfWork)_unitOfWorks[DbKey.BlobStorage]).DbContext.Set<BlobContent>()
                .Where(m => m.BlobContentId == blobContentId).Select(m => m.Content)
                .SingleOrDefault();

            return content;
        }

        private string GenerateFileName(FileType fileType, DateTime fileDate, string mimeType)
        {
            var suffix = string.Format("{0}_{1}_{2}.{3}", fileDate.Year,
                  fileDate.Month, fileDate.Day, MimeTypeHelper.GetFileExtenstionByMimeType(mimeType));

            switch (fileType)
            {
                case FileType.Incoming:
                    return string.Format("incoming_{0}", suffix);
                case FileType.Outgoing:
                    return string.Format("outgoing_{0}", suffix);
                case FileType.PrivateAct:
                    return string.Format("act_{0}", suffix);
                case FileType.PublicAct:
                    return string.Format("act_{0}", suffix);
                case FileType.PrivateMotive:
                    return string.Format("motive_{0}", suffix);
                case FileType.PublicMotive:
                    return string.Format("motive_{0}", suffix);
                case FileType.PrivateProtocol:
                    return string.Format("protocol_{0}", suffix);
                case FileType.PublicProtocol:
                    return string.Format("protocol_{0}", suffix);
                case FileType.Assignment:
                    return string.Format("assignment_{0}", suffix);
                case FileType.Summon:
                    return string.Format("summon_{0}", suffix);
                case FileType.SummonReport:
                    return string.Format("summon_report_{0}", suffix);
                case FileType.ScannedDocument:
                    return string.Format("scanned_document_{0}", suffix);
                default:
                    return string.Format("document_{0}", suffix);
            }
        }

        public string GetMimeType(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var mimeType = MimeTypeHelper.GetFileMimeTypeByExtenstion(fileExtension);

            return mimeType;
        }

        public bool HasKey(Guid blobKey)
        {
            var hasKey = false;
            var blob = ((UnitOfWork)_unitOfWorks[DbKey.Main]).DbContext.Set<Blob>()
                        .FirstOrDefault(b => b.Key == blobKey);

            if (blob != null)
            {
                hasKey = true;
            }

            return hasKey;
        }


        public void CopyBlobContentToResponseStream(Stream responseStream, long blobContentId, string locationConnectionString)
        {

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[locationConnectionString].ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT [Content] FROM BlobContents WHERE BlobContentId = @blobContentId";
                    command.Parameters.AddWithValue("@blobContentId", blobContentId);

                    // The reader needs to be executed with the SequentialAccess behavior to enable network streaming
                    // Otherwise ReadAsync will buffer the entire BLOB into memory which can cause scalability issues or even OutOfMemoryExceptions
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        if (reader.Read())
                        {
                            if (!(reader.IsDBNull(0)))
                            {
                                using (Stream data = reader.GetStream(0))
                                {
                                    data.CopyTo(responseStream, 4096/*default*/);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public enum FileType
    {
        Incoming,
        Outgoing,
        PrivateAct,
        PublicAct,
        PrivateMotive,
        PublicMotive,
        PrivateProtocol,
        PublicProtocol,
        Assignment,
        Summon,
        SummonReport,
        ScannedDocument,
        AttachedDocument
    }
}