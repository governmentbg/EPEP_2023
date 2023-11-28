using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Linq;
using System.Web.Configuration;

namespace eCase.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext<eCaseUserManager>(eCaseUserManager.Create);

            var owinAesProtectorKey = System.Configuration.ConfigurationManager
                                .AppSettings["eCase.Web:OwinAesProtectorKey"];

            app.CreatePerOwinContext<SignInManager>(SignInManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = Constants.ProjectName,
                LoginPath = new PathString("/Account/Login"),
                SlidingExpiration = false,
                ExpireTimeSpan = ((SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState")).Timeout,
                LogoutPath = new PathString("/Account/Logout"),
                CookieName = Constants.CookieName,
                TicketDataFormat = new Microsoft.Owin.Security.DataHandler
                    .TicketDataFormat(new AesDataProtector(owinAesProtectorKey)),
                Provider = new CookieAuthenticationProvider
                {
                    OnApplyRedirect = ctx =>
                    {
                        if (!IsAjaxRequest(ctx.Request))
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                }
            });
        }

        private static bool IsAjaxRequest(IOwinRequest request)
        {
            return request.Path.StartsWithSegments(new PathString("/api"));
        }
    }

    internal class AesDataProtector : Microsoft.Owin.Security.DataProtection.IDataProtector
    {
        #region Fields

        private byte[] key;

        #endregion Fields

        #region Constructors

        public AesDataProtector(string key)
        {
            using (var sha1 = new System.Security.Cryptography.SHA256Managed())
            {
                this.key = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
            }
        }

        #endregion Constructors

        #region IDataProtector Methods

        public byte[] Protect(byte[] data)
        {
            byte[] dataHash;
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                dataHash = sha.ComputeHash(data);
            }

            using (System.Security.Cryptography.AesManaged aesAlg = new System.Security.Cryptography.AesManaged())
            {
                aesAlg.Key = this.key;
                aesAlg.GenerateIV();

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, 16);

                    using (var csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                    using (var bwEncrypt = new System.IO.BinaryWriter(csEncrypt))
                    {
                        bwEncrypt.Write(dataHash);
                        bwEncrypt.Write(data.Length);
                        bwEncrypt.Write(data);
                    }
                    var protectedData = msEncrypt.ToArray();
                    return protectedData;
                }
            }
        }

        public byte[] Unprotect(byte[] protectedData)
        {
            using (System.Security.Cryptography.AesManaged aesAlg = new System.Security.Cryptography.AesManaged())
            {
                aesAlg.Key = this.key;

                using (var msDecrypt = new System.IO.MemoryStream(protectedData))
                {
                    byte[] iv = new byte[16];
                    msDecrypt.Read(iv, 0, 16);

                    aesAlg.IV = iv;

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    using (var csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                    using (var brDecrypt = new System.IO.BinaryReader(csDecrypt))
                    {
                        var signature = brDecrypt.ReadBytes(32);
                        var len = brDecrypt.ReadInt32();
                        var data = brDecrypt.ReadBytes(len);

                        byte[] dataHash;
                        using (var sha = new System.Security.Cryptography.SHA256Managed())
                        {
                            dataHash = sha.ComputeHash(data);
                        }

                        if (!dataHash.SequenceEqual(signature))
                            throw new System.Security.SecurityException("Signature does not match the computed hash");

                        return data;
                    }
                }
            }
        }

        #endregion IDataProtector Methods
    }
}