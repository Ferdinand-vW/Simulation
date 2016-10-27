using System;
using System.Collections.Generic;

using TramSimulator.States;
using TramSimulator.Sim;

namespace TramSimulator.Events
{
    //event voor tram expected departure
    public class TramExpectedDeparture : Event
    {
        string _depStation;
        public static int rounds;
        public static int delayed;

        public TramExpectedDeparture(int tramId, string depStation, double startTime)
        {
            this._tramId = tramId;
            this.StartTime = startTime;
            this._depStation = depStation;
            this.EType = EventType.TramDeparture;
            this.Station = depStation;
        }

        public override void Execute(SimulationState simState)
        {
            var eventQueue = simState.EventQueue;
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_depStation];

            tram.State = Tram.TramState.AtStation;
            var st = tram.Station;
            tram.Station = _depStation;

            string nextSt = simState.Routes.NextStation(_tramId, _depStation);
            Track t = simState.Routes.GetNextTrack(_depStation, nextSt);
            //Find the next tram
            int? id = simState.Routes.NextTram(t, _tramId, _depStation);
            double currTime = StartTime;
            string central = simState.Routes.CentralToPR[0].From;
            string pr = simState.Routes.PRToCentral[0].From;

            if (id.HasValue) //If there exists a tram on the same track
            {
                Tram nextTram = simState.Trams[id.Value];
                //Tram may only depart if there is 40s between the next tram and this one
                if (Math.Ceiling(currTime - nextTram.DepartureTime) >= Constants.TIME_IN_BETWEEN)
                {
                    HandleDepartureEvent(simState);
                }
                else //Tram was not able to depart and has to schedule new departure
                {
                    double timeDiff = Constants.TIME_IN_BETWEEN - (Math.Ceiling(currTime - nextTram.DepartureTime));
                    delayed++;
                    //Generate a new tram departure event
                    eventQueue.AddEvent(new TramExpectedDeparture(_tramId, _depStation, StartTime + timeDiff));
                }
            }
            else
            {
                HandleDepartureEvent(simState);
            }
            rounds++;
        }

        private void HandleDepartureEvent(SimulationState simState)
        {
            var eventQueue = simState.EventQueue;
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_depStation];
            var nextStation = simState.Routes.NextStation(_tramId, _depStation);
            var routes = simState.Routes;

            //Calculate the arrival time
            var arrTime = StartTime + simState.Rates.TramArrivalRate(_depStation, nextStation);

            //Remember the time that this tram left his last station
            tram.DepartureTime = StartTime;
            //Handle departure of a tram
            tram.State = Tram.TramState.OnTrack;

            if (_depStation != Constants.PR && _depStation != Constants.CS)
            {   routes.MoveToNextTrack(_tramId, _depStation);   }

            //Generate the arrival event
            eventQueue.AddEvent(new TramExpectedArrival(_tramId, arrTime, nextStation));

            //Generate events for waiting trams
            GenerateEventForNextTram(simState);
        }

        private void GenerateEventForNextTram(SimulationState simState)
        {
            var station = simState.Stations[_depStation];
            var tram = simState.Trams[_tramId];
            var eventQueue = simState.EventQueue;

            var nextTramId = -1;
            var waitingtrams = new Queue<int>();
            //If the station that we are departing from is not an endstation
            if (_depStation != Constants.CS && _depStation != Constants.PR)
            {
                if (Routes.ToCS(tram.Direction))
                {
                    station.TramIsStationedCS = false;
                    station.TramAtCS = null;
                    if (station.WaitingTramsToCS.Count > 0) { nextTramId = station.WaitingTramsToCS.Peek(); }
                }
                else
                {
                    station.TramIsStationedPR = false;
                    station.TramAtPR = null;
                    if (station.WaitingTramsToPR.Count > 0) { nextTramId = station.WaitingTramsToPR.Peek(); }
                }
            }
            //If it is an endstation, then set the tram to no longer be at the station
            else if (_depStation == Constants.CS || _depStation == Constants.PR)
            {
                if (station.TramAtPR == _tramId)
                {
                    station.TramIsStationedPR = false;
                    station.TramAtPR = null;
                }
                else if (station.TramAtCS == _tramId)
                {
                    station.TramIsStationedCS = false;
                    station.TramAtCS = null;
                }

                waitingtrams = _depStation == Constants.PR ? station.WaitingTramsToPR : station.WaitingTramsToCS; 
            }
            //If we are at PR then any trams waiting to enter the track have priority
            if (_depStation == Constants.PR && station.EnterTrackQueue.Count > 0)
            {
                nextTramId = station.EnterTrackQueue.Dequeue();
                eventQueue.AddEvent(new EnterTrack(nextTramId, _depStation, StartTime));
            }
            else
            {
                if (waitingtrams.Count > 0) { nextTramId = waitingtrams.Peek(); }
                //Only generate an arrival event if there actually is a tram waiting
                if (nextTramId != -1)
                {   eventQueue.AddEvent(new TramExpectedArrival(nextTramId, StartTime, _depStation));   }
            }
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " expected departure " + StartTime + " from " + _depStation;
        }
    }
}
