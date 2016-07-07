using Akka.Actor;
using Akka.Routing;
using IASC.DistributedKeyValueStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var msg = new InsertMessage(key, value);
            await Storage.Ask(new ConsistentHashableEnvelope(msg, msg.Key));
        }

        public async Task Remove(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Lookup(string key)
        {
            var msg = new LookupMessage(key);
            return await Storage.Ask<string>(new ConsistentHashableEnvelope(msg, msg.Key));
        }

        public async Task<IEnumerable<string>> Search(Func<string, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            KvActorSystem.Terminate().Wait();
        }
    }
}
