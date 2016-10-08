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
            var tram = simState.Trams[_tramId];
            //Tram has to wait until station is empty
            bool onA = simState.Routes.OnA(_tramId);
            if ( station.TramIsStationed(onA))
            {
                if(onA) station.WaitingTramsA.Enqueue(_tramId);
                else station.WaitingTramsB.Enqueue(_tramId);
            }
            else
            {
                if (onA) station.TramIsStationedA = true;
                else station.TramIsStationedB = true;
                tram.State = Tram.TramState.AtStation;
                tram.Station = _arrStation;
                string pr = simState.Routes.CentralToPR[0].To;
                string cs = simState.Routes.PRToCentral[0].To;
                double newTime = StartTime + 10;
                if (_arrStation == pr)
                {
                    simState.TimeTables[_tramId].renewTimeTable(simState.Rates, simState.Routes, simState.TimeTables[_tramId].totalTime);
                    newTime += 180;
                    //omdraaien
                }
                else if (_arrStation == cs){
                    //omdraaien
                    newTime += 180;
                }
                //var emptyRate = simState.Rates.TramEmptyRate(_arrStation);
                //var fillRate = simState.Rates.TramFillRate(_arrStation);

                Event e = new TramExpectedDeparture(_tramId,newTime, _arrStation);
                simState.EventQueue.AddEvent(e);
            }
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " expected arrival " + StartTime + " at " + _arrStation;
        }
    }
}
