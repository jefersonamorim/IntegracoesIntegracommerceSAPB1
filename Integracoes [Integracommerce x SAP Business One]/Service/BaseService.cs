using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using System.Net.Http.Headers;

namespace Integracommerce.Service
{
    
    class BaseService
    {
        //static HttpClient client;
        //static string uri = "https://{{accountName}}.{{environment}}.com.br/";
        static string api = ConfigurationManager.AppSettings["api"];

        static string authorization = ConfigurationManager.AppSettings["authorization"];

        public BaseService() {
            
        }

        public static HttpClient BuildClient() {
            string baseUri = api;
            //if (client == null)
            // {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Add aqui os headers necessários IntegraCommerce
            //Criar string base64 user:senha
            client.DefaultRequestHeaders.Add("cache-control", "no-cache");
            client.DefaultRequestHeaders.Add("authorization", authorization);


            //}
            return client;
        }

        public static HttpClient BuildClientPedido()
        {
            string baseUriPedido = api;

            HttpClient clientPedido = new HttpClient();

            clientPedido.BaseAddress = new Uri(baseUriPedido);
            clientPedido.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Add aqui os headers necessários IntegraCommerce
            //Criar string base64 user:senha
            clientPedido.DefaultRequestHeaders.Add("cache-control", "no-cache");
            clientPedido.DefaultRequestHeaders.Add("Authorization", authorization);

            return clientPedido;
        }

        /*public static HttpClient BuildClientLogistics()
        {
            string baseUri = "https://logistics" + "." + environment + ".com.br/";
            //if (client == null)
            //{
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-vtex-api-appKey", appKey);
            client.DefaultRequestHeaders.Add("x-vtex-api-appToken", appToken);
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.vtex.ds.v10+json");
            client.DefaultRequestHeaders.Add("REST-Range", "resources=0-10");
                
            //}
            return client;
        }*/
    }
}
