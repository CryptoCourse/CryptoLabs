using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

            var stringTask = await this.client.GetStringAsync("http://localhost/CryptoLabs/api/values");

            var content = new StringContent(JsonConvert.SerializeObject(Convert.ToBase64String(Encoding.UTF8.GetBytes("qwerty"))), Encoding.UTF8, "application/json");

            var res = await this.client.PostAsync("http://localhost/CryptoLabs/api/EncryptionModeOracle/alice/1/noentropy", content);
            var result = await res.Content.ReadAsStringAsync();

            Console.WriteLine(stringTask);
            Console.WriteLine(result);
        }
    }
}
