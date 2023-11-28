using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading;

namespace Epep.Core.Services
{
    public class BlobServiceSql : BlobServiceBase, IBlobService
    {
        private readonly string CurrentContentBaseConnectionString;
        private readonly string CurrentContentConnection;
        private readonly string MainDbConnectionString;

        public BlobServiceSql(
            IConfiguration _config,
            IRepository _repo
            )
        {
            config = _config;
            repo = _repo;
            MainDbConnectionString = config.GetConnectionString("ApplicationDbContext");
            CurrentContentConnection = _config.GetSection("CurrentContentConnection").Value;
            CurrentContentBaseConnectionString = config.GetConnectionString(CurrentContentConnection);
        }

        public async Task<byte[]> GetFileContent(Guid blobKey, CancellationToken cancellationToken = default)
        {
            var blobInfo = await GetBlobInfo(blobKey, cancellationToken);
            if (blobInfo == null)
            {
                return null;
            }
            using (var ms = new MemoryStream())
            {
                await CopyBlobContentToResponseStream(ms, null, blobInfo, cancellationToken);
                return ms.ToArray();
            }
            //byte[] fileBytes = new byte[ms.Length];
            //await ms.WriteAsync(fileBytes, 0, fileBytes.Length);
            //return fileBytes;
        }



        protected override async Task<BlobUploadResultVM> UploadFile(string hash, byte[] content, CancellationToken cancellationToken)
        {
            var result = new BlobUploadResultVM();
            var BlobContentSequence = new SqlSequenceSimple("BlobContentSequence", MainDbConnectionString);
            var blobContentId = await BlobContentSequence.NextValue();
            result.StorageName = CurrentContentConnection;
            result.BlobContentId = blobContentId;
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(CurrentContentBaseConnectionString))
            {
                await connection.OpenAsync(cancellationToken);

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO [dbo].[BlobContents]([BlobContentId],[Hash],[Size],[Content],[IsDeleted])VALUES(@blobContentId,@hash,@size,@content,@isDeleted)";
                    command.Parameters.AddWithValue("@blobContentId", blobContentId);
                    command.Parameters.AddWithValue("@hash", hash);
                    command.Parameters.AddWithValue("@size", content.Length);
                    command.Parameters.AddWithValue("@content", content);
                    command.Parameters.AddWithValue("@isDeleted", false);
                    rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            result.Result = rowsAffected > 0;
            return result;
        }

        public async Task CopyBlobContentToResponseStream(Stream responseStream, RangeInfo rangeInfo, BlobInfo blobInfo, CancellationToken cancellationToken)
        {

            using (SqlConnection connection = new SqlConnection(config.GetConnectionString(blobInfo.ContentDbConnectionStringName)))
            {
                await connection.OpenAsync(cancellationToken);

                using (SqlCommand command = connection.CreateCommand())
                {
                    if (rangeInfo != null)
                    {
                        command.CommandText = "SELECT SUBSTRING([Content], @offset , @length) FROM BlobContents WHERE BlobContentId = @blobContentId";
                        command.Parameters.AddWithValue("@offset", rangeInfo.From + 1);
                        command.Parameters.AddWithValue("@length", rangeInfo.Length);
                        command.Parameters.AddWithValue("@blobContentId", blobInfo.BlobContentId);
                    }
                    else
                    {
                        command.CommandText = "SELECT [Content] FROM BlobContents WHERE BlobContentId = @blobContentId";
                        command.Parameters.AddWithValue("@blobContentId", blobInfo.BlobContentId);
                    }

                    // The reader needs to be executed with the SequentialAccess behavior to enable network streaming
                    // Otherwise ReadAsync will buffer the entire BLOB into memory which can cause scalability issues or even OutOfMemoryExceptions
                    using (SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken))
                    {
                        if (await reader.ReadAsync(cancellationToken))
                        {
                            if (!(await reader.IsDBNullAsync(0, cancellationToken)))
                            {
                                using (Stream data = reader.GetStream(0))
                                {
                                    await data.CopyToAsync(responseStream, 4096/*default*/, cancellationToken);
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override async Task<bool> RemoveBlobContent(BlobInfo blobInfo, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString(blobInfo.ContentDbConnectionStringName)))
            {
                await connection.OpenAsync(cancellationToken);

                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "DELETE FROM BlobContents WHERE BlobContentId = @blobContentId";
                    command.Parameters.AddWithValue("@blobContentId", blobInfo.BlobContentId);
                    return (await command.ExecuteNonQueryAsync(cancellationToken)) > 0;
                }
            }
        }

        protected override async Task<bool> ExistsBlobContentId(BlobInfo blobInfo, CancellationToken cancellationToken)
        {
            long dbValue = 0;
            using (SqlConnection connection = new SqlConnection(config.GetConnectionString(blobInfo.ContentDbConnectionStringName)))
            {
                await connection.OpenAsync(cancellationToken);

                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = "SELECT BlobContentId FROM BlobContents WHERE BlobContentId = @blobContentId";
                    command.Parameters.AddWithValue("@blobContentId", blobInfo.BlobContentId);
                    var reader = await command.ExecuteReaderAsync(cancellationToken);
                    while (reader.Read())
                    {
                        dbValue = reader.GetInt64(0);
                    }
                }
            }
            return dbValue == blobInfo.BlobContentId;
        }
    }
}
