using Akka.Actor;
using IASC.DistributedKeyValueStore.Common;
using System.Collections.Generic;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class ActorRefJoinerActor<TMessage> : JoinerActor<TMessage, StorageIdentity, Dictionary<int, IActorRef>>
    {
        public ActorRefJoinerActor(IActorRef storage, IActorRef listener)
            : base(
                router: storage,
                joiner: (a, b) =>
                {
                    a.Add(b.Hash, b.Actor);
                    return a;
                },
                initialValue: new Dictionary<int, IActorRef>(),
                listener: listener)
        { }
    }
}