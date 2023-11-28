using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace eCase.Web.Api
{
    public class LimitedStream : Stream
    {
        private Stream underlyingStream;
        private long maxWrittenDataSize;
        private long currentWrittenDataSize;

        public LimitedStream(Stream us, long mwds)
        {
            this.underlyingStream = us;
            this.maxWrittenDataSize = mwds;
            this.currentWrittenDataSize = 0;
        }

        public override bool CanRead
        {
            get { return underlyingStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return underlyingStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return underlyingStream.CanWrite; }
        }

        public override long Length
        {
            get { return underlyingStream.Length; }
        }

        public override long Position
        {
            get
            {
                return underlyingStream.Position;
            }

            set
            {
                underlyingStream.Position = value;
            }
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            this.currentWrittenDataSize += count;
            if (this.currentWrittenDataSize > this.maxWrittenDataSize)
            {
                throw new Exception("Maximum written data size reached!");
            }

            await this.underlyingStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.currentWrittenDataSize += count;
            if (this.currentWrittenDataSize > this.maxWrittenDataSize)
            {
                throw new Exception("Maximum written data size reached!");
            }

            this.underlyingStream.Write(buffer, offset, count);
        }
        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await this.underlyingStream.FlushAsync(cancellationToken);
        }

        public override void Flush()
        {
            this.underlyingStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.underlyingStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.underlyingStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.underlyingStream.SetLength(value);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && this.underlyingStream != null)
                {
                    this.underlyingStream.Dispose();
                }
            }
            finally
            {
                this.underlyingStream = null;
                base.Dispose(disposing);
            }
        }
    }
}