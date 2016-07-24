using Akka.Actor;
using System;
using System.Configuration;

namespace IASC.DistributedKeyValueStore.Server
{
    internal class Program
    {
        private static ActorSystem KvActorSystem;
        private static readonly long maxValueLength = 5;
        private static readonly long storagesAmount = 5;

        private static void Main(string[] args)
        {
            Console.WriteLine("Creating KvActorSystem");
            KvActorSystem = ActorSystem.Create("KvActorSystem");

            Console.WriteLine("Creating SupervisorActor");
            var supervisor = KvActorSystem.ActorOf(Props.Create(() => new SupervisorActor(storagesAmount, maxValueLength)), "supervisor");

            Console.WriteLine("Ready");
            KvActorSystem.WhenTerminated.Wait();
        }
    }
}