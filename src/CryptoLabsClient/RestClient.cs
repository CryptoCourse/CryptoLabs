using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CryptoLabsClient
{
    class RestClient : IDisposable
    {
        private readonly HttpClient client;

        private readonly JsonSerializerOptions options = new() { WriteIndented = true };

        private bool disposedValue;

        public RestClient()
        {
            this.client = new HttpClient();
        }

        public async Task<T> PostObject<T>(string uri, object obj)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(
                    obj, 
                    options: this.options), 
                Encoding.UTF8, 
                "application/json");

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

            var resultedObject = JsonSerializer.Deserialize<T>(resultedContent);
            return resultedObject;
        }

        public async Task PostObject(string uri, object obj)
        {
            StringContent content = new(
                JsonSerializer.Serialize(
                    obj,
                    options: this.options),
                Encoding.UTF8, 
                "application/json");

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

            var resultedObject = JsonSerializer.Deserialize<T>(resultedContent);
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
                        var message = JsonObject.Parse(result)["Message"].GetValue<string>();
                        return new Exception($"Произошла ошибка при работе с Сервером: {message}");
                    }
                    catch (Exception)
                    {
                        return new Exception($"Произошла ошибка при работе с Сервером: {response.StatusCode}: {result}");
                    }
                }
                default:
                {
                    return new Exception($"Произошла ошибка при работе с Сервером: {response.StatusCode}");
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.client?.Dispose();
                }
                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
