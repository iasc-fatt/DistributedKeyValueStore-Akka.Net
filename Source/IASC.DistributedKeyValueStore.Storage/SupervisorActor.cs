using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections.Generic;

namespace IASC.DistributedKeyValueStore.Server
{
    public class SupervisorActor : ReceiveActor
    {
        public SupervisorActor(long maxStorageKeys, string storageNumber)
        {
            var storageProps = Props.Create(() => new StorageActor(maxStorageKeys))
                .WithSupervisorStrategy(new OneForOneStrategy(-1, TimeSpan.FromSeconds(30), x => Directive.Restart));

            Context.ActorOf(storageProps, "s" + storageNumber);
        }
    }
}