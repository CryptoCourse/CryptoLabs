using System;
using System.Net.Http;

namespace CryptoLabBlockCyphersClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            RestClient client = new RestClient();
            client.ProcessRepositories().GetAwaiter().GetResult();
        }        
    }
}
