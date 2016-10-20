using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    //event voor tram expected arrival
    public class TramExpectedArrival : Event
    {
        int _tramId;
        string _arrStation;

        public TramExpectedArrival(int tramId, double startTime, string arrStation)
        {
            this._tramId = tramId;
            this._arrStation = arrStation;
            this.StartTime = startTime;
        }
        public override void execute(SimulationState simState)
        {
            var station = simState.Stations[_arrStation];
            var tram = simState.Trams[_tramId];
            var persons = simState.Persons;
            var rates = simState.Rates;
            var routes = simState.Routes;
            var timetable = simState.TimeTables;
            int nextTram = _tramId > 0? _tramId - 1 : simState.Trams.Count-1;

            double newTime = StartTime + 10;
            int fillRate = 0;
            double emptyRate = 0;

            if(_arrStation == "PR" || _arrStation == "CS")
            {
                HandleEndStationEvent(simState);
            }
            else
            {
                if(Routes.ToPR(tram.Direction) && station.TramIsStationedPR)
                {
                    station.WaitingTramsToPR.Enqueue(_tramId);
                }
                else if(Routes.ToCS(tram.Direction) && station.TramIsStationedCS)
                {
                    station.WaitingTramsToCS.Enqueue(_tramId);
                }

                tram.State = Tram.TramState.AtStation;
                tram.Station = _arrStation;

                emptyRate = rates.TramEmptyRate(simState.Day, _arrStation, tram.Direction, tram, StartTime);
                fillRate = rates.TramFillRate(station, tram);

                var pplExited = tram.EmptyTram(emptyRate);
                var waitingppl = Routes.ToPR(tram.Direction) ? station.WaitingPersonsToPR : station.WaitingPersonsToCS;
                var pplEntered = tram.FillTram(waitingppl, fillRate);
                //update waiting times
                pplEntered.ForEach(x =>
                {
                    persons[x].SetWaitingTime(StartTime);
                    persons[x].ArrivedAt = _arrStation;
                });
                pplExited.ForEach(x =>
                {
                    persons[x].LeaveTime = StartTime;
                    persons[x].LeftAt = _arrStation;
                });

                //Add emptying and filling time of the tram
                newTime += rates.TramEmptyTime(pplExited.Count) + rates.TramFillTime(fillRate);

                //Add delay time if doors were shut
                if (rates.DoorMalfunction()) { newTime += 60; }

                Event e = new TramExpectedDeparture(_tramId, _arrStation, newTime);
                simState.EventQueue.AddEvent(e);
            }
        }

        private void HandleEndStationEvent(SimulationState simState)
        {
            var tram = simState.Trams[_tramId];
            var persons = simState.Persons;
            var rates = simState.Rates;
            var timetable = simState.TimeTables;
            var station = simState.Stations[_arrStation];

            double newTime = StartTime + 10;
            double emptyRate = 0;
            if (_arrStation == "PR")
            {
                timetable[_tramId].renewTimeTable(timetable[_tramId].totalTime);
            }
            emptyRate = rates.TramEmptyRate(simState.Day, _arrStation, tram.Direction, tram, StartTime);

            var exited = tram.EmptyTram(emptyRate);
            //Determine waiting time for people who just entered the tram
            //Determine time that a person left the tram and at which station he did
            exited.ForEach(x =>
            {
                persons[x].LeaveTime = StartTime;
                persons[x].LeftAt = _arrStation;
            });

            //Add emptying and filling time of the tram
            newTime += rates.TramEmptyTime(exited.Count);
            var waitingtrams = _arrStation == "PR" ? station.WaitingTramsToPR : station.WaitingTramsToCS;
            if(waitingtrams.Count > 0)
            {
                waitingtrams.Enqueue(_tramId);
            }
            else
            {
                simState.EventQueue.AddEvent(new EnterCrossing(_tramId, _arrStation, newTime));
            }
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " expected arrival " + StartTime + " at " + _arrStation;
        }
    }
}
