namespace CryptoLabBlockCiphersClient
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    class RestClient
    {
        private readonly HttpClient client;

        public RestClient()
        {
            this.client = new HttpClient();
        }

        public async Task ProcessRepositories(int counter)
        {
            this.client.DefaultRequestHeaders.Accept.Clear();

            var stringTask = await this.client.GetStringAsync(
                                 $"http://localhost/CryptoLabs/api/EncryptionModeOracle/alice/{counter}/verify");
            Console.WriteLine(stringTask);
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(
                        Convert.ToBase64String(
                            Encoding.UTF8.GetBytes(
                                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"))),
                    Encoding.UTF8,
                    "application/json");

                var res = await this.client.PostAsync(
                              $"http://localhost/CryptoLabs/api/EncryptionModeOracle/alice/{counter}/noentropy",
                              content);
                var result = await res.Content.ReadAsStringAsync();
                var rawResult = Convert.FromBase64String(result);
                string hex = BitConverter.ToString(rawResult).Replace("-", string.Empty);
                Console.WriteLine(hex);

            }
            //{
            //    var content = new StringContent(JsonConvert.SerializeObject(Convert.ToBase64String(Encoding.UTF8.GetBytes("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"))), Encoding.UTF8, "application/json");

            //    var res = await this.client.PostAsync("http://localhost/CryptoLabs/api/EncryptionModeOracle/alice/1", content);
            //    var result = await res.Content.ReadAsStringAsync();
            //    var rawResult = Convert.FromBase64String(result);
            //    string hex = BitConverter.ToString(rawResult).Replace("-", string.Empty);
            //    Console.WriteLine(hex);
            //}
        }
    }
}
