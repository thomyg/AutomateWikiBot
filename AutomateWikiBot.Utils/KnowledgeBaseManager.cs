using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AutomateWikiBot.Utils
{
    public class KnowledgeBaseManager
    {
        #region Setters and Constructor
        public string ServiceURL { get; set; }

        public string ClientSecrete { get; set; }

        public string ID { get; set; }

        public KnowledgeBaseManager(string serviceUrl, string clientSecrete, string iD)
        {
            this.ServiceURL = serviceUrl;
            this.ClientSecrete = clientSecrete;
            this.ID = iD;
        }
        #endregion

        #region Add Question and Answer list to Knowledgebase
        public void AddQnAToKB(Dictionary<String, String> qna)
        {
            AddToKB add = new AddToKB();
            add.qnaPairs = new List<QnaPair>();

            foreach (string key in qna.Keys)
            {
                if (key.Length>0)
                {
                    add.qnaPairs.Add(new QnaPair { answer = qna[key], question = key });
                }
            }

            QnaMakerUpdate update = new QnaMakerUpdate { addToKB = add };

            string json = JsonConvert.SerializeObject(update, Formatting.Indented);
            Console.WriteLine("json load for AddQnAToKB: " + json);
            var uri = ServiceURL + ID;
            var response = MakeAddRequest(uri, json, ClientSecrete).Result;                    
           
        }

        static async Task<string> MakeAddRequest(string url, string body, string clientSecrete)
        {
            var client = new HttpClient();
                                                         
            HttpResponseMessage response;
                        
            HttpContent httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            httpContent.Headers.Add("Ocp-Apim-Subscription-Key", clientSecrete);
            Console.WriteLine("Going to call: " + url);
            response = await client.PatchAsync(new Uri(url), httpContent);
            response.StatusCode.ToString();
            Console.WriteLine("QNAMaker MakeAddRequest statuscode: " + response.StatusCode.ToString());
            Console.WriteLine("QNAMaker MakeAddRequest response: " + response.ToString());
            return response.StatusCode.ToString();            
        }
        #endregion

        #region Publish Knowledgebase
        public void PublishKB()
        {
            var uri = ServiceURL + ID;
            var task = MakePublishRequest(uri, "{body}", ClientSecrete).Result;

        }

        static async Task<string> MakePublishRequest(string url, string body, string clientSecrete)
        {
            var client = new HttpClient();

            HttpResponseMessage response;

            HttpContent httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            httpContent.Headers.Add("Ocp-Apim-Subscription-Key", clientSecrete);
            Console.WriteLine("Going to call: " + url);
            response = await client.PutAsync(new Uri(url), httpContent);
            response.StatusCode.ToString();            
            Console.WriteLine("QNAMaker MakePublishRequest statuscode: " + response.StatusCode.ToString());
            Console.WriteLine("QNAMaker MakePublishRequest response: " + response.ToString());
            return response.StatusCode.ToString();

        }
        #endregion
    }

    #region HttpClient extension
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri requestUri, HttpContent iContent)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = iContent
            };

            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = await client.SendAsync(request);
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine("ERROR: " + e.ToString());
            }

            return response;
        }
    }
    #endregion
}
