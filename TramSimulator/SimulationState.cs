﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;
using TramSimulator.Events;

namespace TramSimulator
{
    public class SimulationState
    {
        public Dictionary<int, Tram> Trams { get; private set; }
        public Dictionary<string, Station> Stations { get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Routes Routes { get; set; }
        public SimulationRates Rates {get;set;}
        public Dictionary<int, TimeTable> TimeTables { get; set; }

        public SimulationState(Dictionary<int, Tram> trams, Dictionary<string, Station> stations, 
                               EventQueue eventQueue, Routes routes, SimulationRates rates, Dictionary<int, TimeTable> timeTables)
        {
            this.Trams = trams;
            this.Stations = stations;
            this.EventQueue = eventQueue;
            this.Routes = routes;
            this.Rates = rates;
            this.TimeTables = timeTables;
        }
    }
}
