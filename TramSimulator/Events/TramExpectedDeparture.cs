using System;
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
        //int _tramId;
        string _depStation;
        public static int rounds;
        public static int delayed;

        public TramExpectedDeparture(int tramId, string depStation, double startTime)
        {
            this._tramId = tramId;
            this.StartTime = startTime;
            this._depStation = depStation;
        }

        public override void execute(SimulationState simState)
        {
            var eventQueue = simState.EventQueue;
            var tram = simState.Trams[_tramId];
            var station = simState.Stations[_depStation];

            if (_depStation != Constants.PR && _depStation != Constants.CS)
            {
                if (tram.Direction == Routes.Dir.ToCS)
                {
                    station.TramAtCS = _tramId;
                    station.TramIsStationedCS = true;
                }
                else
                {
                    station.TramAtPR = _tramId;
                    station.TramIsStationedPR = true;
                }
            }

            tram.State = Tram.TramState.AtStation;
            tram.Station = _depStation;

            Track t = simState.Routes.GetNextTrack(_depStation, simState.Routes.NextStation(_tramId, _depStation));
            //Find the next tram
            int? id = simState.Routes.NextTram(t, _tramId, _depStation, simState);
            simState.sw.WriteLine("Next tram id: " + id);
            double currTime = StartTime;
            string central = simState.Routes.CentralToPR[0].From;
            string pr = simState.Routes.PRToCentral[0].From;

            if (id.HasValue) //If there exists a tram on the same track
            {
                Tram nextTram = simState.Trams[id.Value];
                //Tram may only depart if there is 40s between the next tram and this one
                simState.sw.WriteLine("time: " + currTime + " " + nextTram.DepartureTime);
                if (Math.Ceiling(currTime - nextTram.DepartureTime) >= Constants.TIME_IN_BETWEEN)
                {
                    HandleDepartureEvent(simState);
                }
                else //Tram was not able to depart and has to schedule new departure
                {
                    double timeDiff = Constants.TIME_IN_BETWEEN - (Math.Ceiling(currTime - nextTram.DepartureTime));
                    delayed++;
                    eventQueue.AddEvent(new TramExpectedDeparture(_tramId, _depStation, StartTime + timeDiff));
                }
            }
            else
            {
                simState.sw.WriteLine(t.From + " " + t.To + ": ");
                t.Trams.ForEach(x =>
                {
                    simState.sw.Write(x);
                });
                simState.sw.WriteLine("");
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
            var arrTime = StartTime + simState.Rates.TramArrivalRate(_depStation, nextStation);

            //Remember the time that this tram left his last station
            tram.DepartureTime = StartTime;
            //Handle departure of a tram
            tram.State = Tram.TramState.OnTrack;
            simState.Routes.MoveToNextTrack(_tramId, _depStation, simState);

            eventQueue.AddEvent(new TramExpectedArrival(_tramId, arrTime, nextStation));

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
                    station.lastTramCS = _tramId;
                    station.TramIsStationedCS = false;
                    station.TramAtCS = null;
                    if (station.WaitingTramsToCS.Count > 0) { nextTramId = station.WaitingTramsToCS.Peek(); }
                }
                else
                {
                    station.lastTramPR = _tramId;
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
            if (_depStation == Constants.PR && station.EnterTrackQueue.Count > 0)
            {
                nextTramId = station.EnterTrackQueue.Dequeue();
                eventQueue.AddEvent(new EnterTrack(nextTramId, _depStation, StartTime));
            }
            else
            {
                if (waitingtrams.Count > 0) { nextTramId = waitingtrams.Peek(); }
                if (nextTramId != -1) { eventQueue.AddEvent(new TramExpectedArrival(nextTramId, StartTime, _depStation)); }
            }
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " expected departure " + StartTime + " from " + _depStation;
        }
    }
}
