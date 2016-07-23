using Akka.Actor;
using IASC.DistributedKeyValueStore.Common;
using System.Collections.Generic;
using System.Linq;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class StringJoinerActor<TMessage> : JoinerActor<TMessage, string, IEnumerable<string>>
    {
        public StringJoinerActor(IActorRef storage, IActorRef listener)
            : base(
                router: storage,
                joiner: (a, b) => a.Concat(new[] { b }).ToList(),
                initialValue: new string[] { },
                listener: listener)
        { }
    }
}