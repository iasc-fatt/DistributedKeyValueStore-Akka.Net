using Akka.Actor;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using IASC.DistributedKeyValueStore.Common;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace IASC.DistributedKeyValueStore.Server.Tests
{
    public class CoordinatorActorSpecs : TestKit
    {
        [Theory, AutoData]
        public void InsertAndLookupValue_ShouldFindValue(string key, string value)
        {
            var coordinator = Sys.ActorOf(Props.Create(() => new StorageActor()));

            coordinator.Tell(new InsertValue(key, value));

            ExpectMsg<OpSucced>();

            coordinator.Tell(new LookupValue(key));

            ExpectMsg<Maybe<LookupResult>>()
                .ShouldBeEquivalentTo(new LookupResult(key, value).Just());
        }

        [Theory, AutoData]
        public void LookupUnexistingKey_ShouldReplyEmpty(string key)
        {
            var coordinator = Sys.ActorOf(Props.Create(() => new StorageActor()));

            coordinator.Tell(new LookupValue(key));

            ExpectMsg<Maybe<LookupResult>>()
                .Should().BeEmpty();
        }
    }
}