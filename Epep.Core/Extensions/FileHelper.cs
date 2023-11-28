using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.Extensions
{
    public class FileHelper
    {
        public static byte[] LoadFileFromPath(string path)
        {
            if (File.Exists(path))
            {
                using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    int numBytesToRead = (int)file.Length;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        int n = file.Read(bytes, numBytesRead, numBytesToRead);

                        if (n == 0) break;

                        numBytesRead += n;
                        numBytesToRead -= n;
                    }

                    return bytes;
                }
            }
            return null;
        }
        public static string GetTextFromFile(string path)
        {
            var fileBytes = LoadFileFromPath(path);
            if (fileBytes != null)
            {
                return Encoding.UTF8.GetString(fileBytes);
            }
            return string.Empty;
        }

        public static List<T> LoadDataFromFile<T>(string path) where T : class
        {
            var jsonData = GetTextFromFile(path);

            List<T> result = new List<T>();

            if (!String.IsNullOrEmpty(jsonData))
            {
                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };

                result = JsonConvert.DeserializeObject<List<T>>(jsonData, new JsonSerializerSettings() { ContractResolver = contractResolver });
            }

            return result;
        }
    }
}
