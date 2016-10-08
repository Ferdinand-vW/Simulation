using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    //event voor als een persoon aan komt op een bepaald station
    //creert ook de event voor de volgende 
    public class PersonArrival : Event
    {
        string _stationName;
        bool typeA;
        public PersonArrival(double startTime, string stationName, bool typeA)
        {
            this.StartTime = startTime;
            this.typeA = typeA;
            this._stationName = stationName;
        }
        public override void execute(SimulationState simState)
        {
            Station station = simState.Stations[_stationName];
            if (typeA) {
                station.WaitingPersonsA.Enqueue(new Person(StartTime));
            }
            else {
                station.WaitingPersonsB.Enqueue(new Person(StartTime));
            }


            if (simState.Rates.nonZeroPercentage(_stationName, typeA, StartTime))
            {
                double newTime = StartTime + simState.Rates.PersonArrivalRate(_stationName, typeA, StartTime);
                simState.EventQueue.AddEvent(new PersonArrival(newTime, _stationName, typeA));
            }
            else {
                double newTime = StartTime + ((15 * 60) - (StartTime % (15 * 60)));
                simState.EventQueue.AddEvent(new ZeroPersonArrival(newTime, _stationName, typeA));
            }
        }

        public override string ToString()
        {
            return "Person Arrived " + StartTime + " at " + _stationName;
        }
    }
}
