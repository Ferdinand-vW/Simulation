using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    //event voor tram expected arrival
    public class TramExpectedArrival : Event
    {
        int _tramId;
        string _arrStation;
        public TramExpectedArrival(int tramId, double startTime, string arrStation)
        {
            this._tramId = tramId;
            this._arrStation = arrStation;
            this.StartTime = startTime;
        }
        public override void execute(SimulationState simState)
        {
            var station = simState.Stations[_arrStation];
            bool typeA = simState.Routes.OnA(_tramId);

            //Tram has to wait until station is empty

            if (station.TramIsStationed(typeA))
            {
                if (typeA) station.WaitingTramsA.Enqueue(_tramId);
                else station.WaitingTramsB.Enqueue(_tramId);
            }
            else
            {
                HandleArrival(simState);
            }
        }

        private void HandleArrival(SimulationState simState)
        {
            var station = simState.Stations[_arrStation];
            var tram = simState.Trams[_tramId];
            bool typeA = simState.Routes.OnA(_tramId);

            if (typeA) station.TramIsStationedA = true;
            else station.TramIsStationedB = true;
            tram.State = Tram.TramState.AtStation;
            tram.Station = _arrStation;

            double newTime = StartTime + HandlePassengers(simState);

            // misschien moet omdraaien een eigen event krijgen
            if (_arrStation == "PR")
            {
                simState.TimeTables[_tramId].renewTimeTable(simState.TimeTables[_tramId].totalTime);
                newTime += 180;
            }
            else if (_arrStation == "CS")
            {
                newTime += 180;
            }

            Event e = new TramExpectedDeparture(_tramId, newTime, _arrStation);
            simState.EventQueue.AddEvent(e);
        }

        public double HandlePassengers(SimulationState simState)
        {
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_arrStation];
            bool typeA = simState.Routes.OnA(_tramId);
            Queue<Person> waitingPersons = simState.Routes.OnA(_tramId) ? station.WaitingPersonsA : station.WaitingPersonsB;

            int countExit = tram.EmptyTram(simState.Rates.TramEmptyRate(_arrStation,typeA));
            int countEnter = tram.FillTram(waitingPersons);
            return simState.Rates.DelayRate(countEnter,countExit);
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " expected arrival " + StartTime + " at " + _arrStation;
        }
    }
}
