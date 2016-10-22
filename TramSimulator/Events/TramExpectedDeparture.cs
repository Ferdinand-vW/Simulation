using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    //event voor tram expected departure
    public class TramExpectedDeparture : Event
    {
        //int _tramId;
        string _depStation;

        public TramExpectedDeparture(int tramId, string depStation, double startTime)
        {
            this._tramId = tramId;
            this.StartTime = startTime;
            this._depStation = depStation;
        }

        public override void execute(SimulationState simState)
        {
            var eventQueue = simState.EventQueue;
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_depStation];

            if (_tramId == 9 && _depStation == "CS")
            {
                Console.WriteLine("test");
            }

            if(_depStation == Constants.PR || _depStation == Constants.CS)
            {
                if(station.TramAtCS.HasValue && station.TramAtCS != _tramId && station.TramAtPR.HasValue && station.TramAtPR != _tramId)
                {
                    var waitingtrams = _depStation == Constants.PR ? station.WaitingTramsToPR : station.WaitingTramsToCS;
                    waitingtrams.Enqueue(_tramId);
                }
                else
                {
                    HandleWaitTime(simState);
                }
            }

            //If a tram is still at the station, then we add the current tram to the waiting list
            if (tram.Direction == Routes.Dir.ToCS && station.TramAtCS.HasValue && station.TramAtCS != _tramId)
            {
                station.WaitingTramsToCS.Enqueue(_tramId);
            }
            else if (tram.Direction == Routes.Dir.ToPR && station.TramAtPR.HasValue && station.TramAtPR != _tramId)
            {
                station.WaitingTramsToPR.Enqueue(_tramId);
            }
            else
            {
                HandleWaitTime(simState);
            }

        }

        private void HandleWaitTime(SimulationState simState)
        {
            var station = simState.Stations[_depStation];
            var tram = simState.Trams[_tramId];
            var eventQueue = simState.EventQueue;
            //If the station is empty, we can move the tram into the next track

            if (_tramId == 9 && _depStation == "CS")
            {
                Console.WriteLine("test");
            }
            simState.Routes.MoveToNextTrack(_tramId, _depStation, simState);
            if (tram.Direction == Routes.Dir.ToCS)
            {
                station.TramAtCS = _tramId;
                station.TramIsStationedCS = true;
            }
            else
            {
                station.TramAtPR = _tramId;
                station.TramIsStationedPR = true;
            }


            tram.State = Tram.TramState.AtStation;
            tram.Station = _depStation;

            //Find the next tram
            int? id = simState.Routes.NextTram(tram.TramId, _depStation, simState);
            double currTime = StartTime;
            string central = simState.Routes.CentralToPR[0].From;
            string pr = simState.Routes.PRToCentral[0].From;

            if (id.HasValue) //If there exists a tram on the same track
            {
                Tram nextTram = simState.Trams[id.Value];
                //Tram may only depart if there is 40s between the next tram and this one
                simState.sw.WriteLine("time: " + currTime + " " + nextTram.DepartureTime);
                if (currTime - nextTram.DepartureTime >= Constants.TIME_IN_BETWEEN)
                {
                    HandleDepartureEvent(simState);
                }
                else //Tram was not able to depart and has to schedule new departure
                {
                    double timeDiff = Constants.TIME_IN_BETWEEN - (currTime - nextTram.DepartureTime);
                    eventQueue.AddEvent(new TramExpectedDeparture(_tramId, _depStation, StartTime + timeDiff));
                }
            }
            else
            {
                HandleDepartureEvent(simState);
            }
        }

        private void HandleDepartureEvent(SimulationState simState)
        {
            var eventQueue = simState.EventQueue;
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_depStation];
            var nextStation = simState.Routes.NextStation(_tramId, _depStation);
            var arrTime = StartTime + simState.Rates.TramArrivalRate(_depStation, nextStation);

            //Remember the time that this tram left his last station
            tram.DepartureTime = StartTime;
            //Handle departure of a tram
            tram.State = Tram.TramState.OnTrack;

            eventQueue.AddEvent(new TramExpectedArrival(_tramId, arrTime, nextStation));

            if (Routes.ToCS(tram.Direction))
            {
                //Console.WriteLine("Trams waiting to at " + _depStation + " to CS: " + station.WaitingTramsToCS.Count);
                //if there was a tram waiting create an arrival event
                station.lastTramCS = _tramId;
                station.TramIsStationedCS = false;
                station.TramAtCS = null;
                if (station.WaitingTramsToCS.Count > 0)
                {
                    var nextTramId = station.WaitingTramsToCS.Dequeue();
                    eventQueue.AddEvent(new TramExpectedArrival(nextTramId, StartTime, _depStation));
                }
            }
            else
            {
                station.lastTramPR = _tramId;
                station.TramIsStationedPR = false;
                station.TramAtPR = null;
                if (station.WaitingTramsToPR.Count > 0)
                {
                    var nextTramId = station.WaitingTramsToPR.Dequeue();
                    eventQueue.AddEvent(new TramExpectedArrival(nextTramId, StartTime, _depStation));
                }
            }
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " expected departure " + StartTime + " from " + _depStation;
        }
    }
}
