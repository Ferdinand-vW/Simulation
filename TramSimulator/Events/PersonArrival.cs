using TramSimulator.States;
using TramSimulator.Sim;

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
            this.EType = EventType.Other;
        }
        public override void Execute(SimulationState simState)
        {
            Station station = simState.Stations[_stationName];
            var persons = simState.Persons;
            //Create a person object and insert it into a queue
            if(_direction == Routes.Dir.ToCS)
            {
                int pid = persons.Count;
                Person p = new Person(pid, StartTime);
                p.QueueLengthAtArrival = station.WaitingPersonsToCS.Count;
                persons.Add(pid,p);
                station.WaitingPersonsToCS.Enqueue(pid);
                //Set maxqueue statistic
                if (station.WaitingPersonsToCS.Count > station.MaxQueueLengthCS)
                { station.MaxQueueLengthCS = station.WaitingPersonsToCS.Count; }
            }
            else
            {
                int pid = persons.Count;
                Person p = new Person(pid, StartTime);
                p.QueueLengthAtArrival = station.WaitingPersonsToPR.Count;
                persons.Add(pid, p);
                station.WaitingPersonsToPR.Enqueue(pid);
                if (station.WaitingPersonsToPR.Count > station.MaxQueueLengthPR)
                { station.MaxQueueLengthPR = station.WaitingPersonsToPR.Count; }
            }

            //Create the next personarrival event if it exists within the same block
            if (!simState.Rates.NonZeroPercentage(StartTime, _stationName, _direction))
            {
                double newTime = StartTime + simState.Rates.PersonArrivalRate(_stationName, _direction, StartTime);
                simState.EventQueue.AddEvent(new PersonArrival(newTime, _stationName, _direction));
            }
            else //Otherwise, create an event for the next 15 minute block
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
