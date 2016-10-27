using TramSimulator.States;
using TramSimulator.Sim;

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
            this.EType = EventType.Other;
        }
        public override void Execute(SimulationState simState)
        {
            //If there are people arriving this 15 minute block
            if (!simState.Rates.NonZeroPercentage(StartTime, _stationName, _direction))
            {
                double newTime = StartTime + simState.Rates.PersonArrivalRate(_stationName, _direction, StartTime);
                simState.EventQueue.AddEvent(new PersonArrival(newTime, _stationName, _direction));
            }
            else //Otherwise set a new zeropersonarrival event for over 15 minutes
            {
                simState.EventQueue.AddEvent(new ZeroPersonArrival(StartTime + (Constants.SECONDS_IN_MINUTE * 15), _stationName, _direction));
            }
        }

        public override string ToString()
        {
            return "ZeroPerson " + StartTime + " at " + _stationName;
        }

    }
}
