using Akka.Actor;

namespace IASC.DistributedKeyValueStore.Common
{
    public class StorageIdentity
    {
        public int Hash { get; }

        public IActorRef Actor { get; }

        public StorageIdentity(int hash, IActorRef actor)
        {
            Hash = hash;
            Actor = actor;
        }
    }
}