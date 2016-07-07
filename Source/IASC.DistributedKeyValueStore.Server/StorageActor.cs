using Akka.Actor;
using IASC.DistributedKeyValueStore.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
                Console.WriteLine("StorageActor {0} - Inserting key '{1}'", Self, message.Key);
                Storage[message.Key] = message.Value;
                Sender.Tell("ok");
            });

            Receive<LookupMessage>(message =>
            {
                Console.WriteLine("StorageActor {0} - Looking up key '{1}'", Self, message.Key);
                // TODO: handle key not found
                Sender.Tell(Storage[message.Key]);
            });

            // TO DO: we need a parent actor that broadcasts the message and joins the responses
            Receive<SearchMessage>(message =>
            {
                Console.WriteLine("StorageActor {0} - Searching {1} '{2}'", Self, message.Comparison, message.ValueToCompare);

                const int invalidComparision = -2;

                var expectedComparisonResult = message.Comparison
                    .Select(c => c == 'e' ? 0
                                : c == 'g' ? 1
                                : c == 'l' ? -1
                                : invalidComparision)
                    .Where(x => x != invalidComparision)
                    .ToArray();

                var result = Storage
                    .Values
                    .Where(v => expectedComparisonResult.Contains(v.CompareTo(message.ValueToCompare)))
                    .ToList();

                Sender.Tell(result);
            });
        }
    }
}