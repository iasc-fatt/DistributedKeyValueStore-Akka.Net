using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections.Generic;

namespace IASC.DistributedKeyValueStore.Server
{
    public class SupervisorActor : ReceiveActor
    {
        public SupervisorActor(long storagesAmount, long maxStorageKeys)
        {
            var storageProps = Props.Create(() => new StorageActor(maxStorageKeys))
                .WithSupervisorStrategy(new OneForOneStrategy(-1, TimeSpan.FromSeconds(30), x => Directive.Restart));

            var storages = new List<IActorRef>();
            for (int i = 1; i <= storagesAmount; i++)
            {
                storages.Add(Context.ActorOf(storageProps, "s" + i));
            }
        }
    }
}