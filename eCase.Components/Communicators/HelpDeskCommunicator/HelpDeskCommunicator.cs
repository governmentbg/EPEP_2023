using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eCase.Components.Communicators.HelpDeskCommunicator
{
    public enum ActionResultCode
    {
        Ok = 1,
        Error = 2
    }

    public class UnitSimpleIDO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IssuerEmail { get; set; }
    }

    public class ActionResultDO
    {
        public ActionResultCode ResultCode { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
        public UnitSimpleIDO Data { get; set; }
    }

    public class HelpDeskCommunicator : IHelpDeskCommunicator
    {
        public string Login(string domain, string clientId, string secret, string username, string password)
        {
            if (!domain.EndsWith("/"))
            {
                domain += "/";
            }

            using (TokenClient tokenClient = new TokenClient($"{domain}api/token", clientId, secret))
            {
                var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(username, password);
                var tokenResponseResult = tokenResponse.Result;

                if (tokenResponseResult.IsError || tokenResponseResult.TokenType != "Bearer")
                    throw new Exception(String.Format("HelpDesk get access token exception - isError: {0}, tokenType: {1}",
                        tokenResponseResult.IsError,
                        tokenResponseResult.TokenType));

                return tokenResponseResult.AccessToken;
            }
        }

        public void Send(string domain, string accessToken, string subject, string name, string email, string description)
        {
            object postData = new
            {
                name = String.Format("Съобщение за: '{0}'{1}", subject, !String.IsNullOrWhiteSpace(name) ? String.Format(" от {0}", name.Trim()) : String.Empty),
                description = !String.IsNullOrWhiteSpace(description) ? description.Trim() : null,
                issuerEmail = !String.IsNullOrWhiteSpace(email) ? email.Trim() : null
            };

            string postDataJson = JsonConvert.SerializeObject(postData);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(domain);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", accessToken));

                StringContent postContent = new StringContent(postDataJson, Encoding.UTF8, "application/json");

                var responseResult = client.PostAsync("/api/units/createSimple", postContent).Result;

                if (!responseResult.IsSuccessStatusCode)
                    throw new Exception(String.Format("HelpDesk create unit exception - statusCode: {0}", responseResult.StatusCode));

                ActionResultDO actionResultDO = JsonConvert.DeserializeObject<ActionResultDO>(responseResult.Content.ReadAsStringAsync().Result);

                if (actionResultDO.ResultCode != ActionResultCode.Ok)
                    throw new Exception(String.Format("HelpDesk create unit exception - actionResultDO.ResultCode: {0}", actionResultDO.ResultCode));
            }
        }

        
    }
}
