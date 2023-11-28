using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace IO.PaymentProvider.Models
{

    public interface IPaymentProviderClient
    {
        Task<PaymentRegistrationResultModel> Register(PaymentRegistrationModel request);
        Task<PaymentStatusResultModel> Status(PaymentStatusModel request);
    }

    public class PaymentProviderClient : IPaymentProviderClient
    {
        private readonly IHttpClientFactory clientFactory;
        private HttpClient client;
        private Uri uploadUrl;

        public PaymentProviderClient(

              IHttpClientFactory _clientFactory,
              IConfiguration configuration)
        {
            clientFactory = _clientFactory;
            var baseUrl = configuration.GetSection("PaymentProvider:URI").Value;
            uploadUrl = new Uri(baseUrl);
            //uploadUrl = new Uri("https://localhost:7053/");
        }

        void initClient()
        {
            if (client != null)
            {
                return;
            }
            client = clientFactory.CreateClient(PaymentProviderConstants.HttpClientName);
        }

        public async Task<PaymentRegistrationResultModel> Register(PaymentRegistrationModel request)
        {
            initClient();
            return await sendData<PaymentRegistrationResultModel>("payment/registerasync", request);
        }
        public async Task<PaymentStatusResultModel> Status(PaymentStatusModel request)
        {
            initClient();
            return await sendData<PaymentStatusResultModel>("payment/statusasync", request);
        }

        private async Task<Tresponse> sendData<Tresponse>(string methodName, object data) where Tresponse : class
        {
            //var apiKey = repo.AllReadonly<CourtApiKey>()
            //                        .Where(x => x.CourtId == courtId)
            //                        .Select(x => new
            //                        {
            //                            x.Key,
            //                            x.Secret
            //                        }).FirstOrDefault();
            //if (apiKey == null)
            //{
            //    return null;
            //}
            string requestBody = JsonConvert.SerializeObject(data);
            var requestBytes = System.Text.Encoding.UTF8.GetBytes(requestBody);
            //var hass = cryptoHelper.ComputeHash(requestBytes, apiKey.Secret);
            //var autorizationToken = $"{apiKey.Key}.{hass}";


            Uri address = new Uri(uploadUrl, methodName);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", autorizationToken);
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(address.AbsoluteUri, content);
            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Tresponse>(responseContent);
            }
            else
            {
                //var resError = Activator.CreateInstance<Tresponse>();
                //resError.Error = new ErrorModel()
                //{
                //    ErrorType = "Response Error",
                //    Reason = response.StatusCode.ToString()
                //};
                return null;
                //throw new Exception($"Response Error : {response.StatusCode.ToString()}");
            }
        }
    }
}
