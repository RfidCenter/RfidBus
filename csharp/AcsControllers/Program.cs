using System;

namespace AcsControllers
{
    class Program
    {
        static void Main(string[] args)
        {
            SphinxClient.Initialize();
            //SphinxClient.AddKeys(new []{ "FAE47404" });
            SphinxClient.GetKeys();
            WaitForKey("Press ESC to stop.", ConsoleKey.Escape);
        }

        private static void WaitForKey(string message, ConsoleKey key)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            while (Console.ReadKey(true).Key != key)
            {
            }
        }
    }

}
