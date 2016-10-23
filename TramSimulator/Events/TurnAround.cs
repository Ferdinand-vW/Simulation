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

            tram.State = Tram.TramState.AtStation;
            tram.Station = _arrStation;
            var persons = simState.Persons;
            var rates = simState.Rates;
            var sw = simState.sw;

            //GenerateEventForWaitingTram(simState);
            //Update the timetable and get the new expected departuretime
            double newTime = StartTime;
            //sw.WriteLine("----------------");
            //sw.WriteLine("On tram " + _tramId + ": " + tram.PersonsOnTram.Count);
            //sw.WriteLine("exiting...");
            double emptyRate = rates.TramEmptyRate(simState.Day, _arrStation, tram.Direction, tram, StartTime);
            var oldpersonCount = tram.PersonsOnTram.Count;
            var pplExited = tram.EmptyTram(emptyRate);
            var transfer = oldpersonCount - pplExited.Count;
            //sw.WriteLine("On tram " + _tramId + ": " + tram.PersonsOnTram.Count);

            tram.Direction = TurnDirection(tram.Direction);
            //sw.Write("Tram times: ");
            tram.Times.ForEach(x => sw.WriteLine(x + " "));
            //sw.WriteLine();
            tram.Times = new List<Tuple<double,string>>();

            //sw.WriteLine("Turned");
            int fillRate = rates.TramFillRate(station, tram);
            var waitingppl = Routes.ToPR(tram.Direction) ? station.WaitingPersonsToPR : station.WaitingPersonsToCS;
            //waitingppl.ToList().ForEach(x => persons[x].PassedTrams.Add(Tuple.Create(_tramId, tram.PersonsOnTram.Count)));
            //sw.WriteLine("entering...");
            var pplEntered = tram.FillTram(waitingppl, fillRate);
            //sw.WriteLine("On tram " + _tramId + ": " + tram.PersonsOnTram.Count);
            //simState.sw.WriteLine("People entered: " + pplEntered.Count);
            //simState.sw.WriteLine("People exited: " + pplExited.Count);
            //simState.sw.WriteLine("Waiting people: " + waitingppl.Count);
            //update waiting times
            //sw.WriteLine("Tram arrival time: " + StartTime);
            pplEntered.ForEach(x =>
            {
                //sw.WriteLine("---------ENTERING");
                //sw.WriteLine("\tPerson arrived at " + persons[x].ArrivalTime);
                persons[x].SetWaitingTime(StartTime);
                persons[x].ArrivedAt = _arrStation;
                persons[x].EnteredTramTime = StartTime;
                //sw.WriteLine("\tWaiting time is " + (StartTime - persons[x].ArrivalTime));
                //sw.WriteLine("\tWaiting time is " + persons[x].WaitingTime);
            });
            pplExited.ForEach(x =>
            {
                //sw.WriteLine("---------LEAVING");
                //sw.WriteLine("\tPerson arrived at " + persons[x].ArrivalTime);
                //sw.WriteLine("\tPerson stepped in at " + persons[x].ArrivedAt);
                persons[x].LeaveTime = StartTime;
                persons[x].LeftAt = _arrStation;
                //sw.WriteLine("\tLeft at is " + persons[x].LeaveTime);
                //sw.WriteLine("\tTravel + wait time is " + (persons[x].LeaveTime - persons[x].ArrivalTime));
                //sw.WriteLine("\tTravel time is " + (persons[x].LeaveTime - persons[x].EnteredTramTime));
            });

            //Add emptying and filling time of the tram
            newTime += rates.DwellTime(pplEntered.Count, pplExited.Count, transfer);

            newTime = timetable.UpdateTimetable(_tramId, _arrStation, newTime + Constants.ACTUAL_TURNAROUND_TIME);

            //sw.WriteLine("Timetable: ");
            //sw.WriteLine("CS avg: " + timetable.CSAverageDelay);
            //sw.WriteLine("CS max: " + timetable.CSMaxDelay);
            //sw.WriteLine("PR avg: " + timetable.PRAverageDelay);
            //sw.WriteLine("PR max: " + timetable.PRMaxDelay);
            //sw.WriteLine("Delays above one: " + timetable.DelaysOverOneMinute);
            //Add delay time if doors were shut
            //if (rates.DoorMalfunction()) { newTime += Constants.SECONDS_IN_MINUTE; }


            if (newTime <= 0) { throw new Exception("test"); }
            simState.sw.WriteLine("Generated departure for " + _tramId + " at " + newTime);
            simState.EventQueue.AddEvent(new TramExpectedDeparture(_tramId, _arrStation, newTime));
        }

        private void GenerateEventForWaitingTram(SimulationState simState)
        {
            var station = simState.Stations[_arrStation];
            if (_arrStation == Constants.CS)
            {
                if (station.WaitingTramsToCS.Count > 0)
                {
                    var nextTramId = station.WaitingTramsToCS.Dequeue();
                    simState.EventQueue.AddEvent(new TramExpectedArrival(nextTramId, StartTime, _arrStation));
                }
            }
            if (_arrStation == Constants.PR)
            {

                if (station.WaitingTramsToPR.Count > 0)
                {
                    var nextTramId = station.WaitingTramsToPR.Dequeue();
                    simState.EventQueue.AddEvent(new TramExpectedArrival(nextTramId, StartTime, _arrStation));
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