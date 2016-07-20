using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IASC.DistributedKeyValueStore.Server
{
    class RouterActor : ReceiveActor
    {
        public IEnumerable<IActorRef> Routees;
        public ConsistentHash<IActorRef> Circle;
        public Dictionary<string, IActorRef> PathDict;

        public RouterActor(IEnumerable<IActorRef> routees)
        {
            Routees = routees;

            Receive<ConsistentHashableEnvelope>((envelope) =>
            {
                var msg = envelope.Message;
                var routee = GetRoutee((string) envelope.HashKey);

                routee.Forward(msg);
            });

            Receive<Broadcast>((envelope) =>
            {
                var msg = envelope.Message;

                Routees.ToList().ForEach((r) =>
                {
                    r.Forward(msg);
                });
            });

            Receive<PathSelectorEnvelope>((envelope) =>
            {
                var msg = envelope.Message;
                var path = envelope.Path;
                IActorRef routee;

                if (PathDict.TryGetValue(path, out routee))
                    routee.Forward(msg);
            });

            Receive<HasRouteeByPath>(msg => 
            {

            });
        }

        public bool ContainsRouteeByPath(string path)
        {
            return PathDict.ContainsKey(path);
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
