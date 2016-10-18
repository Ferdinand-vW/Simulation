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

        public SimulationState run(bool debug, int tramFrequency, List<int> timeTable, DayOfWeek dayOfWeek, string[] stationNames)
        {
            SimulationState simState = Setup(tramFrequency, 6 * 60 * 60, stationNames,dayOfWeek);
            int n = 0;
            while (simState.EventQueue.HasEvent())
            {

                //debug
                var stations = simState.Stations;
                var trams = simState.Trams;
                var centralToPR = simState.Routes.CentralToPR;
                var prToCentral = simState.Routes.PRToCentral;
                var eventQueue = simState.EventQueue;
                if (debug)
                {
                    for (int i = 0; i < stations.Count; i++)
                    {
                        Station st = stations.Values.ToArray()[i];
                        Console.WriteLine("Station " + st.Name + ": " + st.WaitingPersonsToCS.Count
                                                               + " " + st.WaitingPersonsToPR.Count);
                    }
                    Console.WriteLine("From Central to PR");
                    for (int i = 0; i < centralToPR.Count; i++)
                    {
                        Track t = centralToPR[i];
                        Console.WriteLine("Track " + t.From + " to " + t.To);
                        t.Trams.ForEach(x => Console.WriteLine("\t tram: " + x));
                    }
                    Console.WriteLine("From PR to Central");
                    for (int i = 0; i < prToCentral.Count; i++)
                    {
                        Track t = prToCentral[i];
                        Console.WriteLine("Track " + t.From + " to " + t.To);
                        t.Trams.ForEach(x => Console.WriteLine("\t tram: " + x));
                    }
                    Console.WriteLine("Trams at stations:");
                    simState.Trams.Values.ToList().ForEach(x =>
                    {
                        if (x.State == Tram.TramState.AtStation)
                        {
                            Console.WriteLine(x.Station + " " + x.TramId);
                        }
                    });
                    Console.WriteLine("Trams waiting before station");
                    simState.Stations.Values.ToList().ForEach(x =>
                    {
                        if (x.WaitingTramsToCS.Count > 0)
                        {
                            var tramList = x.WaitingTramsToCS.Select(t => t.ToString())
                                                             .Aggregate((t1, t2) => t1 + " " + t2);
                            Console.WriteLine(x.Name + " to CS: " + tramList);
                        }
                        if (x.WaitingTramsToPR.Count > 0)
                        {
                            var tramList = x.WaitingTramsToPR.Select(t => t.ToString())
                                                             .Aggregate((t1, t2) => t1 + " " + t2);
                            Console.WriteLine

                            (x.Name + " to PR: " + tramList);
                        }
                    });
                    Console.WriteLine("Tram directions");
                    simState.Trams.ToList().ForEach(x =>
                    {
                        Console.WriteLine(x.Key + ": " + x.Value.Direction.ToString());
                    });
                    var eventQueueSorted = eventQueue.EventList.OrderBy(x => x.StartTime).ToList();

                    Console.WriteLine("Current scheduled events: ");
                    eventQueueSorted.ForEach(x => Console.WriteLine("\t event: " + x.ToString()));

                    Console.ReadLine();
                }

                Event e = simState.EventQueue.Next();

                //At 9PM we shut down the simulation
                if(e.StartTime > 22 * 60 * 60) { break; }


                e.execute(simState);
                //Console.WriteLine("Event " + n + ": " + e.ToString());
                n++;
            }

            return simState;
        }

        public SimulationState Setup(int tramFrequency, double startTime, string[] stationNames,DayOfWeek day)
        {
            var rates = new SimulationRates(a,b,day);

            EventQueue eventQueue = new EventQueue();
            var stations = new Dictionary<string, Station>();
            for (int i = 0; i < stationNames.Length; i++)
            {
                Station station = new Station(stationNames[i]);
                stations.Add(stationNames[i], station);

                //Only generate personarrival events for stations
                //where persons actually arrive
                if (a.EnteringTotal(day, stationNames[i]) != 0)
                {
                    eventQueue.AddEvent(new ZeroPersonArrival(startTime, stationNames[i], Routes.Dir.ToCS));
                }
                if (b.EnteringTotal(day, stationNames[i]) != 0)
                {
                    eventQueue.AddEvent(new ZeroPersonArrival(startTime, stationNames[i], Routes.Dir.ToPR));
                }
            }

            double minutesPerTram = (double)60 / tramFrequency;
            double totalRunTime = (17 + 17 + 4 + 4);
            int numberOfTrams = (int)(totalRunTime / minutesPerTram);
            var trams = new Dictionary<int, Tram>();
            Console.WriteLine("number of trams: {0}", numberOfTrams);
            for (int i = 0; i < numberOfTrams; i++)
            {
                trams[i] = new Tram(i, 0);
                trams[i].Station = stationNames[0];
                trams[i].State = Tram.TramState.AtShuntyard;
                eventQueue.AddEvent(new EnterTrack(i, startTime - (60 * 60) + (i * minutesPerTram * 60), stations.Values.ToArray()[0].Name));
            }


            var prToCentral = GenerateRoute(stationNames);
            var centralToPR = GenerateRoute(stationNames.Reverse().ToArray());

            var routes = new Routes(centralToPR, prToCentral);

            var timeTables = new Dictionary<int, TimeTable>();


            return new SimulationState(trams, stations, eventQueue, routes, rates, timeTables, day);
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
