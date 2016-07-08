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
        private readonly IActorRef sut;

        public CoordinatorActorSpecs()
            : base(@"
              akka {
                actor {
                  deployment {
                    /server/storage {
                      router = consistent-hashing-pool
                      nr-of-instances = 5
                      virtual-nodes-factor = 10
                    }
                  }
                }
              }")
        {
            sut = Sys.ActorOf(Props.Create(() => new CoordinatorActor()), "server");
        }

        [Theory, AutoData]
        public void InsertAndLookupValue_ShouldFindValue(string key, string value)
        {
            sut.Tell(new InsertValue(key, value));
            ExpectMsg<OpSucced>();

            sut.Tell(new LookupValue(key));

            ExpectMsg<Maybe<LookupResult>>()
                .ShouldBeEquivalentTo(new LookupResult(key, value).Just());
        }

        [Theory, AutoData]
        public void RemoveUnexistingKey_ShouldSucced(string key)
        {
            sut.Tell(new RemoveValue(key));

            ExpectMsg<OpSucced>();
        }

        [Theory, AutoData]
        public void LookupUnexistingKey_ShouldReplyEmpty(string key)
        {
            sut.Tell(new LookupValue(key));

            ExpectMsg<Maybe<LookupResult>>()
                .Should().BeEmpty();
        }

        [Theory, AutoData]
        public void InsertRemoveAndLookupValue_ShouldReplyEmpty(string key, string value)
        {
            sut.Tell(new InsertValue(key, value));
            ExpectMsg<OpSucced>();
            sut.Tell(new RemoveValue(key));
            ExpectMsg<OpSucced>();

            sut.Tell(new LookupValue(key));

            ExpectMsg<Maybe<LookupResult>>()
                .Should().BeEmpty();
        }
    }
}