using System;
using System.Collections.Generic;

using TramSimulator.States;
using TramSimulator.Rates;

namespace TramSimulator.Sim
{
    public class SimulationState
    {
        public Dictionary<int, Tram> Trams { get; private set; }
        public Dictionary<string, Station> Stations { get; private set; }
        public Dictionary<int, Person> Persons { get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Routes Routes { get; set; }
        public AbstractSimulationRates Rates {get;set;}
        public TimeTable TimeTable { get; set; }
        public DayOfWeek Day { get; set; }

        public SimulationState(Dictionary<int, Tram> trams, Dictionary<string, Station> stations, 
                               EventQueue eventQueue, Routes routes, AbstractSimulationRates rates, 
                               TimeTable timeTable, DayOfWeek day)
        {
            this.Trams = trams;
            this.Stations = stations;
            this.Persons = new Dictionary<int, Person>();
            this.EventQueue = eventQueue;
            this.Routes = routes;
            this.Rates = rates;
            this.TimeTable = timeTable;
            this.Day = day;
        }
    }
}
