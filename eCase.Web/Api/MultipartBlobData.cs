using System;
using System.Net.Http.Headers;

namespace eCase.Web.Api
{
    public class MultipartBlobData
    {
        public MultipartBlobData(HttpContentHeaders headers, BlobInfo blobInfo)
        {
            this.Headers = headers;
            this.BlobInfo = blobInfo;
        }

        public HttpContentHeaders Headers { get; private set; }

        public BlobInfo BlobInfo { get; private set; }
    }
}
