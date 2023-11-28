using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace eCase.Web.Api
{
    public class MultipartBlobStreamProvider : MultipartStreamProvider, IDisposable
    {
        private List<BlobWriter> blobWriters = new List<BlobWriter>();

        private SqlConnection blobDbConnection;
        private SqlConnection mainDbConnection;

        public MultipartBlobStreamProvider(SqlConnection blobDbConnection, SqlConnection mainDbConnection)
        {
            this.blobDbConnection = blobDbConnection;
            this.mainDbConnection = mainDbConnection;
        }

        public NameValueCollection FormData { get; private set; }

        public ICollection<MultipartBlobData> BlobData { get; private set; }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;

            if (contentDisposition == null)
            {
                // for form data, Content-Disposition header is a requirement
                throw new InvalidOperationException("Did not find required 'Content-Disposition' header field in MIME multipart body part.");
            }

            // if no filename parameter was found in the Content-Disposition header then return a memory stream.
            if (string.IsNullOrEmpty(contentDisposition.FileName))
            {
                this.blobWriters.Add(null);
                return new MemoryStream();
            }
            // if we have a file name then write contents to with the BlobWriter which will write it to the database
            else
            {
                var blobWriter = new BlobWriter(this.blobDbConnection, this.mainDbConnection);
                this.blobWriters.Add(blobWriter);
                var blobStream = blobWriter.OpenStream();
                return new LimitedStream(blobStream, long.Parse(ConfigurationManager.AppSettings["eCase.Blob.Host:MaxBlobSize"]));
            }
        }

        public override async Task ExecutePostProcessingAsync()
        {
            NameValueCollection formData = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            List<MultipartBlobData> blobData = new List<MultipartBlobData>();

            for (int index = 0; index < this.Contents.Count; index++)
            {
                BlobWriter blobWriter = this.blobWriters[index];
                HttpContent content = this.Contents[index];
                if (blobWriter == null)
                {
                    // Extract name from Content-Disposition header. We know from earlier that the header is present.
                    ContentDispositionHeaderValue contentDisposition = content.Headers.ContentDisposition;
                    string formFieldName = HttpHelpers.UnquoteToken(contentDisposition.Name) ?? string.Empty;

                    // Read the contents as string data and add to form data
                    string formFieldValue = await content.ReadAsStringAsync();
                    formData.Add(formFieldName, formFieldValue);
                }
                else
                {
                    BlobInfo blobInfo = await blobWriter.GetBlobInfoAsync();
                    blobWriter.Dispose();
                    blobData.Add(new MultipartBlobData(content.Headers, blobInfo));
                }
            }

            this.FormData = new NameValueCollection(formData);
            this.BlobData = blobData.AsReadOnly();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            try
            {
                if (disposing && this.blobWriters != null)
                {
                    foreach (var blobWriter in this.blobWriters)
                    {
                        if (blobWriter != null)
                        {
                            blobWriter.Dispose();
                        }
                    }
                }
            }
            finally
            {
                this.blobWriters = null;

                //we are not managing the connection so we are not disposing it
                this.blobDbConnection = null;
            }
        }
    }
}
