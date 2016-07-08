using Akka.Actor;
using Akka.Routing;
using NDesk.Options;
using System;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class Program
    {
        private static ActorSystem KvActorSystem;

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
            var props = Props.Create<StorageActor>().WithRouter(FromConfig.Instance);
            var storage = KvActorSystem.ActorOf(props, "storage");

            Console.WriteLine("Ready");
            KvActorSystem.WhenTerminated.Wait();
        }
    }
}