using Akka.Actor;
using Akka.TestKit.Xunit2;
using IASC.DistributedKeyValueStore.Common;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace IASC.DistributedKeyValueStore.Server.Tests
{
    public class CoordinatorActorSpecs : TestKit
    {
        [Theory, AutoData]
        public void InsertMessage_ShouldReplyOk(string key, string value)
        {
            var coordinator = Sys.ActorOf(Props.Create(() => new StorageActor()));

            coordinator.Tell(new InsertValue(key, value));

            ExpectMsg<OpSucced>();
        }
    }
}