using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TramSimulator.States;

namespace TramSimulator.Events
{
    class TurnAroundFree : Event
    {
        int _tramId;
        string _arrStation;

        public TurnAroundFree(int tramId, double startTime, string arrStation)
        {
            this._tramId = tramId;
            this.StartTime = startTime;
            this._arrStation = arrStation;
        }

        public override void execute(SimulationState simState)
        {
            Tram tram = simState.Trams[_tramId];
            Station station = simState.Stations[_arrStation];
            double newTime = StartTime + 90;
            if (_arrStation == "CS")
            {
                tram.Direction = Routes.Dir.ToPR;
                station.TramIsStationedCS = false;
                if (station.WaitingTramsToCS.Count > 0)
                {
                    var wTramId = station.WaitingTramsToCS.Dequeue();
                    simState.EventQueue.AddEvent(new TramExpectedArrival(wTramId, StartTime, _arrStation));
                }
            }
            if (_arrStation == "PR")
            {
                tram.Direction = Routes.Dir.ToCS;
                station.TramIsStationedPR = false;
                if (station.WaitingTramsToPR.Count > 0)
                {
                    var wTramId = station.WaitingTramsToPR.Dequeue();
                    simState.EventQueue.AddEvent(new TramExpectedArrival(wTramId, StartTime, _arrStation));
                }
            }
            Event e = new TramExpectedArrival(_tramId, newTime, _arrStation);
            simState.EventQueue.AddEvent(e);
        }
    }
}