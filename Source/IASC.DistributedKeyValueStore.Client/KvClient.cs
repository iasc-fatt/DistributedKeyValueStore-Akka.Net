using Akka.Actor;
using Akka.Routing;
using IASC.DistributedKeyValueStore.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IASC.DistributedKeyValueStore.Client
{
    public class KvClient : IDisposable
    {
        private readonly ActorSystem KvActorSystem;
        private readonly ICanTell Storage;

        public KvClient(string serverAddress)
        {
            KvActorSystem = ActorSystem.Create("KvActorSystem");
            Storage = KvActorSystem.ActorSelection(serverAddress + "/user/storage");
        }

        public async Task Insert(string key, string value)
        {
            var msg = new InsertValue(key, value);
            await Storage.Ask(new ConsistentHashableEnvelope(msg, msg.Key));
        }

        public async Task Remove(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Lookup(string key)
        {
            var msg = new LookupValue(key);
            return await Storage.Ask<string>(new ConsistentHashableEnvelope(msg, msg.Key));
        }

        public async Task<IEnumerable<string>> Search(string valueToCompare, string comparison)
        {
            var msg = new SearchValues(valueToCompare, comparison);
            return await Storage.Ask<IEnumerable<string>>(new Broadcast(msg));
        }

        public void Dispose()
        {
            KvActorSystem.Terminate().Wait();
        }
    }
}