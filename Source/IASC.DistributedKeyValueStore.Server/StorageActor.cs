using Akka.Actor;
using Akka.Event;
using IASC.DistributedKeyValueStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IASC.DistributedKeyValueStore.Server
{
    public class StorageActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        private readonly Dictionary<string, string> Storage;

        public StorageActor(long maxValuesAcepted)
        {
            _log.Info("Initializing storage actor with hash '{0}'", this.GetHashCode());
            Storage = new Dictionary<string, string>();

            Receive<InsertValue>(msg =>
            {
                _log.Info("Inserting key '{0}'", msg.Key);

                if (Storage.Count >= maxValuesAcepted)
                {
                    _log.Info("Max memory. Not storing key '{0}'", msg.Key);
                    Sender.Tell(Maybe.Nothing<OpSucced>());
                    return;
                }

                Storage[msg.Key] = msg.Value;
                Sender.Tell(new OpSucced().Just());
            });

            Receive<RemoveValue>(msg =>
            {
                _log.Info("Removing key '{0}'", msg.Key);

                Storage.Remove(msg.Key);
                Sender.Tell(new OpSucced());
            });

            Receive<LookupValue>(msg =>
            {
                _log.Info("Looking up key '{0}'", msg.Key);

                string value;
                if (Storage.TryGetValue(msg.Key, out value))
                    Sender.Tell(new LookupResult(msg.Key, value).Just());
                else
                    Sender.Tell(Maybe.Nothing<LookupResult>());
            });

            Receive<SearchValues>(msg =>
            {
                _log.Info("Searching {0} '{1}'", msg.Comparison, msg.ValueToCompare);

                const int invalidComparision = -2;

                // TO DO: encapsulate logic in another object
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

            Receive<SearchKeys>(msg =>
            {
                var regex = new Regex(msg.Regex);

                var result = Storage
                    .Keys
                    .Where(v => regex.IsMatch(v))
                    .ToList();

                Sender.Tell(result);
            });

            Receive<HealthCheck>(msg =>
            {
                var keys = string.Join(", ", Storage.Keys);
                var result = string.Format("Actor {0} {1} ({2})", Self.Path.Name, this.GetHashCode(), keys);
                Sender.Tell(result);
            });

            Receive<StorageHash>(msg =>
            {
                Sender.Tell(new StorageIdentity(this.GetHashCode(), Self));
            });

            Receive<KillActor>(msg =>
            {
                _log.Info("Mmm me muero, con hash '{0}'", this.GetHashCode());
                Sender.Tell(new OpSucced().Just());
                throw new Exception();
                return true;
            });
        }
    }
}