using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace eCase.Common.Helpers
{
    public static class CryptId
    {
        public static string Encrypt(int value)
        {
            return SecurityUtils.EncryptValue(value);
        }

        public static Dictionary<string, string> Decrypt(string encryptedText)
        {
            return SecurityUtils.DecryptValue(encryptedText);
        }
    }

    public static class SecurityUtils
    {
        #region Crypto Mix

        private static class RandomCryptoKeys
        {
            static RandomCryptoKeys()
            {
                Rijndael r = Rijndael.Create();
                r.GenerateKey();
                r.GenerateIV();

                RijndaelKey = r.Key;
                RijndaelIV = r.IV;
            }

            public static byte[] RijndaelKey;
            public static byte[] RijndaelIV;
        }

        private static string EncryptToBase64String(string data, byte[] IV, byte[] key)
        {
            Rijndael r = Rijndael.Create();
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, r.CreateEncryptor(key, IV), CryptoStreamMode.Write))
                using (StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(data);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static string DecryptFromBase64String(string encryptedData, byte[] IV, byte[] key)
        {
            byte[] encryptedByteContens = Convert.FromBase64String(encryptedData);
            Rijndael r = Rijndael.Create();
            using (MemoryStream ms = new MemoryStream(encryptedByteContens))
            {
                using (CryptoStream cs = new CryptoStream(ms, r.CreateDecryptor(key, IV), CryptoStreamMode.Read))
                using (StreamReader sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        #endregion

        #region Redirect Data

        private static bool? _encryptRedirectData = null;
        private static bool EncryptRedirectDataConfigValue
        {
            get
            {
                if (!_encryptRedirectData.HasValue)
                {
                    _encryptRedirectData = true;
                    string rdConfig = @"///"; 
                    //ConfigurationManager.AppSettings["EncryptRedirectData"];
                    if (!string.IsNullOrEmpty(rdConfig))
                    {
                        _encryptRedirectData = bool.Parse(rdConfig);
                    }
                }
                return _encryptRedirectData.Value;
            }
        }

        private static Random random = new Random();
        private static string GetRandomData()
        {
            return random.Next(1, 1).ToString();
        }

        public static string EncryptValue(int redirectData)
        {
            string contents = redirectData.ToString();

            if (!EncryptRedirectDataConfigValue)
                return contents;

            contents = GetRandomData() + '?' + contents;

            return SecurityUtils.EncryptToBase64String(
                contents,
                SecurityUtils.RandomCryptoKeys.RijndaelIV,
                SecurityUtils.RandomCryptoKeys.RijndaelKey);
        }

        public static Dictionary<string, string> DecryptValue(string encryptedStringContents)
        {
            string content;

            if (EncryptRedirectDataConfigValue)
            {
                string randomDataConcatenatedContents =
                    SecurityUtils.DecryptFromBase64String(
                        encryptedStringContents,
                        SecurityUtils.RandomCryptoKeys.RijndaelIV,
                        SecurityUtils.RandomCryptoKeys.RijndaelKey);

                string[] contentsPair = randomDataConcatenatedContents.Split('?');

                content = contentsPair[1];
            }
            else
            {
                content = encryptedStringContents;
            }

            return new Dictionary<string, string>() { { "id", content } };
        }

        #endregion
    }
}