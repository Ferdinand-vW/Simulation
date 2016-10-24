using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator.Events
{
    //Class for events
    [Serializable]
    public class Event
    {
        public double StartTime { get; set; }
        public int _tramId { get; set; }
        public string Station { get; set; }
        public enum EventType { TramArrival, TramDeparture, TurnAround, Other};
        public EventType EType { get; set; }
        public SimulationState Snapshot { get; set; }


        public virtual void execute(SimulationState simState) { }
        public override String ToString()
        {
            return StartTime + "";
        }
    }
}
