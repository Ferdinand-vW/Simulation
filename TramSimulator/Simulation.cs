using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using TramSimulator.Events;
using TramSimulator.States;

namespace TramSimulator
{

    public class Simulation
    {
        Data a;
        Data b;
        int i = 0;

        public Simulation(Data a, Data b)
        {
            this.a = a;
            this.b = b;
        }

        public SimulationState run(bool debug, int tramFrequency, int turnAroundTime, DayOfWeek dayOfWeek, string[] stationNames)
        {
            SimulationState simState = Setup(tramFrequency, turnAroundTime, Constants.BEGIN_TIME, stationNames,dayOfWeek);
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
                    for (int i = 0; i < centralToPR.Count; i++)
                    {
                        Track t = centralToPR[i];
                        Console.Write("Track " + t.From + " to " + t.To + ":");
                        t.Trams.ForEach(x => Console.Write(x + "  "));
                        Console.WriteLine("");
                    }
                    for (int i = 0; i < prToCentral.Count; i++)
                    {
                        Track t = prToCentral[i];
                        Console.Write("Track " + t.From + " to " + t.To + ":");
                        t.Trams.ForEach(x => Console.Write(x + "  "));
                        Console.WriteLine("");
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
                var tracks2 = simState.Routes.CentralToPR.Union(simState.Routes.PRToCentral).ToList();
                i++;
                

                //At 9PM we shut down the simulation
                if(e.StartTime > Constants.END_TIME) { break; }
                //simState.Stations["CS"].WaitingTramsToCS.ToList().ForEach(x => Console.WriteLine(x));
                if (e.GetType() != typeof(PersonArrival))
                {
                    simState.sw.WriteLine("Event " + n + ": " + e.ToString() + " " + simState.Stations["PR"].TramAtCS.HasValue + " " + simState.Stations["PR"].TramAtPR.HasValue
                    + " " + simState.Stations["CS"].TramAtCS.HasValue + " " + simState.Stations["CS"].TramAtPR.HasValue + " ID: ");
                    simState.Routes.CentralToPR.ForEach(x =>
                    {
                        simState.sw.Write(x.From + " " + x.To + ": ");
                        x.Trams.ForEach(y => simState.sw.Write(y + " "));
                        simState.sw.WriteLine();
                    });
                    simState.Routes.PRToCentral.ForEach(x =>
                    {
                        simState.sw.Write(x.From + " " + x.To + ": ");
                        x.Trams.ForEach(y => simState.sw.Write(y + " "));
                        simState.sw.WriteLine();
                    });
                    simState.Trams.Values.ToList().ForEach(x =>
                    {
                        if(x.State == Tram.TramState.AtStation)
                        {
                            simState.sw.WriteLine(x.Station + " " + x.TramId);
                        }
                    });
                    simState.sw.WriteLine();
                    simState.Stations.Values.ToList().ForEach(x => x.PrintQueues(simState));
                }
                if(simState.Stations["PR"].EnterTrackQueue.Count > 0)
                {
                    //simState.sw.Close();
                    
                    Console.WriteLine("test");
                }
                e.execute(simState);
                
                n++;
            }

            simState.sw.Close();

            return simState;
        }

        public SimulationState Setup(int tramFrequency, int turnAroundTime, double startTime, string[] stationNames,DayOfWeek day)
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

            double secondsPerTram = (double)Constants.SECONDS_IN_HOUR / tramFrequency;
            double totalRunTime = Constants.ONE_WAY_DRIVING_TIME * 2 + 2 * turnAroundTime;
            int numberOfTrams = (int)(totalRunTime / secondsPerTram);
            var trams = new Dictionary<int, Tram>();
            Console.WriteLine("number of trams: {0}", numberOfTrams);
            var departureTimes = new double[numberOfTrams];
            for (int i = 0; i < numberOfTrams; i++)
            {
                trams[i] = new Tram(i, 0);
                trams[i].Station = stationNames[0];
                trams[i].State = Tram.TramState.AtShuntyard;
                departureTimes[i] = secondsPerTram * i + Constants.BEGIN_TIME - Constants.SECONDS_IN_HOUR;
                eventQueue.AddEvent(new EnterTrack(i, stations.Values.ToArray()[0].Name, departureTimes[i]));
            }


            var prToCentral = GenerateRoute(stationNames);
            prToCentral.Insert(0, new Track(Constants.SHUNTYARD, Constants.PR));
            var centralToPR = GenerateRoute(stationNames.Reverse().ToArray());

            var routes = new Routes(centralToPR, prToCentral);

            var timeTable = new TimeTable(startTime, turnAroundTime, trams.Keys.ToArray(), departureTimes);

            Stream f = File.Create("test.txt");
            StreamWriter sw = new StreamWriter(f);
            return new SimulationState(trams, stations, eventQueue, routes, rates, timeTable, day, sw);
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
