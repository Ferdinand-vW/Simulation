﻿using System;
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
        int _tramId;
        string _depStation;

        public TramExpectedDeparture(int tramId, double startTime, string depStation)
        {
            this._tramId = tramId;
            this.StartTime = startTime;
            this._depStation = depStation;
        }

        public override void execute(SimulationState simState)
        {
            var eventQueue = simState.EventQueue;
            var tram = simState.Trams[_tramId];

            //Find the next tram
            int? id = simState.Routes.NextTram(tram.TramId, _depStation);
            double currTime = StartTime;
            double timeFromCentral = simState.TimeTables[_tramId].halfTime;
            string central = simState.Routes.CentralToPR[0].From;
            double timeFromPR = simState.TimeTables[_tramId].startTime;
            string pr = simState.Routes.PRToCentral[0].From;
            var station = simState.Stations[_depStation];

            if (_depStation == central && StartTime < timeFromCentral)
                eventQueue.AddEvent(new TramExpectedDeparture(_tramId, timeFromCentral, _depStation));
            //  Op PR wachten op time table
            else if (_depStation == pr && StartTime < timeFromPR)
                eventQueue.AddEvent(new TramExpectedDeparture(_tramId, timeFromPR, _depStation));
            else //If there exists a tram on the same track
            {
                //Tram may only depart if there is 40s between the next tram and this one
                double lastTramTime = tram.Direction == Routes.Dir.ToCS ? station.lastTramTimeCS : station.lastTramTimePR;
                if (currTime - lastTramTime >= 40)
                {
                    HandleDepartureEvent(simState);
                }
                else //Tram was not able to depart and has to schedule new departure
                {
                    eventQueue.AddEvent(new TramExpectedDeparture(_tramId, StartTime + timeDiff, _depStation));
                }
            }
        }

        private void HandleDepartureEvent(SimulationState simState)
        {
            var eventQueue = simState.EventQueue;
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_depStation];
            var nextStation = simState.Routes.NextStation(_tramId, _depStation);
            var arrTime = StartTime + simState.Rates.TramArrivalRate(_depStation, nextStation);

            //update timeTable
            simState.TimeTables[_tramId].update(StartTime, _depStation);

            //Handle departure of a tram
            tram.State = Tram.TramState.OnTrack;
            eventQueue.AddEvent(new TramExpectedArrival(_tramId, arrTime, nextStation));
            simState.Routes.MoveToNextTrack(_tramId, _depStation);

            if (Routes.ToCS(tram.Direction))
            {
                //Console.WriteLine("Trams waiting to at " + _depStation + " to CS: " + station.WaitingTramsToCS.Count);
                //if there was a tram waiting create an arrival event
                station.lastTramCS = _tramId;
                station.lastTramTimeCS = StartTime;
                station.TramIsStationedCS = false;
                if (station.WaitingTramsToCS.Count > 0)
                {
                    var wTramId = station.WaitingTramsToCS.Dequeue();
                    eventQueue.AddEvent(new TramExpectedArrival(wTramId, StartTime, _depStation));
                }
                if (_depStation == "PR" && simState.turnAroundPR.Count > 0) {
                    var wTramId = simState.turnAroundPR.Dequeue();
                    eventQueue.AddEvent(new ExpectedTurnAround(wTramId, StartTime, _depStation));
                }
            }
            else
            {
                station.lastTramPR = _tramId;
                station.lastTramTimePR = StartTime;
                station.TramIsStationedPR = false;
                if (station.WaitingTramsToPR.Count > 0)
                {
                    var wTramId = station.WaitingTramsToPR.Dequeue();
                    eventQueue.AddEvent(new TramExpectedArrival(wTramId, StartTime, _depStation));
                }
                if (_depStation == "CS" && simState.turnAroundCS.Count > 0)
                {
                    var wTramId = simState.turnAroundCS.Dequeue();
                    eventQueue.AddEvent(new ExpectedTurnAround(wTramId, StartTime, _depStation));
                }
            }

        }

        public override string ToString()
        {
            return "Tram " + _tramId + " expected departure " + StartTime + " from " + _depStation;
        }
    }
}
