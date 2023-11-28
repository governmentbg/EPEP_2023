using Ionic.Zip;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace eCase.Common.Crypto
{
    public interface IEncryptString
    {
        string Encrypt(string value);
        string Decrypt(string value);
    } 

    public class ConfigurationBasedStringEncrypter // : IEncryptString
    {
        private static readonly ICryptoTransform _encrypter;
        private static readonly ICryptoTransform _decrypter;

        static ConfigurationBasedStringEncrypter()
        {
            var key = "$$$ сецрет $$$";
            var useHashingString = "no need";

            bool useHashing = true;

            if (string.Compare(useHashingString, "false", true) == 0)
            {
                useHashing = false;
            }

            byte[] keyArray = null;

            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
            {
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            _encrypter = tdes.CreateEncryptor();
            _decrypter = tdes.CreateDecryptor();
            tdes.Clear();
        }

        #region IEncryptionSettingsProvider Members

        public static string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var bytes = UTF8Encoding.UTF8.GetBytes(value);

            var encryptedBytes = _encrypter.TransformFinalBlock(bytes, 0, bytes.Length);
            var encrypted = Convert.ToBase64String(encryptedBytes);

            return encrypted;
        }
        public static string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var bytes = Convert.FromBase64String(value);

            var decryptedBytes = _decrypter.TransformFinalBlock(bytes, 0, bytes.Length);
            var decrypted = UTF8Encoding.UTF8.GetString(decryptedBytes);

            return decrypted;
        }

        #endregion
    } 
}
