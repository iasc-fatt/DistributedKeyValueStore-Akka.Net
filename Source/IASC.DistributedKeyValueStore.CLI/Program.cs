﻿using IASC.DistributedKeyValueStore.Client;
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
                    try
                    {
                        if (action == "insert")
                        {
                            var key = commandSegments[1];
                            var value = commandSegments[2];

                            var succed = client.Insert(key, value).Result;

                            if (succed.Any())
                                Console.WriteLine("inserted");
                            else
                                Console.WriteLine("value not inserted");
                        }
                        else if (action == "remove")
                        {
                            var key = commandSegments[1];

                            client.Remove(key).Wait();

                            Console.WriteLine("removed");
                        }
                        else if (action == "lookup")
                        {
                            var key = commandSegments[1];

                            var value = client.Lookup(key).Result;

                            if (value.Any())
                                Console.WriteLine("value found: {0}", value.Single().Value);
                            else
                                Console.WriteLine("key not found");
                        }
                        else if (action == "search")
                        {
                            var comparison = commandSegments[1];
                            var valueToCompare = commandSegments[2];

                            var values = client.Search(valueToCompare, comparison).Result;

                            Console.WriteLine("found: {0}", string.Join(", ", values));
                        }
                        else if (action == "keys")
                        {
                            var regex = ".*";
                            if (commandSegments.Length > 1)
                            {
                                regex = commandSegments[1];
                            }

                            var values = client.Keys(regex).Result;

                            Console.WriteLine("found: {0}", string.Join(", ", values));
                        }
                        else if (action == "status")
                        {
                            var statuses = client.HealthCheck().Result;

                            Console.WriteLine("Status:\n{0}", string.Join("\n", statuses));
                        }
                        else if (action == "kill")
                        {
                            var succeed = client.KillActor(string.Concat("/user/storages/", commandSegments[1])).Result;

                            if (succeed.Any())
                                Console.WriteLine("Killed: {0}", commandSegments[1]);
                            else
                                Console.WriteLine("Could not be killed");
                        }
                        else if (action == "exit")
                        {
                            break;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Could not execute the command");
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