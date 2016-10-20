using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    //Event for trams that wish to leave an endstation and have to go through a crossing
    public class DepartCrossing : Event
    {
        int _tramId;
        string _depStation;
        string _arrStation;

        public DepartCrossing(int tramId, string depStation, string arrStation, double time)
        {
            this._tramId = tramId;
            this._depStation = depStation;
            this._arrStation = arrStation;
            this.StartTime = time;
        }

        public override void execute(SimulationState simState)
        {
            var crossing = simState.GetCrossing(_depStation);
            var station = simState.Stations[_depStation];
            var eventQueue = simState.EventQueue;
            var sw = crossing.Switch;

            //If no tram is using the crossing at this time or it is being crossed by this tram
            if (!crossing.IsBeingCrossedBy.HasValue || crossing.IsBeingCrossedBy.Value == _tramId)
            {
                if(Routes.ToPR(sw) && station.TramAtPR.HasValue && station.TramAtPR.Value == _tramId)
                {
                    station.TramAtPR = null;
                    station.TramIsStationedPR = false;
                    Cross(simState);
                }
                else if(Routes.ToCS(sw) && station.TramAtCS.HasValue && station.TramAtCS.Value == _tramId)
                {
                    station.TramAtCS = null;
                    station.TramIsStationedCS = false;
                    Cross(simState);
                }
                else if(Routes.ToPR(sw) && station.TramAtCS.HasValue && station.TramAtCS.Value == _tramId)
                {
                    crossing.Switch = Routes.Dir.ToCS;
                    crossing.IsBeingCrossedBy = _tramId;
                    eventQueue.AddEvent(new DepartCrossing(_tramId, _depStation, _arrStation, StartTime + 60));
                }
                else if(Routes.ToCS(sw) && station.TramAtPR.HasValue && station.TramAtPR.Value == _tramId)
                {
                    crossing.Switch = Routes.Dir.ToPR;
                    crossing.IsBeingCrossedBy = _tramId;
                    eventQueue.AddEvent(new DepartCrossing(_tramId, _depStation, _arrStation, StartTime + 60));
                }
            }
            else
            {
                crossing.WaitingQueue.Enqueue(new DepartCrossing(_tramId, _depStation, _arrStation, StartTime));
            }
        }

        private void Cross(SimulationState simState)
        {
            var crossing = simState.GetCrossing(_depStation);
            var eventQueue = simState.EventQueue;

            crossing.IsBeingCrossedBy = null;
            var arrTime = StartTime + simState.Rates.TramArrivalRate(_depStation, _arrStation);
            eventQueue.AddEvent(new TramExpectedArrival(_tramId, arrTime, _arrStation));

            //Check whether there are any trams waiting to enter the crossing
            if (crossing.WaitingQueue.Count > 0)
            {
                //Get the crossing event for the next tram and insert it into the eventqueue
                Event nextCrossingEvent = crossing.WaitingQueue.Dequeue();
                //Since we only now know the time that an event should be invoked, we can assign it
                nextCrossingEvent.StartTime = this.StartTime;
                eventQueue.AddEvent(nextCrossingEvent);
            }
        }

        public override string ToString()
        {
            return "DepartCrossing: " + _tramId + " from " + _depStation;
        }
    }
}
