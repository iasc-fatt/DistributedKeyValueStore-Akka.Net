using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class RouterActor : ReceiveActor
    {
        public IEnumerable<string> Routees;
        public ConsistentHash<string> Circle;
        public Dictionary<string, ActorSelection> PathDict;

        public RouterActor(IEnumerable<string> routees)
        {
            Routees = routees;
            ProcessRoutees();

            Receive<ConsistentHashableEnvelope>((envelope) =>
            {
                var msg = envelope.Message;
                var path = GetRouteePath((string)envelope.HashKey);

                var routee = PathDict[path];
                routee.Tell(msg, Sender);
            });

            Receive<Broadcast>((envelope) =>
            {
                PathDict.ToList().ForEach((a) =>
                {
                    a.Value.Tell(envelope.Message, Sender);
                });
            });

            Receive<PathSelectorEnvelope>((envelope) =>
            {
                if (PathDict.ContainsKey(envelope.Path))
                    PathDict[envelope.Path].Tell(envelope.Message, Sender);
            });

            Receive<HasRouteeByPath>(msg =>
            {
                Sender.Tell(PathDict.ContainsKey(msg.Path));
            });

            Receive<RouteesCount>(msg =>
            {
                Sender.Tell(Routees.Count(), Sender);
            });
        }

        private void ProcessRoutees()
        {
            Circle = ConsistentHash.Create<string>(Routees.Select(p => PathWithoutAddress(p)), 20);
            PathDict = new Dictionary<string, ActorSelection>();
            Routees.ToList().ForEach((path) =>
            {
                var actorSelector = Context.ActorSelection(path);
                
                PathDict.Add(PathWithoutAddress(path), actorSelector);
                Console.WriteLine(PathWithoutAddress(path));
            });
        }

        private string PathWithoutAddress(string fullPath)
        {
            var index = fullPath.IndexOf("/user/");
            return fullPath.Substring(index);
        }

        private string GetRouteePath(string hashKey)
        {
            return Circle.NodeFor(hashKey);
        }
    }
}