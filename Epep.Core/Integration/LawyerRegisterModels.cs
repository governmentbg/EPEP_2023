using Newtonsoft.Json;

namespace Epep.Core.Integration.LawyerRegister
{
    public static class HttpClientExtensions
    {
        public static async Task<LawyerDto[]> GetLawyers(this HttpClient client, string baseUrl)
        {

            client.DefaultRequestHeaders.Add("accept", @"application/json");

            var res = await client.GetAsync(new Uri(new Uri(baseUrl), "api/attorneys"));
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LawyerDto[]>(content);
            }

            return null;
        }
    }

    public class LawyerDto
    {
        public string barAssociation { get; set; }
        public string identityNumber { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string name { get; set; }
        public string egnOrBirthDate { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string address { get; set; }
    }
}
