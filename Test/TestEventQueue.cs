using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TramSimulator.Events;
using TramSimulator.States;
using TramSimulator;

namespace Test
{
    [TestClass]
    public class TestEventQueue
    {
        [TestMethod]
        public void TestPriority()
        {
            EventQueue eventQueue = new EventQueue();
            eventQueue.AddEvent(new PersonArrival(5.0, "alpha", Routes.Dir.ToCS));
            eventQueue.AddEvent(new PersonArrival(10.0, "beta", Routes.Dir.ToCS));
            eventQueue.AddEvent(new PersonArrival(3.0, "gamma", Routes.Dir.ToCS));
            eventQueue.AddEvent(new PersonArrival(6.0, "delta", Routes.Dir.ToCS));

            Event e1 = eventQueue.Next();
            Event e2 = eventQueue.Next();
            Event e3 = eventQueue.Next();
            Event e4 = eventQueue.Next();

            Assert.AreEqual(e1.StartTime, 3.0);
            Assert.AreEqual(e2.StartTime, 5.0);
            Assert.AreEqual(e3.StartTime, 6.0);
            Assert.AreEqual(e4.StartTime, 10.0);
        }
    }
}
