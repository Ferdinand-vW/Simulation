using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.Events;

namespace TramSimulator.States
{
    public class Crossing
    {
        public int? IsBeingCrossedBy { get; set; }
        //Assumption is that there is only a single switch that can be turned.
        //Furthermore, only a single tram can cross each time
        public Routes.Dir Switch { get; set; }
        public Queue<Event> WaitingQueue { get; set; }

        string _endStation;
        

        public Crossing(string endStation)
        {
            this.IsBeingCrossedBy = null;
            if(endStation == Constants.PR) { this.Switch = Routes.Dir.ToPR; }
            else                   { this.Switch = Routes.Dir.ToCS; }
            this.WaitingQueue = new Queue<Event>();
            this._endStation = endStation;
        }

        public static void HandleCrossingQueues(Station station, Crossing crossing, EventQueue eventQueue, double StartTime)
        {
            //Check whether there are any trams waiting to enter the crossing
            if (crossing.WaitingQueue.Count > 0)
            {
                //Get the crossing event for the next tram and insert it into the eventqueue
                Event nextCrossingEvent = crossing.WaitingQueue.Dequeue();
                nextCrossingEvent.StartTime = StartTime;
                eventQueue.AddEvent(nextCrossingEvent);
            }
            else if (station.Name == Constants.CS && station.WaitingTramsToCS.Count > 0)
            {
                var nextTramId = station.WaitingTramsToCS.Dequeue();
                eventQueue.AddEvent(new EnterCrossing(nextTramId, station.Name, StartTime));
            }
            else if (station.Name == Constants.PR && station.WaitingTramsToPR.Count > 0)
            {
                var nextTramId = station.WaitingTramsToPR.Dequeue();
                eventQueue.AddEvent(new EnterCrossing(nextTramId, station.Name, StartTime));
            }
        }
    }
}
