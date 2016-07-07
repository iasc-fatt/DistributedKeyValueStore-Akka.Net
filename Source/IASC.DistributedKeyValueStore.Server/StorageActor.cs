using Akka.Actor;
using IASC.DistributedKeyValueStore.Common;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class StorageActor : ReceiveActor
    {
        private readonly Dictionary<string, string> Storage;

        public StorageActor()
        {
            Storage = new Dictionary<string, string>();

            Receive<InsertMessage>(message =>
            {
                Storage[message.Key] = message.Value;
                Sender.Tell("ok");
            });

            Receive<LookupMessage>(message =>
            {
                // TODO: handle key not found
                Sender.Tell(Storage[message.Key]);
            });
        }
    }
}