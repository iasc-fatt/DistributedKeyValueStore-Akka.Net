﻿using Akka.Actor;
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
        private readonly ICanTell Server;

        public KvClient(string serverAddress)
        {
            KvActorSystem = ActorSystem.Create("KvActorSystem");
            Server = KvActorSystem.ActorSelection(serverAddress + "/user/server");
        }

        public async Task<Maybe<OpSucced>> Insert(string key, string value)
        {
            var msg = new InsertValue(key, value);
            return await Server.Ask<Maybe<OpSucced>>(msg);
        }

        public async Task Remove(string key)
        {
            var msg = new RemoveValue(key);
            await Server.Ask(msg);
        }

        public async Task<Maybe<LookupResult>> Lookup(string key)
        {
            var msg = new LookupValue(key);
            return await Server.Ask<Maybe<LookupResult>>(msg);
        }

        public async Task<IEnumerable<string>> Search(string valueToCompare, string comparison)
        {
            var msg = new SearchValues(valueToCompare, comparison);
            return await Server.Ask<IEnumerable<string>>(msg);
        }

        public async Task<IEnumerable<string>> Keys(string regex)
        {
            var msg = new SearchKeys(regex);
            return await Server.Ask<IEnumerable<string>>(msg);
        }

        public async Task<IEnumerable<string>> HealthCheck()
        {
            var msg = new HealthCheck();
            return await Server.Ask<IEnumerable<string>>(msg);
        }

        public async Task<Maybe<OpSucced>> KillActor(int hash)
        {
            var msg = new KillActor(hash);
            return await Server.Ask<Maybe<OpSucced>>(msg);
        }

        public void Dispose()
        {
            KvActorSystem.Terminate().Wait();
        }
    }
}