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
        //int _tramId;
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
            var timetable = simState.TimeTable;
            var sw = simState.sw;
            int nextTram = _tramId > 0 ? _tramId - 1 : simState.Trams.Count-1;

            double newTime = StartTime;
            int fillRate = 0;
            double emptyRate = 0;

            if(_arrStation == Constants.PR || _arrStation == Constants.CS)
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
                else
                {
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

                    tram.Times.Add(Tuple.Create(StartTime, _arrStation));

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
                       // sw.WriteLine("\tWaiting time is " + (StartTime - persons[x].ArrivalTime));
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
                       // sw.WriteLine("\tTravel + wait time is " + (persons[x].LeaveTime - persons[x].ArrivalTime));
                       // sw.WriteLine("\tTravel time is " + (persons[x].LeaveTime - persons[x].EnteredTramTime));
                    });

                    //Add emptying and filling time of the tram
                    newTime += rates.DwellTime(pplEntered.Count, pplExited.Count, transfer);

                    //Add delay time if doors were shut
                    if (rates.DoorMalfunction()) { newTime += Constants.SECONDS_IN_MINUTE; }

                    var waitingtrams = Routes.ToCS(tram.Direction) ? station.WaitingTramsToCS : station.WaitingTramsToPR;
                    if(waitingtrams.Count > 0 && waitingtrams.Peek() == _tramId) { waitingtrams.Dequeue(); }

                    Event e = new TramExpectedDeparture(_tramId, _arrStation, newTime);
                    simState.EventQueue.AddEvent(e);
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

            if((waitingtrams.Count > 0 && waitingtrams.Peek() != _tramId)|| (station.TramAtPR.HasValue && station.TramAtCS.HasValue) || station.EnterTrackQueue.Count > 0)
            {
                waitingtrams.Enqueue(_tramId);
            }
            else
            {
                if(!station.TramAtPR.HasValue)
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
                if(waitingtrams.Count > 0)
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
