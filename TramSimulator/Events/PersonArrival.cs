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
        Routes.Dir _direction;

        public PersonArrival(double startTime, string stationName, Routes.Dir dir)
        {
            this.StartTime = startTime;
            this._direction = dir;
            this._stationName = stationName;
        }
        public override void execute(SimulationState simState)
        {
            Station station = simState.Stations[_stationName];
            var persons = simState.Persons;
            simState.counter++;
            simState.sw.WriteLine("persons: " + simState.counter);
            if(_direction == Routes.Dir.ToCS)
            {
                int pid = persons.Count;
                Person p = new Person(pid, StartTime);
                p.QueueLengthAtArrival = station.WaitingPersonsToCS.Count;
                persons.Add(pid,p);
                station.WaitingPersonsToCS.Enqueue(pid);
            }
            else
            {
                int pid = persons.Count;
                Person p = new Person(pid, StartTime);
                p.QueueLengthAtArrival = station.WaitingPersonsToPR.Count;
                persons.Add(pid, p);
                station.WaitingPersonsToPR.Enqueue(pid);
            }

            if (!simState.Rates.nonZeroPercentage(StartTime, _stationName, _direction))
            {
                double newTime = StartTime + simState.Rates.PersonArrivalRate(_stationName, _direction, StartTime);
                simState.EventQueue.AddEvent(new PersonArrival(newTime, _stationName, _direction));
            }
            else
            {
                double newTime = StartTime + ((15 * 60) - (StartTime % (15 * 60)));
                simState.EventQueue.AddEvent(new ZeroPersonArrival(newTime, _stationName, _direction));
            }
        }

        public override string ToString()
        {
            return "Person Arrived " + StartTime + " at " + _stationName;
        }
    }
}
