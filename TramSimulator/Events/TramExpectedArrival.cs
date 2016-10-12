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
            var rates = simState.Rates;
            var routes = simState.Routes;
            var timetable = simState.TimeTables;

            double newTime = StartTime;
            // int fillRate = 0;
            //int emptyRate = 0;
            //Tram has to wait until station is empty
            if (tram.Direction == Routes.Dir.ToPR)
            {
                if (station.TramIsStationedPR)
                {
                    Console.WriteLine("Tram " + _tramId + " enters the queue to PR");
                    station.WaitingTramsToPR.Enqueue(_tramId);
                    Console.WriteLine(station.WaitingTramsToPR.Count);
                }
                else
                {
                    station.TramIsStationedPR = true;
                    tram.State = Tram.TramState.AtStation;
                    tram.Station = _arrStation;
                    string pr = routes.CentralToPR[routes.CentralToPR.Count - 1].To;
                    if (_arrStation == pr)
                    {
                        timetable[_tramId].renewTimeTable(timetable[_tramId].totalTime);
                        newTime += 180;
                        tram.Direction = Routes.Dir.ToCS;
                    }

                    double emptyRate = rates.TramEmptyRate(_arrStation, tram.Direction, StartTime);
                    int outCount = tram.EmptyTram(emptyRate);
                    int inCount = tram.FillTram(station.WaitingPersonsToCS);

                    //van het interview
                    newTime += 12.5 + 0.22 * inCount + 0.13 * outCount;

                    //Add delay time if doors were shut
                    if (rates.DoorMalfunction()) { newTime += 60; }

                    Event e = new TramExpectedDeparture(_tramId, newTime, _arrStation);
                    simState.EventQueue.AddEvent(e);

                }
            }
            else
            {
                if (station.TramIsStationedCS)
                {
                    Console.WriteLine("Tram " + _tramId + " enters the queue to CS");
                    station.WaitingTramsToCS.Enqueue(_tramId);
                    Console.WriteLine(station.WaitingTramsToCS.Count);
                }
                else
                {
                    station.TramIsStationedCS = true;
                    tram.State = Tram.TramState.AtStation;
                    tram.Station = _arrStation;
                    string cs = routes.PRToCentral[routes.PRToCentral.Count - 1].To;
                    if (_arrStation == cs)
                    {

                        newTime += 180;
                        tram.Direction = Routes.Dir.ToPR;
                    }

                    double emptyRate = rates.TramEmptyRate(_arrStation, tram.Direction, StartTime);
                    int outCount = tram.EmptyTram(emptyRate);
                    int inCount = tram.FillTram(station.WaitingPersonsToCS);

                    //van het interview
                    newTime += 12.5 + 0.22 * inCount + 0.13 * outCount;

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
