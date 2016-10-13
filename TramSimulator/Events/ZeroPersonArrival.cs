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
        Routes.Dir _direction;
        public ZeroPersonArrival(double startTime, string stationName, Routes.Dir dir)
        {
            this.StartTime = startTime;
            this._direction = dir;
            this._stationName = stationName;
        }
        public override void execute(SimulationState simState)
        {
            if (!simState.Rates.nonZeroPercentage(StartTime, _stationName, _direction))
            {
                double newTime = StartTime + simState.Rates.PersonArrivalRate(_stationName, _direction, StartTime);
                simState.EventQueue.AddEvent(new PersonArrival(newTime, _stationName, _direction));
            }
            else
            {
                simState.EventQueue.AddEvent(new ZeroPersonArrival(StartTime + (60 * 15), _stationName, _direction));
            }
        }

        public override string ToString()
        {
            return "ZeroPerson " + StartTime + " at " + _stationName;
        }

    }
}
