using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using IASC.DistributedKeyValueStore.Common;

namespace IASC.DistributedKeyValueStore.Server
{
    public class CoordinatorActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        private readonly IActorRef Storage;

        public CoordinatorActor()
        {
            var props = Props.Create<StorageActor>().WithRouter(FromConfig.Instance);
            Storage = Context.ActorOf(props, "storage");

            Receive<InsertValue>(msg =>
            {
                _log.Info("Inserting key '{0}'", msg.Key);

                Storage.Forward(new ConsistentHashableEnvelope(msg, msg.Key));
            });

            Receive<RemoveValue>(msg =>
            {
                _log.Info("Removing key '{0}'", msg.Key);

                Storage.Forward(new ConsistentHashableEnvelope(msg, msg.Key));
            });

            Receive<LookupValue>(msg =>
            {
                _log.Info("Looking up key '{0}'", msg.Key);

                Storage.Forward(new ConsistentHashableEnvelope(msg, msg.Key));
            });

            Receive<SearchValues>(msg =>
            {
                _log.Info("Searching {0} '{1}'", msg.Comparison, msg.ValueToCompare);

                var joiner = Context.ActorOf(Props.Create(() => new SearchValuesActor(Storage, Sender)));
                joiner.Tell(msg);
            });
        }
    }
}