using Akka.Actor;
using Akka.Routing;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IASC.DistributedKeyValueStore.Server
{
    class Program
    {
        private static ActorSystem KvActorSystem;

        static void Main(string[] args)
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

            //SmokeTest(storage);

            Console.WriteLine("Ready");
            KvActorSystem.WhenTerminated.Wait();
        }

        private static void SmokeTest(IActorRef storage)
        {
            var key = "asd";
            var msg = new Common.InsertMessage(key, "value");
            var r = storage.Ask(new ConsistentHashableEnvelope(msg, msg.Key)).Result;
            //var r = storage.Ask(msg).Result;

            var msg2 = new Common.LookupMessage(key);
            var r2 = storage.Ask<string>(new ConsistentHashableEnvelope(msg2, msg2.Key)).Result;
            //var r2 = storage.Ask<string>(msg2).Result;

            Console.WriteLine(r2);
        }
    }
}
