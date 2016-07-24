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
            var storages = new List<IActorRef>();
            var tasks = new List<Task>();

            for (int i = 1; i <= storagesAmount; i++)
            {
                tasks.Add(GetStorage(storageAddress, i).ContinueWith(storage => {
                    storages.Add(storage.Result);
                }));
            }

            Task.WaitAll(tasks.ToArray());

            var routerProps = Props.Create(() => new RouterActor(storages))
                .WithSupervisorStrategy(new OneForOneStrategy(-1, TimeSpan.FromSeconds(30), x => Directive.Restart));

            var router = Context.ActorOf(routerProps, "router");

            var serverProps = Props.Create(() => new CoordinatorActor(router, maxKeyLength, maxValueLength))
                .WithRouter(new SmallestMailboxPool(5));
            var server = Context.ActorOf(serverProps, "server");
        }

        private Task<IActorRef> GetStorage(string storageAddress, int i)
        {
            return Context.ActorSelection(storageAddress + "/user/supervisor/s" + i)
                .ResolveOne(TimeSpan.FromSeconds(30));
        }
    }
}