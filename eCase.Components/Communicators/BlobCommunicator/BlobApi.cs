using System;
using System.Configuration;

namespace eCase.Components.Communicators
{
    public class BlobApi
    {
        public static Uri CreateRedirectUri(Guid fileKey)
        {
            string blobServerLocation = ConfigurationManager.AppSettings["eCase.Components:BlobServerLocation"];

            if (!blobServerLocation.EndsWith("/"))
            {
                blobServerLocation += "/";
            }

            return new Uri(new Uri(blobServerLocation), fileKey.ToString());
        }
    }
}