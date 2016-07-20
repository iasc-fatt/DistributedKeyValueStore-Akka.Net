using Akka.Actor;
using Akka.Routing;
using System.Collections.Generic;
using System.Linq;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class RouterActor : ReceiveActor
    {
        public IEnumerable<IActorRef> Routees;
        public ConsistentHash<IActorRef> Circle;
        public Dictionary<string, IActorRef> PathDict;

        public RouterActor(IEnumerable<IActorRef> routees)
        {
            Routees = routees;
            ProcessRoutees();

            Receive<ConsistentHashableEnvelope>((envelope) =>
            {
                var msg = envelope.Message;
                var routee = GetRoutee((string)envelope.HashKey);

                routee.Forward(msg);
            });

            Receive<Broadcast>((envelope) =>
            {
                Routees.ToList().ForEach((r) =>
                {
                    r.Forward(envelope.Message);
                });
            });

            Receive<PathSelectorEnvelope>((envelope) =>
            {
                if (PathDict.ContainsKey(envelope.Path))
                    PathDict[envelope.Path].Forward(envelope.Message);
            });

            Receive<HasRouteeByPath>(msg =>
            {
                Sender.Tell(PathDict.ContainsKey(msg.Path));
            });

            Receive<RouteesCount>(msg =>
            {
                Sender.Tell(Routees.Count());
            });
        }

        private void ProcessRoutees()
        {
            Circle = ConsistentHash.Create<IActorRef>(Routees, 20);
            PathDict = new Dictionary<string, IActorRef>();

            Routees.ToList().ForEach((r) =>
            {
                PathDict.Add(r.Path.ToStringWithoutAddress(), r);
            });
        }

        private IActorRef GetRoutee(string hashKey)
        {
            return Circle.NodeFor(hashKey);
        }
    }
}