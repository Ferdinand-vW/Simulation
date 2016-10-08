using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.Events;
using TramSimulator.States;

namespace TramSimulator
{

    public class Simulation
    {
        Data a;
        Data b;

        public Simulation(Data a, Data b)
        {
            this.a = a;
            this.b = b;
        }

        public void run(int tramFrequency, List<int> timeTable, DayOfWeek dayOfWeek, string[] stationNames)
        {
            SimulationState simState = Setup(tramFrequency, 6 * 60 * 60, stationNames,dayOfWeek);
            while (simState.EventQueue.HasEvent())
            {

                //debug
                var stations = simState.Stations;
                var trams = simState.Trams;
                var centralToPR = simState.Routes.CentralToPR;
                var prToCentral = simState.Routes.PRToCentral;
                var eventQueue = simState.EventQueue;
                for (int i = 0; i < stations.Count; i++)
                {
                    Station st = stations.Values.ToArray()[i];
                    Console.WriteLine("StationA : {0} WaitingP : {1} WaitingT : {2} ",st.Name,st.WaitingPersonsA.Count,st.WaitingTramsA.Count);
                    Console.WriteLine("StationB : {0} WaitingP : {1} WaitingT : {2} ", st.Name, st.WaitingPersonsB.Count, st.WaitingTramsB.Count);
                }
                //Console.WriteLine("From Central to PR");
                for (int i = 0; i < centralToPR.Count; i++)
                {
                    Track t = centralToPR[i];
                    Console.WriteLine("Track " + t.From + " to " + t.To);
                    t.Trams.ForEach(x => Console.WriteLine("\t tram: " + x));
                }
                //Console.WriteLine("From PR to Central");
                for (int i = 0; i < prToCentral.Count; i++)
                {
                    Track t = prToCentral[i];
                    Console.WriteLine("Track " + t.From + " to " + t.To);
                    t.Trams.ForEach(x => Console.WriteLine("\t tram: " + x));
                }
                Console.WriteLine("Trams at stations:");
                simState.Trams.Values.ToList().ForEach(x => 
                {
                    if(x.State == Tram.TramState.AtStation)
                    {
                        Console.WriteLine(x.Station + " " + x.TramId);
                    }
                });
                var eventQueueSorted = eventQueue.EventList.OrderBy(x => x.StartTime).ToList();

                Console.WriteLine("Current scheduled events: ");
                eventQueueSorted.ForEach(x => Console.WriteLine("\t event: " + x.ToString()));


                Event e = simState.EventQueue.Next();
                e.execute(simState);
                if(! e.GetType().Equals(typeof(PersonArrival)))
                Console.ReadLine();
                Console.WriteLine(e.ToString());
            }
        }

        public SimulationState Setup(int tramFrequency, double startTime, string[] stationNames, DayOfWeek day)
        {
            var rates = new SimulationRates(a, b, day);

            EventQueue eventQueue = new EventQueue();
            var stations = new Dictionary<string, Station>();
            for (int i = 0; i < stationNames.Length; i++)
            {
                Station station = new Station(stationNames[i]);
                stations.Add(stationNames[i], station);
                eventQueue.AddEvent(new ZeroPersonArrival(startTime, stationNames[i], true));
                eventQueue.AddEvent(new ZeroPersonArrival(startTime, stationNames[i], false));
            }

            var trams = new Dictionary<int, Tram>();
            for (int i = 0; i < tramFrequency; i++)
            {
                trams[i] = new Tram(i, 0);
                trams[i].Station = stationNames[stationNames.Length - 1];
                trams[i].State = Tram.TramState.AtShuntyard;
                eventQueue.AddEvent(new EnterTrack(i, startTime + (i * 5 * 60), stations.Values.ToArray()[stationNames.Length - 1].Name));
            }

            var centralToPR = GenerateRoute(stationNames);
            var prToCentral = GenerateRoute(stationNames.Reverse().ToArray());

            var routes = new Routes(centralToPR, prToCentral);

            var timeTables = new Dictionary<int, TimeTable>();


            return new SimulationState(trams, stations, eventQueue, routes, rates, timeTables);
        }

        static public List<Track> GenerateRoute(string[] stationNames)
        {
            List<Track> route = new List<Track>();
            string from = stationNames[0];
            for (int i = 1; i < stationNames.Length; i++)
            {
                route.Add(new Track(from, stationNames[i]));
                from = stationNames[i];
            }

            return route;
        }
        
        
    }

}
