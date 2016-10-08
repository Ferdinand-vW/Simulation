using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TramSimulator.States;

namespace TramSimulator.Events
{

    class ZeroPersonArrival : Event
    {
        string _stationName;
        bool typeA;
        public ZeroPersonArrival(double startTime, string stationName, bool typeA)
        {
            this.StartTime = startTime;
            this.typeA = typeA;
            this._stationName = stationName;
        }
        public override void execute(SimulationState simState)
        {
            if (simState.Rates.nonZeroPercentage( _stationName, typeA, StartTime))
            {
                double newTime = StartTime + simState.Rates.PersonArrivalRate(_stationName, typeA, StartTime);
                Console.WriteLine();
                Console.WriteLine("##############newTime = {0}", newTime);
                simState.EventQueue.AddEvent(new PersonArrival(newTime, _stationName, typeA));
            }
            else
                simState.EventQueue.AddEvent(new ZeroPersonArrival(StartTime + (60 * 15), _stationName, typeA));
        }
    }
}
