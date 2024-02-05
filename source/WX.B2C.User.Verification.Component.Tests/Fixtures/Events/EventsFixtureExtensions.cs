using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WX.Messaging.Core;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures.Events
{
    internal static class EventsFixtureExtensions
    {
        public static TEvent ShouldExist<TEvent>(this EventsFixture fixture, Guid correlationId) where TEvent : Event
        {
            return fixture.ShouldExist<TEvent>(e => e.CorrelationId == correlationId);
        }

        public static TEvent ShouldExistSingle<TEvent>(this EventsFixture fixture, Guid correlationId, object targetObject = null) where TEvent : Event
        {
            return fixture.ShouldExistSingle<TEvent>(e => e.CorrelationId == correlationId);
        }

        public static void ShouldNotExist<TEvent>(this EventsFixture fixture, Guid correlationId) where TEvent : Event
        {
            fixture.ShouldNotExist<TEvent>(e => e.CorrelationId == correlationId);
        }
    }
}
