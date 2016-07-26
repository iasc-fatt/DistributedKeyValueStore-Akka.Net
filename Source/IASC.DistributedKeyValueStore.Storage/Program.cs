using Akka.Actor;
using Akka.Configuration;
using System;
using System.Configuration;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class Program
    {
        private static ActorSystem KvActorSystem;
        private static readonly long maxValueLength = 5;

        private static void Main(string[] args)
        {
            Console.WriteLine("Indicate the storaget number");
            var storageNumber = Console.ReadLine();

            var stringConfig = @"
                akka {
                    loglevel = INFO

                    actor {
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        debug {
                            receive = on
                            autoreceive = on
                            lifecycle = on
                            event-stream = on
                            unhandled = on
                        }
                    }

                    remote {
                        helios.tcp {
  	                        port = CONFIG_PORT
  	                        hostname = localhost
                        }
                    }
                }
            ";

            stringConfig = stringConfig.Replace("CONFIG_PORT", "805" + storageNumber);
            var config = ConfigurationFactory.ParseString(stringConfig);

            Console.WriteLine("Creating KvActorSystem");
            KvActorSystem = ActorSystem.Create("KvActorSystem", config);

            Console.WriteLine("Creating SupervisorActor");
            var supervisor = KvActorSystem.ActorOf(Props.Create(() => new SupervisorActor(maxValueLength, storageNumber)), "storages");

            Console.WriteLine("Ready");
            KvActorSystem.WhenTerminated.Wait();
        }
    }
}