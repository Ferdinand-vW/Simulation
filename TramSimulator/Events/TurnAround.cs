using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TramSimulator.States;

namespace TramSimulator.Events
{
    class TurnAround : Event
    {
        //int _tramId;
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
            var timetable = simState.TimeTable;

            tram.Direction = TurnDirection(tram.Direction);
            tram.State = Tram.TramState.AtStation;
            tram.Station = _arrStation;
            var persons = simState.Persons;
            var rates = simState.Rates;

            //GenerateEventForWaitingTram(simState);
            //Update the timetable and get the new expected departuretime
            double newTime = StartTime;

            double emptyRate = rates.TramEmptyRate(simState.Day, _arrStation, tram.Direction, tram, StartTime);
            int fillRate = rates.TramFillRate(station, tram);

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
            if (rates.DoorMalfunction()) { newTime += Constants.SECONDS_IN_MINUTE; }

            newTime = timetable.UpdateTimetable(_tramId, _arrStation, newTime + timetable.TurnAroundTime);

            simState.EventQueue.AddEvent(new DepartCrossing(_tramId, _arrStation, newTime));
        }

        private void GenerateEventForWaitingTram(SimulationState simState)
        {
            var station = simState.Stations[_arrStation];
            if (_arrStation == Constants.CS)
            {
                if (station.WaitingTramsToCS.Count > 0)
                {
                    var nextTramId = station.WaitingTramsToCS.Dequeue();
                    simState.EventQueue.AddEvent(new EnterCrossing(nextTramId, _arrStation, StartTime));
                }
            }
            if (_arrStation == Constants.PR)
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