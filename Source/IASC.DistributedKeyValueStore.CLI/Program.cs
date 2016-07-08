using IASC.DistributedKeyValueStore.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace IASC.DistributedKeyValueStore.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var serverAddress = ConfigurationManager.AppSettings["server.address"];
            using (var client = new KvClient(serverAddress))
            {
                do
                {
                    ShortPause();

                    Console.WriteLine();
                    Console.WriteLine("enter a command and hit enter");

                    var command = Console.ReadLine();
                    var commandSegments = command.Split(',');
                    var action = commandSegments[0];

                    if (action == "insert")
                    {
                        var key = commandSegments[1];
                        var value = commandSegments[2];

                        client.Insert(key, value).Wait();

                        Console.WriteLine("inserted");
                    }
                    else if (action == "lookup")
                    {
                        var key = commandSegments[1];

                        var value = client.Lookup(key).Result;

                        if(value.Any())
                            Console.WriteLine("value found: {0}", value.Single().Value);
                        else
                            Console.WriteLine("key not found");
                    }
                    else if (action == "search")
                    {
                        var comparison = commandSegments[1];
                        var valueToCompare = commandSegments[2];

                        var values = client.Search(valueToCompare, comparison).Result;

                        Console.WriteLine("found: {0}", string.Join(",", values));
                    }
                    else if (action == "exit")
                    {
                        break;
                    }
                } while (true);
            }

            Console.ReadKey();
            Environment.Exit(1);
        }

        // Perform a short pause for demo purposes to allow console to update nicely
        private static void ShortPause()
        {
            Thread.Sleep(450);
        }
    }
}