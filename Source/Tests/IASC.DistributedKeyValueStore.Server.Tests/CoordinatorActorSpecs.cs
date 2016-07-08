using Akka.Actor;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using IASC.DistributedKeyValueStore.Common;
using System;
using Xunit;

namespace IASC.DistributedKeyValueStore.Server.Tests
{
    public class CoordinatorActorSpecs : TestKit
    {
        [Fact]
        public void InsertMessage_ShouldReply_Ok()
        {
            var value = Guid.NewGuid().ToString();
            var coordinator = Sys.ActorOf(Props.Create(() => new StorageActor()));

            coordinator.Tell(new InsertMessage("key", value));

            ExpectMsg<string>().Should().Be("ok");
        }
    }
}