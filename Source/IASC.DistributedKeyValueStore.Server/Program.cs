using Akka.Actor;
using NDesk.Options;
using System;
using System.Configuration;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class Program
    {
        private static ActorSystem KvActorSystem;
        private static readonly long maxKeyLength = Convert.ToInt64(ConfigurationManager.AppSettings["maxKeyLength"]);
        private static readonly long maxValueLength = Convert.ToInt64(ConfigurationManager.AppSettings["maxValueLength"]);
        private static readonly long maxStorageKeys = Convert.ToInt64(ConfigurationManager.AppSettings["maxStorageKeys"]);

        private static void Main(string[] args)
        {
            string value = null;

            var p = new OptionSet() {
                { "value=", "the {NAME} of someone to greet.",
                   v => value = v }
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `--help' for more information.");
                return;
            }

            Console.WriteLine("Creating KvActorSystem");
            KvActorSystem = ActorSystem.Create("KvActorSystem");

            Console.WriteLine("Creating SupervisorActor");
            var supervisor = KvActorSystem.ActorOf(Props.Create(() => new SupervisorActor(5, maxStorageKeys, maxKeyLength, maxValueLength)), "supervisor");

            Console.WriteLine("Ready");
            KvActorSystem.WhenTerminated.Wait();
        }
    }
}