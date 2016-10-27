using System;

using TramSimulator.Sim;

namespace TramSimulator.Events
{
    //Class for events
    public class Event
    {
        public double StartTime { get; set; }
        public int _tramId { get; set; }
        public string Station { get; set; }
        public enum EventType { TramArrival, TramDeparture, TurnAround, Other};
        public EventType EType { get; set; }
        public SimulationState Snapshot { get; set; }


        public virtual void Execute(SimulationState simState) { }
        public override String ToString()
        {
            return StartTime + "";
        }
    }
}
