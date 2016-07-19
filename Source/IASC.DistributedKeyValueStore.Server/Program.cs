using Akka.Actor;
using Akka.Routing;
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

            Console.WriteLine("Creating actor supervisory hierarchy");

            var supervisorStrategy = new OneForOneStrategy(-1, TimeSpan.FromSeconds(30), x => Directive.Restart);
            var storageProps = Props.Create(() => new StorageActor(maxStorageKeys))
                .WithRouter(FromConfig.Instance)
                .WithSupervisorStrategy(supervisorStrategy);

            var storage = KvActorSystem.ActorOf(storageProps, "storage");
            var server = KvActorSystem.ActorOf(Props.Create(() => new CoordinatorActor(storage, maxKeyLength, maxValueLength)), "server");

            Console.WriteLine("Ready");
            KvActorSystem.WhenTerminated.Wait();
        }
    }
}