using Akka.Actor;
using NDesk.Options;
using System;
using System.Collections.Generic;
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

            var supervisorStrategy = new OneForOneStrategy(-1, TimeSpan.FromSeconds(30), x =>
            {
                return Directive.Restart;
            });

            var storageProps = Props.Create(() => new StorageActor(maxStorageKeys))
                .WithSupervisorStrategy(supervisorStrategy);

            var storage1 = KvActorSystem.ActorOf(storageProps, "s1");
            var storage2 = KvActorSystem.ActorOf(storageProps, "s2");

            var storages = new List<IActorRef>()
            {
                storage1,
                storage2
            };

            var storageRouter = KvActorSystem.ActorOf(Props.Create(() => new RouterActor(storages)), "router");
            var server = KvActorSystem.ActorOf(Props.Create(() => new CoordinatorActor(storageRouter, maxKeyLength, maxValueLength)), "server");

            Console.WriteLine("Ready");
            KvActorSystem.WhenTerminated.Wait();
        }
    }
}