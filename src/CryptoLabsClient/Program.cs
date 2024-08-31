using System;
using System.Threading.Tasks;

namespace CryptoLabsClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int i = 0;
            while (true)
            {
                i++;
                try
                {
                    Console.WriteLine("NewIteration");
                    Console.ReadLine();

                    using (RestClient client = new RestClient())
                    {
                        await client.TestConnection("http://localhost:50412");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
