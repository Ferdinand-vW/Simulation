using System;
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
        public Dictionary<int, Person> Persons { get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Routes Routes { get; set; }
        public SimulationRates Rates {get;set;}
        public Dictionary<int, TimeTable> TimeTables { get; set; }
        public DayOfWeek Day { get; set; }
        public Queue<int> turnAroundPR;
        public Queue<int> turnAroundCS;
        public SimulationState(Dictionary<int, Tram> trams, Dictionary<string, Station> stations, 
                               EventQueue eventQueue, Routes routes, SimulationRates rates, 
                               Dictionary<int, TimeTable> timeTables, DayOfWeek day)
        {
            this.Trams = trams;
            this.Stations = stations;
            this.Persons = new Dictionary<int, Person>();
            this.EventQueue = eventQueue;
            this.Routes = routes;
            this.Rates = rates;
            this.TimeTables = timeTables;
            this.Day = day;
            turnAroundCS = new Queue<int>();
            
            turnAroundPR = new Queue<int>();
        }
    }
}
