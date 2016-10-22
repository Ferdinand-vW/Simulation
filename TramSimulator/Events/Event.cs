using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator.Events
{
    //Class for events
    public class Event
    {
        public double StartTime { get; set; }
        public int _tramId { get; set; }


        public virtual void execute(SimulationState simState) { }
        public override String ToString()
        {
            return StartTime + "";
        }
    }
}
