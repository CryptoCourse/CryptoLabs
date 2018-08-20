namespace CryptoLabBlockCiphersClient
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
                    client.ProcessRepositories(i).GetAwaiter().GetResult();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
