using Akka.Actor;
using System;
using System.Collections.Generic;

namespace IASC.DistributedKeyValueStore.Server
{
    public class SupervisorActor : ReceiveActor
    {
        public SupervisorActor(long storagesAmount, long maxStorageKeys, long maxKeyLength, long maxValueLength)
        {
            var storageProps = Props.Create(() => new StorageActor(maxStorageKeys))
                .WithSupervisorStrategy(new OneForOneStrategy(-1, TimeSpan.FromSeconds(30), x => Directive.Restart));

            var storages = new List<IActorRef>();
            for (int i = 1; i <= storagesAmount; i++)
            {
                storages.Add(Context.ActorOf(storageProps, "s" + i));
            }

            var routerProps = Props.Create(() => new RouterActor(storages))
                .WithSupervisorStrategy(new AllForOneStrategy(-1, TimeSpan.FromSeconds(30), x => Directive.Restart));

            var router = Context.ActorOf(routerProps, "router");

            var server = Context.ActorOf(Props.Create(() => new CoordinatorActor(router, maxKeyLength, maxValueLength)), "server");
        }
    }
}