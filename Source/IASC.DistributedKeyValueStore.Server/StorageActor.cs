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

            Receive<InsertValue>(msg =>
            {
                _log.Info("Inserting key '{1}'", msg.Key);

                Storage[msg.Key] = msg.Value;
                Sender.Tell(new OpSucced());
            });

            Receive<RemoveValue>(msg =>
            {
                _log.Info("Removing key '{1}'", msg.Key);

                Storage.Remove(msg.Key);
                Sender.Tell(new OpSucced());
            });

            Receive<LookupValue>(msg =>
            {
                _log.Info("Looking up key '{1}'", msg.Key);

                string value;
                if (Storage.TryGetValue(msg.Key, out value))
                    Sender.Tell(new LookupResult(msg.Key, value).Just());
                else
                    Sender.Tell(Maybe.Nothing<LookupResult>());
            });

            // TO DO: we need a parent actor that broadcasts the message and joins the responses
            Receive<SearchValues>(msg =>
            {
                _log.Info("Searching {1} '{2}'", msg.Comparison, msg.ValueToCompare);

                const int invalidComparision = -2;

                var expectedComparisonResult = msg.Comparison
                    .Select(c => c == 'e' ? 0
                                : c == 'g' ? 1
                                : c == 'l' ? -1
                                : invalidComparision)
                    .Where(x => x != invalidComparision)
                    .ToArray();

                var result = Storage
                    .Values
                    .Where(v => expectedComparisonResult.Contains(v.CompareTo(msg.ValueToCompare)))
                    .ToList();

                Sender.Tell(result);
            });
        }
    }
}