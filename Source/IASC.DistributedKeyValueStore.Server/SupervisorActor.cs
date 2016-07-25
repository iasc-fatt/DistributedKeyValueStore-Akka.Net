using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IASC.DistributedKeyValueStore.Server
{
    public class SupervisorActor : ReceiveActor
    {
        public SupervisorActor(string storageAddress, long storagesAmount, long maxKeyLength, long maxValueLength)
        {
            var storagesPaths = new List<string>();

            for (int i = 1; i <= storagesAmount; i++)
            {
                storagesPaths.Add(storageAddress + "/user/storages/s" + i);
            }

            var routerProps = Props.Create(() => new RouterActor(storagesPaths))
                .WithSupervisorStrategy(new OneForOneStrategy(-1, TimeSpan.FromSeconds(30), x => Directive.Restart));

            var router = Context.ActorOf(routerProps, "router");

            var serverProps = Props.Create(() => new CoordinatorActor(router, maxKeyLength, maxValueLength))
                .WithRouter(new SmallestMailboxPool(5));
            var server = Context.ActorOf(serverProps, "server");
        }
    }
}