using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    public class EnterTrack : Event
    {
       // int _tramId;
        string _station;

        public EnterTrack(int tramId, string station, double startTime)
        {
            this._tramId = tramId;
            this._station = station;
            this.StartTime = startTime;
        }

        public override void execute(SimulationState simState)
        {
            var station = simState.Stations[_station];
            //If the tram can enter PR
            if(!station.TramIsStationedPR || !station.TramIsStationedCS)
            {
                if (!station.TramIsStationedPR)
                {
                    station.TramAtPR = _tramId;
                    station.TramIsStationedPR = true;
                }
                else if (!station.TramIsStationedCS)
                {
                    station.TramAtCS = _tramId;
                    station.TramIsStationedCS = true;
                }

                //Insert the tram into the route
                var route = simState.Routes.PRToCentral;
                route[0].Trams.Insert(0, _tramId);
                var tram = simState.Trams[_tramId];
                tram.State = Tram.TramState.AtStation;
                tram.Station = _station;
                //Add an initial event
                var eventQueue = simState.EventQueue;

                if(station.EnterTrackQueue.Count > 0)
                {
                    station.EnterTrackQueue.Dequeue();
                }
                
                eventQueue.AddEvent(new TramExpectedDeparture(_tramId, _station, StartTime));

                if(station.EnterTrackQueue.Count > 0)
                {
                    var nextTramId = station.EnterTrackQueue.Dequeue();
                    eventQueue.AddEvent(new EnterTrack(nextTramId, _station, StartTime));
                }
                else if(station.WaitingTramsToPR.Count > 0)
                {
                    var nextTramId = station.WaitingTramsToPR.Peek();
                    eventQueue.AddEvent(new TramExpectedArrival(nextTramId, StartTime, _station));

                }
            }
            else
            {   //Insert the tram into an entertrack queue. This queue has the highest priority.
                station.EnterTrackQueue.Enqueue(_tramId);
            }
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " enters " + _station + " at " + StartTime;
        }
    }
}
