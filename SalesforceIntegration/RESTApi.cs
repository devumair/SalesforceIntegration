using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SalesforceIntegration
{
    public abstract class RESTApi
    {
        public const string LoginEndpoint = "https://login.salesforce.com/services/oauth2/token";
        public const string ApiEndpoint = "/services/data/v36.0/"; //Use your org's version number

        private string Username { get; set; } = "XXX@XXX.com";
        private string Password { get; set; } = "XXXX";
        private string Token { get; set; }
        private string ClientId { get; set; } = "XXXX";
        private string ClientSecret { get; set; } = "XXXX";
        public string AuthToken { get; set; }
        public string ServiceUrl { get; set; }

        readonly protected HttpClient Client;

        public RESTApi()
        {
            Client = new HttpClient();

            InitRESTApi();
        }

        private void InitRESTApi()
        {
            HttpContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
              {"grant_type", "password"},
              {"client_id", ClientId},
              {"client_secret", ClientSecret},
              {"username", Username},
              {"password", Password}
            });

            HttpResponseMessage message = Client.PostAsync(LoginEndpoint, content).Result;

            string response = message.Content.ReadAsStringAsync().Result;
            JObject obj = JObject.Parse(response);

            AuthToken = (string)obj["access_token"];
            ServiceUrl = (string)obj["instance_url"];

            Console.WriteLine("response: {0}", response);
            Console.WriteLine("AuthToken: {0}", AuthToken);
            Console.WriteLine("ServiceUrl: {0}", ServiceUrl);
        }

        protected string GetSOjectDetail(string sObjectName)
        {
            string restQuery = $"{ServiceUrl}{ApiEndpoint}sobjects/{sObjectName}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restQuery);
            request.Headers.Add("Authorization", "Bearer " + AuthToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = Client.SendAsync(request).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        protected string QueryRecord(string queryMessage)
        {
            string restQuery = $"{ServiceUrl}{ApiEndpoint}query?q={queryMessage}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restQuery);
            request.Headers.Add("Authorization", "Bearer " + AuthToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = Client.SendAsync(request).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        protected string UpdateRecord(string updateMessage, string recordType, string recordId)
        {
            HttpContent contentUpdate = new StringContent(updateMessage, Encoding.UTF8, "application/json");

            string uri = $"{ServiceUrl}{ApiEndpoint}sobjects/{recordType}/{recordId}?_HttpMethod=PATCH";

            HttpRequestMessage requestUpdate = new HttpRequestMessage(HttpMethod.Post, uri);
            requestUpdate.Headers.Add("Authorization", "Bearer " + AuthToken);
            requestUpdate.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsons"));
            requestUpdate.Content = contentUpdate;

            HttpResponseMessage response = Client.SendAsync(requestUpdate).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        protected string CreateRecord(string createMessage, string recordType)
        {
            HttpContent contentCreate = new StringContent(createMessage, Encoding.UTF8, "application/json");
            string uri = $"{ServiceUrl}{ApiEndpoint}sobjects/{recordType}";

            HttpRequestMessage requestCreate = new HttpRequestMessage(HttpMethod.Post, uri);
            requestCreate.Headers.Add("Authorization", "Bearer " + AuthToken);
            requestCreate.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestCreate.Content = contentCreate;

            HttpResponseMessage response = Client.SendAsync(requestCreate).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
