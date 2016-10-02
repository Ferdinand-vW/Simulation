﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    public class EnterTrack : Event
    {
        int _tramId;
        string _station;

        public EnterTrack(int tramId, double startTime, string station)
        {
            this._tramId = tramId;
            this._station = station;
            this.StartTime = startTime;
        }

        public override void execute(SimulationState simState)
        {
            //Insert the tram into the route
            var route = simState.Routes.PRToCentral;
            route[0].Trams.Add(_tramId);
            var tram = simState.Trams[_tramId];
            tram.State = Tram.TramState.AtStation;
            //Add an initial event
            var eventQueue = simState.EventQueue;

            simState.TimeTables[_tramId] = new TimeTable(simState.Rates,simState.Routes,StartTime);
            eventQueue.AddEvent(new TramExpectedDeparture(_tramId, StartTime, _station));
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " enters " + _station + " at " + StartTime;
        }
    }
}
