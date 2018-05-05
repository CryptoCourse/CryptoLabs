using System;
using System.Net.Http;

namespace CryptoLabBlockCyphersClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Hello World!");
                Console.ReadLine();

                RestClient client = new RestClient();
                client.ProcessRepositories().GetAwaiter().GetResult();

                Console.ReadLine();
                
            }
        }
    }
}
