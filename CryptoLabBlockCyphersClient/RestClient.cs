using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLabBlockCyphersClient
{
    class RestClient
    {
        private readonly HttpClient client;

        public RestClient()
        {
            this.client = new HttpClient();
        }

        public async Task ProcessRepositories()
        {
            this.client.DefaultRequestHeaders.Accept.Clear();

            var stringTask = this.client.GetStringAsync("htpp://localhost/api/values");

            var content = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("", "login")
                });

            await this.client.PostAsync("htpp://localhost/api/values", content); 

            var msg = await stringTask;
            Console.Write(msg);
        }
    }
}
