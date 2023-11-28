using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eCase.Web.Api
{
    public class BlobInfo
    {
        public BlobInfo(long blobContentLocationId, long size, string hash)
        {
            this.BlobContentLocationId = blobContentLocationId;
            this.Size = size;
            this.Hash = hash;
        }

        public long BlobContentLocationId { get; private set; }

        public long Size { get; private set; }

        public string Hash { get; private set; }
    }
}