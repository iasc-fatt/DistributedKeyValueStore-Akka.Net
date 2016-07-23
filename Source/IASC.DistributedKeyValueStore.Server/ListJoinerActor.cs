using Akka.Actor;
using IASC.DistributedKeyValueStore.Common;
using System.Collections.Generic;
using System.Linq;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class ListJoinerActor<TMessage> : JoinerActor<TMessage, IEnumerable<string>, IEnumerable<string>>
    {
        public ListJoinerActor(IActorRef storage, IActorRef listener)
            : base(
                router: storage,
                joiner: (a, b) => a.Concat(b).ToList(),
                initialValue: new string[0],
                listener: listener)
        { }
    }
}