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
            //Tram has to wait until station is empty
            if (tram.Direction == Routes.Dir.ToPR)
            {
                if(station.TramIsStationedPR || (station.lastTramPR != nextTram && station.lastTramPR != -1))
                {
                    //Console.WriteLine("Tram " + _tramId + " enters the queue to PR");
                    station.WaitingTramsToPR.Enqueue(_tramId);
                    //Console.WriteLine(station.WaitingTramsToPR.Count);
                }
                else
                {
                    station.TramIsStationedPR = true;
                    tram.State = Tram.TramState.AtStation;
                    tram.Station = _arrStation;
                    string pr = routes.CentralToPR[routes.CentralToPR.Count - 1].To;
                    if(_arrStation == pr)
                    {
                        timetable[_tramId].renewTimeTable(timetable[_tramId].totalTime);
                        newTime += 90;
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
                        simState.EventQueue.AddEvent(new ExpectedTurnAround(_tramId, newTime, _arrStation));
                        return;
                    }

                    emptyRate = rates.TramEmptyRate(simState.Day, _arrStation, tram.Direction, tram, StartTime);
                    fillRate = rates.TramFillRate(station, tram);

                    var pplExited = tram.EmptyTram(emptyRate);
                    var pplEntered = tram.FillTram(station.WaitingPersonsToPR, fillRate);
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

                    Event e = new TramExpectedDeparture(_tramId, newTime, _arrStation);
                    simState.EventQueue.AddEvent(e);

                }
            }
            else
            {
                if(station.TramIsStationedCS || (station.lastTramCS != nextTram && station.lastTramCS != -1))
                {
                    //Console.WriteLine("Tram " + _tramId + " enters the queue to CS");
                    station.WaitingTramsToCS.Enqueue(_tramId);
                    //Console.WriteLine(station.WaitingTramsToCS.Count);
                }
                else
                {
                    station.TramIsStationedCS = true;
                    tram.State = Tram.TramState.AtStation;
                    tram.Station = _arrStation;
                    string cs = routes.PRToCentral[routes.PRToCentral.Count - 1].To;
                    if (_arrStation == cs)
                    {
                        newTime += 90;
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
                        simState.EventQueue.AddEvent(new ExpectedTurnAround(_tramId, newTime, _arrStation));
                        return;
                    }

                    emptyRate = rates.TramEmptyRate(simState.Day, _arrStation, tram.Direction, tram, StartTime);
                    fillRate = rates.TramFillRate(station, tram);
                  
                    var pplExited = tram.EmptyTram(emptyRate);
                    var pplEntered = tram.FillTram(station.WaitingPersonsToCS, fillRate);
                    //Determine waiting time for people who just entered the tram
                    pplEntered.ForEach(x => 
                    {
                        persons[x].SetWaitingTime(StartTime);
                        persons[x].ArrivedAt = _arrStation;
                    });
                    //Determine time that a person left the tram and at which station he did
                    pplExited.ForEach(x =>
                    {
                        persons[x].LeaveTime = StartTime;
                        persons[x].LeftAt = _arrStation;
                    });

                    //Add emptying and filling time of the tram
                    newTime += rates.TramEmptyTime(pplExited.Count) + rates.TramFillTime(fillRate);

                    //Add delay time if doors were shut
                    if (rates.DoorMalfunction()) { newTime += 60; }

                    Event e = new TramExpectedDeparture(_tramId, newTime, _arrStation);
                    simState.EventQueue.AddEvent(e);
                }
            }
            
        }



        public override string ToString()
        {
            return "Tram " + _tramId + " expected arrival " + StartTime + " at " + _arrStation;
        }
    }
}
