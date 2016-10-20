using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TramSimulator.States;

namespace TramSimulator.Events
{
    class TurnAround : Event
    {
        int _tramId;
        string _arrStation;

        public TurnAround(int tramId, string arrStation, double startTime)
        {
            this._tramId = tramId;
            this._arrStation = arrStation;
            this.StartTime = startTime;
        }

        public override void execute(SimulationState simState)
        {
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_arrStation];

            double newTime = StartTime + 90;
            tram.Direction = TurnDirection(tram.Direction);
            tram.State = Tram.TramState.AtStation;
            tram.Station = _arrStation;
            GenerateEventForWaitingTram(simState);

            simState.EventQueue.AddEvent(new TramExpectedDeparture(_tramId, _arrStation, newTime));
        }

        private void GenerateEventForWaitingTram(SimulationState simState)
        {
            var station = simState.Stations[_arrStation];
            if (_arrStation == "CS")
            {
                if (station.WaitingTramsToCS.Count > 0)
                {
                    var nextTramId = station.WaitingTramsToCS.Dequeue();
                    simState.EventQueue.AddEvent(new EnterCrossing(nextTramId, _arrStation, StartTime));
                }
            }
            if (_arrStation == "PR")
            {

                if (station.WaitingTramsToPR.Count > 0)
                {
                    var nextTramId = station.WaitingTramsToPR.Dequeue();
                    simState.EventQueue.AddEvent(new EnterCrossing(nextTramId, _arrStation, StartTime));
                }
            }
        }

        private Routes.Dir TurnDirection(Routes.Dir dir)
        {
            return (Routes.Dir)(((int)dir + 1) % 2);
        }

        public override string ToString()
        {
            return "Turnaround: " + _tramId + " at " + _arrStation;
        }
    }
}