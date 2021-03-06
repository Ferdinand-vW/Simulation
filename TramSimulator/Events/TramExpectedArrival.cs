﻿using System;
using System.Collections.Generic;
using System.Linq;

using TramSimulator.States;
using TramSimulator.Sim;

namespace TramSimulator.Events
{
    //event voor tram expected arrival
    public class TramExpectedArrival : Event
    {
        string _arrStation;

        public TramExpectedArrival(int tramId, double startTime, string arrStation)
        {
            this._tramId = tramId;
            this._arrStation = arrStation;
            this.StartTime = startTime;
            this.EType = EventType.TramArrival;
            this.Station = arrStation;
        }
        public override void Execute(SimulationState simState)
        {
            var station = simState.Stations[_arrStation];
            var tram = simState.Trams[_tramId];
            var persons = simState.Persons;
            var rates = simState.Rates;
            var routes = simState.Routes;
            var timetable = simState.TimeTable;
            int nextTram = _tramId > 0 ? _tramId - 1 : simState.Trams.Count-1;

            double newTime = StartTime;
            int fillRate = 0;
            double emptyRate = 0;
            
            if (_arrStation == Constants.PR || _arrStation == Constants.CS)
            {
                HandleEndStationEvent(simState);
            }
            else
            {
                //If there is a tram at the station, wait in the queue
                if(Routes.ToPR(tram.Direction) && station.TramIsStationedPR)
                {
                    station.WaitingTramsToPR.Enqueue(_tramId);
                }
                else if(Routes.ToCS(tram.Direction) && station.TramIsStationedCS)
                {
                    station.WaitingTramsToCS.Enqueue(_tramId);
                }
                else
                {
                    //Set this tram to be at the station
                    if(Routes.ToPR(tram.Direction))
                    {
                        station.TramAtPR = _tramId;
                        station.TramIsStationedPR = true;
                    }
                    else if(Routes.ToCS(tram.Direction))
                    {
                        station.TramAtCS = _tramId;
                        station.TramIsStationedCS = true;
                    }

                    tram.State = Tram.TramState.AtStation;
                    tram.Station = _arrStation;

                    var waitingppl = Routes.ToPR(tram.Direction) ? station.WaitingPersonsToPR : station.WaitingPersonsToCS;

                    emptyRate = rates.TramEmptyRate(simState.Day, _arrStation, tram.Direction, tram, StartTime);

                    var oldPersonCount = tram.PersonsOnTram.Count;
                    var pplExited = tram.EmptyTram(emptyRate);
                    var transfer = oldPersonCount - pplExited.Count;

                    fillRate = rates.TramFillRate(station, tram);
                   
                    var oldwaitingppl = new Queue<int>(waitingppl);
                    waitingppl.ToList().ForEach(x => persons[x].PassedTrams.Add(Tuple.Create(_tramId, tram.PersonsOnTram.Count)));
                    var oldtramcount = new List<int>(tram.PersonsOnTram);
                    var pplEntered = tram.FillTram(waitingppl, fillRate);

                    pplEntered.ForEach(x =>
                    {
                        persons[x].SetWaitingTime(StartTime);
                        persons[x].ArrivedAt = _arrStation;
                        persons[x].EnteredTramTime = StartTime;
                    });
                    pplExited.ForEach(x =>
                    {
                        persons[x].LeaveTime = StartTime;
                        persons[x].LeftAt = _arrStation;
                    });

                    //Add emptying and filling time of the tram
                    newTime += rates.DwellTime(pplEntered.Count, pplExited.Count, transfer);

                    //Add delay time if doors were shut
                    if (rates.DoorMalfunction()) { newTime += Constants.SECONDS_IN_MINUTE; }

                    var waitingtrams = Routes.ToCS(tram.Direction) ? station.WaitingTramsToCS : station.WaitingTramsToPR;
                    if(waitingtrams.Count > 0 && waitingtrams.Peek() == _tramId) { waitingtrams.Dequeue(); }

                    simState.EventQueue.AddEvent(new TramExpectedDeparture(_tramId, _arrStation, newTime));
                }
            }
        }

        private void HandleEndStationEvent(SimulationState simState)
        {
            var tram = simState.Trams[_tramId];
            var persons = simState.Persons;
            var rates = simState.Rates;
            var timetable = simState.TimeTable;
            var station = simState.Stations[_arrStation];

            var waitingtrams = _arrStation == Constants.PR ? station.WaitingTramsToPR : station.WaitingTramsToCS;

            //Wait in queue at endstation
            if(((waitingtrams.Count > 0 && waitingtrams.Peek() != _tramId) || (station.TramAtPR.HasValue && station.TramAtCS.HasValue) || station.EnterTrackQueue.Count > 0))
            {
                //If a tram just entered PR, we have to check that the current tram isn't already in the queue
                if (!waitingtrams.Contains(_tramId))
                { waitingtrams.Enqueue(_tramId); }
            }
            else
            {
                if (!station.TramAtPR.HasValue)
                {
                    station.TramAtPR = _tramId;
                    station.TramIsStationedPR = true;
                }
                else
                {
                    station.TramAtCS = _tramId;
                    station.TramIsStationedCS = true;
                }
                
                //The only way to get to this point is if the tram is still at the head of the queue. 
                //We've now set that this tram is at this station, so we can remove him from the queue.
                if (waitingtrams.Count > 0)
                { waitingtrams.Dequeue(); }

                simState.EventQueue.AddEvent(new TurnAround(_tramId, _arrStation, StartTime));
            }
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " expected arrival " + StartTime + " at " + _arrStation;
        }
    }
}
