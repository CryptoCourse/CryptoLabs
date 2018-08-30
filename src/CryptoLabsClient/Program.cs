namespace CryptoLabsClient
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            while (true)
            {
                i++;
                try
                {
                    Console.WriteLine("NewIteration");
                    Console.ReadLine();

                    RestClient client = new RestClient();
                    client.TestConnection("http://192.168.13.128").GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
