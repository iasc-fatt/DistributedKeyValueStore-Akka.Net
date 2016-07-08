using Akka.Actor;
using Akka.Event;
using IASC.DistributedKeyValueStore.Common;
using System.Collections.Generic;
using System.Linq;

namespace IASC.DistributedKeyValueStore.Server
{
    public class StorageActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        private readonly Dictionary<string, string> Storage;

        public StorageActor()
        {
            Storage = new Dictionary<string, string>();

            Receive<InsertValue>(message =>
            {
                _log.Info("Inserting key '{1}'", Self, message.Key);

                Storage[message.Key] = message.Value;
                Sender.Tell(new OpSucced());
            });

            Receive<LookupValue>(message =>
            {
                _log.Info("Looking up key '{1}'", Self, message.Key);

                string value;
                if (Storage.TryGetValue(message.Key, out value))
                    Sender.Tell(new LookupResult(message.Key, value).Just());
                else
                    Sender.Tell(Maybe.Nothing<LookupResult>());
            });

            // TO DO: we need a parent actor that broadcasts the message and joins the responses
            Receive<SearchValues>(message =>
            {
                _log.Info("Searching {1} '{2}'", Self, message.Comparison, message.ValueToCompare);

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