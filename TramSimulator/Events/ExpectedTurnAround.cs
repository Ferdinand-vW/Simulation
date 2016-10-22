using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TramSimulator.States;

namespace TramSimulator.Events
{
    class ExpectedTurnAround : Event
    {
        int _tramId;
        string _arrStation;

        public ExpectedTurnAround(int tramId, double startTime, string arrStation)
        {
            this._tramId = tramId;
            this.StartTime = startTime;
            this._arrStation = arrStation;
        }

        public override void execute(SimulationState simState)
        {
           
            Tram tram = simState.Trams[_tramId];
            Station station = simState.Stations[_arrStation];
            int nextTram = _tramId > 0 ? _tramId - 1 : simState.Trams.Count - 1;

            double newTime = StartTime + 90;

            if (_arrStation == "CS" )
            {
                if (station.TramIsStationedPR || (station.lastTramPR != nextTram && station.lastTramPR != -1)) {
                    simState.turnAroundCS.Enqueue(_tramId);
                    return;
                }
                tram.Direction = Routes.Dir.ToPR;
                station.TramIsStationedCS = false;
                station.lastTramCS = _tramId;
                if (station.WaitingTramsToCS.Count > 0)
                {
                    var wTramId = station.WaitingTramsToCS.Dequeue();
                    simState.EventQueue.AddEvent(new TramExpectedArrival(wTramId, StartTime, _arrStation));
                }
            }
            if (_arrStation == "PR")
            {
                if (station.TramIsStationedCS || (station.lastTramCS != nextTram && station.lastTramCS != -1))
                {
                    simState.turnAroundPR.Enqueue(_tramId);
                    return;
                }
                tram.Direction = Routes.Dir.ToCS;
                station.TramIsStationedPR = false;
                station.lastTramPR = _tramId;
                if (station.WaitingTramsToPR.Count > 0)
                {
                    var wTramId = station.WaitingTramsToPR.Dequeue();
                    simState.EventQueue.AddEvent(new TramExpectedArrival(wTramId, StartTime, _arrStation));
                }
            }
            Event e = new TramExpectedArrival(_tramId, newTime, _arrStation);
            simState.EventQueue.AddEvent(e);
        }
        public override string ToString()
        {
            return "Tram " + _tramId + " turn around " + StartTime + " at " + _arrStation;
        }
    }
}