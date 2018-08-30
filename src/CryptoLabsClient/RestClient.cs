namespace CryptoLabsClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    class RestClient
    {
        private readonly HttpClient client;

        public RestClient()
        {
            this.client = new HttpClient();
        }

        public async Task<T> PostObject<T>(string uri, object obj)
        {
            var content = new StringContent(JsonConvert.SerializeObject(obj, Formatting.Indented), Encoding.UTF8, "application/json");

            var result = await this.client.PostAsync(uri, content);

            var err = await RestClient.HandleResult(result);
            if (err != null)
            {
                throw err;
            }

            var resultedContent = await result.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(resultedContent))
            {
                throw new Exception("Nothing to return");
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)resultedContent;
            }

            var resultedObject = JsonConvert.DeserializeObject<T>(resultedContent);
            return resultedObject;
        }

        public async Task PostObject(string uri, object obj)
        {
            var content = new StringContent(JsonConvert.SerializeObject(obj, Formatting.Indented), Encoding.UTF8, "application/json");

            var result = await this.client.PostAsync(uri, content);

            var err = await RestClient.HandleResult(result);
            if (err != null)
            {
                throw err;
            }
        }

        public async Task<T> GetObject<T>(string uri)
        {
            var result = await this.client.GetAsync(uri);

            var err = await RestClient.HandleResult(result);
            if (err != null)
            {
                throw err;
            }

            var resultedContent = await result.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(resultedContent))
            {
                throw new Exception("Nothing to return");
            }

            if (typeof(T) == typeof(string))
            {
                return (T)(object)resultedContent;
            }

            var resultedObject = JsonConvert.DeserializeObject<T>(resultedContent);
            return resultedObject;
        }

        public async Task TestConnection(string baseUri)
        {
            var getResult = await this.GetObject<string[]>($"{baseUri}/api/");
            if (getResult[0] != "Api is working!")
            {
                throw new Exception();
            }

            Console.WriteLine($"GET working, result = [{getResult[0]}]");

            var postResult = await this.PostObject<string>($"{baseUri}/api/values", "someString");
            if (postResult != "POST request for value = someString")
            {
                throw new Exception();
            }

            Console.WriteLine($"POST working, result = [{postResult}]");
        }

        /// <summary>
        /// Производит проверку результата
        /// </summary>
        /// <param name="response">
        /// Запрос для проверки
        /// </param>
        /// <returns></returns>
        private static async Task<Exception> HandleResult(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                {
                    var result = string.Empty;
                    try
                    {
                        result = await response.Content.ReadAsStringAsync();
                        var message = JObject.Parse(result).GetValue("Message").Value<string>();
                        return new Exception($"Произошла ошибка при работе с DSS: {message}");
                    }
                    catch (Exception)
                    {
                        return new Exception($"Произошла ошибка при работе с DSS: {response.StatusCode}: {result}");
                    }
                }
                default:
                {
                    return new Exception($"Произошла ошибка при работе с DSS: {response.StatusCode}");
                }
            }
        }
    }
}
