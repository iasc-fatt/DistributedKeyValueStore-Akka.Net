using Akka.Actor;
using Akka.Routing;
using Akka.Event;
using IASC.DistributedKeyValueStore.Common;

namespace IASC.DistributedKeyValueStore.Server
{
    public class CoordinatorActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        private readonly IActorRef Storage;

		public CoordinatorActor(IActorRef storage, long maxKeyLength, long maxValueLength)
		{
            Storage = storage;

			Receive<InsertValue>(msg =>
			{
				_log.Info("Inserting key '{0}'", msg.Key);

				if (CheckMaxArgumentsLength(msg, maxKeyLength, maxValueLength))
                {
                    Sender.Tell(Maybe.Nothing<OpSucced>());
                    return;
                }

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

				var joiner = Context.ActorOf(Props.Create(() => new ListJoinerActor<SearchValues>(Storage, Sender)));
				joiner.Tell(msg);
			});

            Receive<SearchKeys>(msg =>
            {
                _log.Info("Searching keys");

                var joiner = Context.ActorOf(Props.Create(() => new ListJoinerActor<SearchKeys>(Storage, Sender)));
                joiner.Tell(msg);
            });

            Receive<HealthCheck>(msg =>
            {
                _log.Info("Requested HealthCheck");

                var joiner = Context.ActorOf(Props.Create(() => new StringJoinerActor<HealthCheck>(Storage, Sender)));
                joiner.Tell(msg);
            });
        }

		private bool CheckMaxArgumentsLength(InsertValue msg, long maxKeyLength, long maxValueLength)
		{
			if (msg.Key.Length > maxKeyLength)
			{
                _log.Error("Key length is superior to max allowed: '{0}'", maxKeyLength);
                return true;
            }

			if (msg.Value.Length > maxValueLength)
			{
                _log.Error("Key value is superior to max allowed: '{0}'", maxValueLength);
                return true;
            }

			return false;
		}

    }
}