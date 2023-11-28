using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using System.Web;
using System.IO;
using System.Data;
using System.Configuration;
using eCase.Web.Helpers;
using eCase.Common.Helpers;

namespace eCase.Web.Controllers
{
    public class FileController : ApiController
    {
        [HttpGet]
        [SecretChecker]
        [Route("api/file/download/{fileKey:guid}")]
        public async Task<HttpResponseMessage> Download(Guid fileKey, CancellationToken cancellationToken)
        {
            try
            {
                
                BlobInfo blobInfo = await this.GetBlobInfo(fileKey, cancellationToken);
                if (blobInfo == null)
                {
                    throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound));
                }

                RangeInfo rangeInfo = GetRangeInfo(this.Request, blobInfo.FileLength);

                var response = this.Request.CreateResponse();
                response.Content = new PushStreamContent(
                    async (responseStream, httpContent, transportContext) =>
                    {
                        using (responseStream)
                        {
                            try
                            {
                                await this.CopyBlobContentToResponseStream(responseStream, rangeInfo, blobInfo, cancellationToken);
                            }
                            catch (HttpException ex)
                            {
                                //throw an OperationCanceledException if we have been canceled
                                cancellationToken.ThrowIfCancellationRequested();

                                //swallow communication exceptions, we can't do anything about them
                                if (!ex.Message.StartsWith("The remote host closed the connection.") &&
                                    !ex.Message.StartsWith("An error occurred while communicating with the remote host."))
                                {
                                    throw;
                                }
                            }
                        }
                    });

                this.SetResponseHeaders(response, rangeInfo, blobInfo);

                return response;
            }
            catch
            {
                //throw an OperationCanceledException if we have been canceled
                cancellationToken.ThrowIfCancellationRequested();

                //otherwise rethrow the original exception
                throw;
            }
        }

        #region Private

        private async Task CopyBlobContentToResponseStream(Stream responseStream, RangeInfo rangeInfo, BlobInfo blobInfo, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[blobInfo.ContentDbConnectionStringName].ConnectionString))
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

        private class RangeInfo
        {
            public long From { get; set; }
            public long To { get; set; }
            public long Length { get; set; }
        }

        private RangeInfo GetRangeInfo(HttpRequestMessage request, long contentLength)
        {
            RangeHeaderValue rangeHeader = request.Headers.Range;
            if (rangeHeader != null && rangeHeader.Ranges.Count > 0)
            {
                //we support only one range
                if (rangeHeader.Ranges.Count > 1)
                {
                    throw new HttpResponseException(HttpStatusCode.RequestedRangeNotSatisfiable);
                }

                RangeItemHeaderValue range = rangeHeader.Ranges.First();

                //check if range is satisfiable
                //http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html#sec14.35.1
                if (range.From < 0 ||
                    range.From > range.To ||
                    range.From == null && range.To == null)
                {
                    throw new HttpResponseException(HttpStatusCode.RequestedRangeNotSatisfiable);
                }

                long from;
                long to;
                long length;

                if (range.From == null)
                {
                    from = contentLength - range.To.Value;
                    to = contentLength - 1;
                }
                else if (range.To == null)
                {
                    from = range.From.Value;
                    to = contentLength - 1;
                }
                else
                {
                    from = range.From.Value;
                    to = Math.Min(range.To.Value, contentLength - 1);
                }

                length = to - from + 1;

                return new RangeInfo
                {
                    From = from,
                    To = to,
                    Length = length
                };
            }

            return null;
        }

        private void SetResponseHeaders(HttpResponseMessage response, RangeInfo rangeInfo, BlobInfo blobInfo)
        {
            if (rangeInfo != null)
            {
                response.StatusCode = HttpStatusCode.PartialContent;
                response.Content.Headers.ContentLength = rangeInfo.Length;
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(rangeInfo.From, rangeInfo.To, blobInfo.FileLength);
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Content.Headers.ContentLength = blobInfo.FileLength;
            }

            response.Headers.AcceptRanges.Add("bytes");

            //do not use the class version of the ContentDisposition as it incorrectly implements UTF8 filenames
            response.Content.Headers.Add(
                "Content-Disposition",
                "inline; filename=\"" + blobInfo.FileName + "\"; filename*=UTF-8''" + Uri.EscapeDataString(blobInfo.FileName));

            string mimeType = MimeTypeHelper.GetFileMimeTypeByExtenstion(Path.GetExtension(blobInfo.FileName));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrEmpty(mimeType) ? "application/octet-stream" : mimeType);
        }

        private class BlobInfo
        {
            public long BlobContentId { get; set; }
            public string ContentDbConnectionStringName { get; set; }
            public string FileName { get; set; }
            public long FileLength { get; set; }
        }

        private async Task<BlobInfo> GetBlobInfo(Guid fileKey, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DbContextMain"].ConnectionString))
            {
                await connection.OpenAsync(cancellationToken);

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = @"
                        SELECT bcl.BlobContentId, bcl.ContentDbConnectionStringName, bcl.Size, b.FileName FROM Blobs b
                        INNER JOIN BlobContentLocations bcl ON b.BlobContentLocationId = bcl.BlobContentLocationId
                        WHERE b.[Key] = @blobKey";
                cmd.Parameters.AddWithValue("@blobKey", fileKey);

                SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);

                if (!reader.HasRows)
                {
                    return null;
                }

                await reader.ReadAsync(cancellationToken);

                return new BlobInfo
                {
                    BlobContentId = reader.GetInt64(reader.GetOrdinal("BlobContentId")),
                    ContentDbConnectionStringName = reader.GetString(reader.GetOrdinal("ContentDbConnectionStringName")),
                    FileName = reader.GetString(reader.GetOrdinal("FileName")),
                    FileLength = reader.GetInt64(reader.GetOrdinal("Size"))
                };
            }
        }

        #endregion //Private
    }
}
