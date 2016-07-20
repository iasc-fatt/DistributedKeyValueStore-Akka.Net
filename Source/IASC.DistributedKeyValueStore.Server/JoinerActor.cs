using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using System;

namespace IASC.DistributedKeyValueStore.Server
{
    /// <summary>
    /// A short-lived actor that sends a message to all routees of a router and joins the responses
    /// </summary>
    /// <typeparam name="TMessage">The type of the message that must be broadcasted</typeparam>
    /// <typeparam name="TResponse">The type of the expected response of each routee</typeparam>
    public class JoinerActor<TMessage, TResponse, TContainer> : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        private bool _processing = false;
        private TContainer _joinedResponse;
        private int _nrOfReceivedResponses = 0;

        /// <param name="router">Router actor</param>
        /// <param name="nrOfRoutees">Number of routees</param>
        /// <param name="joiner">A function that joins two responses</param>
        /// <param name="initialValue">The initial value of the joined response</param>
        /// <param name="listener">Listener actor</param>
        public JoinerActor(
            IActorRef router,
            int nrOfRoutees,
            Func<TContainer, TResponse, TContainer> joiner,
            TContainer initialValue,
            IActorRef listener)
        {
            this._joinedResponse = initialValue;

            Receive<TMessage>(msg =>
            {
                _log.Info("Received message '{0}'", msg);

                if (_processing)
                {
                    _log.Info("Message discarded '{0}'", msg);
                    return;
                }

                nrOfRoutees = router.Ask<int>(new RouteesCount()).Result;

                router.Tell(new Broadcast(msg));
                _processing = true;

                // TO DO: handle timeout
            });

            Receive<TResponse>(msg =>
            {
                _nrOfReceivedResponses++;
                _log.Info("Joining response nr {0}", _nrOfReceivedResponses);

                _joinedResponse = joiner(_joinedResponse, msg);

                if (_nrOfReceivedResponses == nrOfRoutees)
                {
                    listener.Tell(_joinedResponse);
                    Self.Tell(PoisonPill.Instance);
                }
            });
        }
    }
}