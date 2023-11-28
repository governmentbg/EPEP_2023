using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using eCase.Common.Crypto;
using NLog;
using eCase.Common.Db;
using System.Configuration;

namespace eCase.Web.Api
{
    public class BlobWriter : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static object updatingSyncRoot = new Object();
        private static Sequence BlobContentSequence = new Sequence("BlobContentSequence", "DbContext");

        private SqlConnection blobDbConnection;
        private SqlConnection mainDbConnection;

        private long blobContentId;
        private Stream stream;
        private SHA256 sha256;

        public BlobWriter(SqlConnection blobDbConnection, SqlConnection mainDbConnection)
        {
            this.blobDbConnection = blobDbConnection;
            this.mainDbConnection = mainDbConnection;
        }

        public Stream OpenStream()
        {
            this.blobContentId = BlobWriter.BlobContentSequence.NextValue();
            using (SqlCommand cmdInsert = this.CreateInsertCmd())
            {
                try
                {
                    cmdInsert.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    Logger.Error(string.Format("Failed: \"INSERT INTO BlobContents (BlobContentId, [Hash], [Size], [Content], IsDeleted) VALUES ({0}, NULL, NULL, NULL, 1);\"", this.blobContentId));
                    throw;
                }

                return this.CreateStream();
            }
        }

        public async Task<Stream> OpenStreamAsync()
        {
            this.blobContentId = BlobWriter.BlobContentSequence.NextValue();
            using (SqlCommand cmdInsert = this.CreateInsertCmd())
            {
                try
                {
                    await cmdInsert.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                    Logger.Error(string.Format("Failed: \"INSERT INTO BlobContents (BlobContentId, [Hash], [Size], [Content], IsDeleted) VALUES ({0}, NULL, NULL, NULL, 1);\"", this.blobContentId));
                    throw;
                }

                return this.CreateStream();
            }
        }

        public Task<BlobInfo> GetBlobInfoAsync()
        {
            // make sure noone writes to the blob after we calculate its hash
            this.stream.Close();

            return this.GetBlobInfoInternalAsync();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            try
            {
                if (disposing && this.sha256 != null && this.stream != null)
                {
                    using (this.sha256)
                    using (this.stream)
                    {
                    }
                }
            }
            finally
            {
                this.sha256 = null;
                this.stream = null;

                //we are not managing the connection so we are not disposing it
                this.blobDbConnection = null;
            }
        }

        private async Task<BlobInfo> GetBlobInfoInternalAsync()
        {
            string hash = CryptoUtils.GetHexString(this.sha256.Hash);

            var getSizeCmd = this.blobDbConnection.CreateCommand();
            getSizeCmd.CommandText = "SELECT DATALENGTH([Content]) FROM BlobContents WHERE BlobContentId = @blobContentId";
            getSizeCmd.Parameters.AddWithValue("@blobContentId", this.blobContentId);

            var getSizeCmdRes = await getSizeCmd.ExecuteScalarAsync();
            long size = 0;
            if (getSizeCmdRes != DBNull.Value)
            {
                size = (long)getSizeCmdRes;
            }

            var getLocationCmd = this.mainDbConnection.CreateCommand();
            getLocationCmd.CommandText = "SELECT BlobContentLocationId FROM BlobContentLocations WHERE [Hash] = @hash AND [Size] = @size";
            getLocationCmd.Parameters.AddWithValue("@hash", hash);
            getLocationCmd.Parameters.AddWithValue("@size", size);

            var reader = await getLocationCmd.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                await reader.ReadAsync();
                return new BlobInfo((long)reader.GetInt64(reader.GetOrdinal("BlobContentLocationId")), size, hash);
            }

            var insertLocationCmd = this.mainDbConnection.CreateCommand();
            insertLocationCmd.CommandText =
                @"INSERT INTO BlobContentLocations (BlobContentId, ContentDbCSName, [Hash], [Size]) 
                    VALUES (@blobContentId, @contentDbCSName, @hash, @size);

                SET @blobContentLocationId = SCOPE_IDENTITY();";
            insertLocationCmd.Parameters.AddWithValue("@blobContentId", this.blobContentId);
            insertLocationCmd.Parameters.AddWithValue("@contentDbCSName", ConfigurationManager.AppSettings["eCase.Web:CurrentBlobDbConnectionString"]);
            insertLocationCmd.Parameters.AddWithValue("@hash", hash);
            insertLocationCmd.Parameters.AddWithValue("@size", size);
            SqlParameter blobContentLocationIdParam = new SqlParameter("@blobContentLocationId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
            insertLocationCmd.Parameters.Add(blobContentLocationIdParam);

            bool uniqueConstraintViolated = false;
            try
            {
                await insertLocationCmd.ExecuteNonQueryAsync();
            }
            catch (SqlException sqlExc)
            {
                if (sqlExc.Errors.Cast<SqlError>().Any(e => e.Number == 2601 && e.Message.Contains("UQ_BlobContents_Hash_Size")))
                {
                    uniqueConstraintViolated = true;
                }
                else
                {
                    //not unique constraint violated
                    throw;
                }
            }

            //someone uploaded this blob first
            if (uniqueConstraintViolated)
            {
                //get the location of the uploaded blob
                return new BlobInfo((long)await getLocationCmd.ExecuteScalarAsync(), size, hash);
            }

            //we succeeded in uploading the blob
            else
            {
                //set its hash and size in the BlobContents table
                var updateBlobCmd = this.blobDbConnection.CreateCommand();
                updateBlobCmd.CommandText = "UPDATE BlobContents SET [Hash] = @hash, [Size] = @size, [IsDeleted] = 0 WHERE BlobContentId = @blobContentId;";
                updateBlobCmd.Parameters.AddWithValue("@blobContentId", this.blobContentId);
                updateBlobCmd.Parameters.AddWithValue("@hash", hash);
                updateBlobCmd.Parameters.AddWithValue("@size", size);

                try
                {
                    await updateBlobCmd.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                    Logger.Error(string.Format("Failed: \"UPDATE BlobContents SET [Hash] = {0}, [Size] = {1} WHERE BlobContentId = {2}\"", hash, size, this.blobContentId));
                    throw;
                }

                return new BlobInfo((long)blobContentLocationIdParam.Value, size, hash);
            }
        }

        private Stream CreateStream()
        {
            BlobWriteStream blobStream = new BlobWriteStream(this.blobDbConnection, null, "dbo", "BlobContents", "Content", "BlobContentId", this.blobContentId);

            this.sha256 = new SHA256Managed();
            this.stream = new CryptoStream(blobStream, this.sha256, CryptoStreamMode.Write);

            return this.stream;
        }

        private SqlCommand CreateInsertCmd()
        {
            var cmd = this.blobDbConnection.CreateCommand();
            cmd.CommandText =
                @"INSERT INTO BlobContents (BlobContentId, [Hash], [Size], [Content], IsDeleted) 
                    VALUES (@id, NULL, NULL, NULL, 1);";
            cmd.Parameters.AddWithValue("@id", this.blobContentId);

            return cmd;
        }
    }
}
