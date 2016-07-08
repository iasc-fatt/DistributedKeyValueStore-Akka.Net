using Akka.Actor;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using IASC.DistributedKeyValueStore.Common;
using Ploeh.AutoFixture.Xunit2;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System;
using Ploeh.AutoFixture;

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

        [Theory]
        [InlineAutoData(/*greater:*/ false, /*equal:*/ false)]
        [InlineAutoData(/*greater:*/ false, /*equal:*/ true)]
        [InlineAutoData(/*greater:*/ true, /*equal:*/ false)]
        [InlineAutoData(/*greater:*/ true, /*equal:*/ true)]
        public void InsertAndSearchValues_ShouldFindValues(
            bool greater,
            bool equal,
            IFixture fixture,
            Dictionary<string, string> values)
        {
            // setup
            fixture.AddManyTo(values);

            var valueToCompare = values.Values
                .OrderBy(x => x)
                .ElementAt(values.Count() / 2);

            var comparison = (greater ? "g" : "l") + (equal ? "e" : "");

            var expectedValues = Filter(values, valueToCompare, comparison);
            expectedValues.Should().NotBeEmpty();

            values.ToList().ForEach(x =>
            {
                sut.Tell(new InsertValue(x.Key, x.Value));
                ExpectMsg<OpSucced>();
            });

            // exercise
            sut.Tell(new SearchValues(valueToCompare, comparison));

            // assert
            ExpectMsg<IEnumerable<string>>()
                .ShouldBeEquivalentTo(expectedValues);
        }

        [Theory, AutoData]
        public void SearchValues_EmptyStorage_ShouldReplyEmpty(
            string valueToCompare)
        {
            // exercise
            sut.Tell(new SearchValues(valueToCompare, "gle"));

            // assert
            ExpectMsg<IEnumerable<string>>()
                .Should().BeEmpty();
        }

        private List<string> Filter(Dictionary<string, string> values, string valueToCompare, string comparison)
        {
            const int invalidComparision = -2;
            var expectedComparisonResult = comparison
                .Select(c => c == 'e' ? 0
                            : c == 'g' ? 1
                            : c == 'l' ? -1
                            : invalidComparision)
                .Where(x => x != invalidComparision)
                .ToArray();
            return values.Values
                .Where(v => expectedComparisonResult.Contains(v.CompareTo(valueToCompare)))
                .ToList();
        }
    }
}