using TramSimulator.States;
using TramSimulator.Sim;

namespace TramSimulator.Events
{
    class TurnAround : Event
    {
        string _arrStation;

        public TurnAround(int tramId, string arrStation, double startTime)
        {
            this._tramId = tramId;
            this._arrStation = arrStation;
            this.StartTime = startTime;
            this.EType = EventType.TurnAround;
            this.Station = arrStation;
        }

        public override void Execute(SimulationState simState)
        {
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_arrStation];
            var timetable = simState.TimeTable;
            var routes = simState.Routes;
            var persons = simState.Persons;
            var rates = simState.Rates;

            //Set tram state
            tram.State = Tram.TramState.AtStation;
            tram.Station = _arrStation;

            double newTime = StartTime;
            //Determine the percentage of passengers that leave the tram
            double emptyRate = rates.TramEmptyRate(simState.Day, _arrStation, tram.Direction, tram, StartTime);
            var oldpersonCount = tram.PersonsOnTram.Count;
            //Actual number of passengers leaving the tram
            var pplExited = tram.EmptyTram(emptyRate);
            var transfer = oldpersonCount - pplExited.Count;

            //Turn
            tram.Direction = TurnDirection(tram.Direction);
            routes.MoveToNextTrack(_tramId, _arrStation);
            

            //How many people are in the tram
            int fillRate = rates.TramFillRate(station, tram);
            //How many people are waiting
            var waitingppl = Routes.ToPR(tram.Direction) ? station.WaitingPersonsToPR : station.WaitingPersonsToCS;

            //How many people actually entered
            var pplEntered = tram.FillTram(waitingppl, fillRate);

            //Set information about  waiting, entering and leaving for each related passenger
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

            newTime = timetable.UpdateTimetable(_tramId, _arrStation, newTime + Constants.ACTUAL_TURNAROUND_TIME);

            //Add delay time if doors were shut
            if (rates.DoorMalfunction()) { newTime += Constants.SECONDS_IN_MINUTE; }

            simState.EventQueue.AddEvent(new TramExpectedDeparture(_tramId, _arrStation, newTime));
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