using Akka.Actor;
using IASC.DistributedKeyValueStore.Common;
using System.Collections.Generic;
using System.Linq;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class SearchKeysActor : JoinerActor<SearchKeys, IEnumerable<string>>
    {
        public SearchKeysActor(IActorRef storage, IActorRef listener)
            : base(
                router: storage,
                nrOfRoutees: 5, //TO DO: read from config
                joiner: (a, b) => a.Concat(b).ToList(),
                initialValue: new string[0],
                listener: listener)
        { }
    }
}