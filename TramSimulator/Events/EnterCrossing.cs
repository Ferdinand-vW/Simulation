using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    //Event for trams that want to enter a station and have to go through a crossing
    public class EnterCrossing : Event
    {
       // int _tramId;
        string _arrStation;
         
        public EnterCrossing(int tramId, string arrStation, double time)
        {
            this._tramId = tramId;
            this._arrStation = arrStation;
            this.StartTime = time;
        }

        public override void execute(SimulationState simState)
        {
            var crossing = simState.GetCrossing(_arrStation);
            var station = simState.Stations[_arrStation];
            var eventQueue = simState.EventQueue;
            var sw = crossing.Switch;
            var tram = simState.Trams[_tramId];

            //If no tram is using the crossing at this time or it is being crossed by this tram
            if (!crossing.IsBeingCrossedBy.HasValue || crossing.IsBeingCrossedBy.Value == _tramId)
            {
                //If the switch is on PR side and at the station there is no tram on PR
                //then the tram can just move ahead and does not have to wait for the switch
                if (Routes.ToPR(sw) && !station.TramIsStationedPR)
                {
                    station.TramIsStationedPR = true;
                    station.TramAtPR = _tramId;
                    tram.State = Tram.TramState.AtStation;
                    tram.Station = _arrStation;
                    Cross(simState);
                }
                //Same as above but for CS side
                else if (Routes.ToCS(sw) && !station.TramIsStationedCS)
                {
                    station.TramIsStationedCS = true;
                    station.TramAtCS = _tramId;
                    tram.State = Tram.TramState.AtStation;
                    tram.Station = _arrStation;
                    Cross(simState);
                }
                //If the switch has to be turned, we call a new EnterCrossing event
                //We add the time it takes for the switch to be 'switched' to the event
                //We also set that the crossing is in use, so that no other tram can try to cross at the same time
                else if (Routes.ToCS(sw) && !station.TramIsStationedPR)
                {
                    crossing.Switch = Routes.Dir.ToPR;
                    crossing.IsBeingCrossedBy = _tramId;
                    eventQueue.AddEvent(new EnterCrossing(_tramId, _arrStation, StartTime + Constants.SECONDS_IN_MINUTE));
                }
                //Same as above
                else if (Routes.ToPR(sw) && !station.TramIsStationedCS)
                {
                    crossing.Switch = Routes.Dir.ToCS;
                    crossing.IsBeingCrossedBy = _tramId;
                    eventQueue.AddEvent(new EnterCrossing(_tramId, _arrStation, StartTime + Constants.SECONDS_IN_MINUTE));
                }
                else
                {
                    crossing.WaitingQueue.Enqueue(new EnterCrossing(_tramId, _arrStation, StartTime));
                }
            }
            //If there is a tram using the crossing, then we add this tram to the crossing waitingQueue
            else
            {
                crossing.WaitingQueue.Enqueue(new EnterCrossing(_tramId, _arrStation, StartTime));
            }
        }

        private void Cross(SimulationState simState)
        {
            var crossing = simState.GetCrossing(_arrStation);
            var eventQueue = simState.EventQueue;
            var station = simState.Stations[_arrStation];

            crossing.IsBeingCrossedBy = null;
            eventQueue.AddEvent(new TurnAround(_tramId, _arrStation, StartTime));

            //If there are trams waiting to enter, then they get priority
            if (station.EnterTrackQueue.Count <= 0)
            {
                Crossing.HandleCrossingQueues(station, crossing, eventQueue, StartTime);
            }
        }

        public override string ToString()
        {
            return "EnterCrossing: " + _tramId + " at " + _arrStation;
        }

    }
}
