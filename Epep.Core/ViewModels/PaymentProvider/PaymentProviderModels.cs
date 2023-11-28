using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace IO.PaymentProvider.Models
{
    public static class PaymentProviderExtensions
    {

        public static void ConfigurePaymentProviderHttpClients(this IServiceCollection services, int timeoutSeconds = 60)
        {

            services.AddHttpClient(PaymentProviderConstants.HttpClientName, client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });
        }
    }

    public class PaymentProviderConstants
    {
        public const string HttpClientName = "paymentProviderClient";

        public class Status
        {
            public const int Registered = 1;
            public const int Paid = 2;
            public const int Failed = 3;
        }
    }



    public class PaymentRegistrationModel
    {
        public string ClientCode { get; set; }

        /// <summary>
        /// Сума в стотинки 20.10 лв -> 2010
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// BGN,EUR
        /// </summary>
        public string Currency { get; set; }

        public string InvoiceNumber { get; set; }

        public string SuccessUrl { get; set; }

        public string FailUrl { get; set; }

        /// <summary>
        /// BG,EN
        /// </summary>
        public string Language { get; set; }
    }

    public class PaymentRegistrationResultModel
    {
        //0-OK, 1-Innvalid clientcode, 2-Invalid amount, 3-invalid currency or language, 4-Invalid invoice number
        public int Code { get; set; }

        public string ErrorMessage { get; set; }

        public string Gid { get; set; }

        public string CardFormUrl { get; set; }
    }

    public class PaymentStatusModel
    {
        public string Gid { get; set; }
    }

    public class PaymentStatusResultModel
    {
        //payment.paymentstatus.code
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
