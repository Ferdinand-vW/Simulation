using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    public class EndSimulation : Event
    {


        public EndSimulation(double startTime)
        {

            this.StartTime = startTime;
        }

        public override void execute(SimulationState simState)
        {
            while (simState.EventQueue.HasEvent())
                simState.EventQueue.Next();
        }

        public override string ToString()
        {
            return "End " + StartTime;
        }
    }
}
