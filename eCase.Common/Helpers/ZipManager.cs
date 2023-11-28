using System;
using System.IO;
using Ionic.Zip;

namespace eCase.Common.Helpers
{
    public static class ZipManager
    {
        public static byte[] Compress(byte[] contentToZip, string filename)
        {
            byte[] zipContent = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipFile zf = new ZipFile())
                {
                    zf.AddEntry(filename, contentToZip);
                    zf.Save(ms);
                }

                zipContent = ms.ToArray();
                return zipContent;
            }
        }

        public static byte[] Decompress(byte[] zipContent)
        {
            using (MemoryStream zipStream = new MemoryStream(zipContent))
            {
                if (ZipFile.IsZipFile(zipStream, false))
                {
                    zipStream.Position = 0;

                    using (ZipFile zf = ZipFile.Read(zipStream))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            if (zf.Count == 1)
                                zf[0].Extract(ms);

                            return ms.ToArray();
                        }
                    }
                }
                else
                    throw new ArgumentException("The specified content does not represent a .zip file.");
            }
        }

        public static string CreateFileName(string uid, string mimeType)
        {
            switch (mimeType)
            {
                case "text/html":
                    uid += ".htm";
                    break;
                case "application/msword":
                    uid += ".doc";
                    break;
                case "text/plain":
                    uid += ".txt";
                    break;
                case "application/pdf":
                    uid += ".pdf";
                    break;
                default:
                    break;
            }

            return uid;
        }
    }
}
