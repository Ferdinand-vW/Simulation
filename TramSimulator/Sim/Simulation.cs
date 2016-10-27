using System;
using System.Collections.Generic;
using System.Linq;

using TramSimulator.Events;
using TramSimulator.States;
using TramSimulator.Rates;

namespace TramSimulator.Sim
{

    public class Simulation
    {
        int i = 0;

        public SimulationState run(bool debug, int tramFrequency, int turnAroundTime, DayOfWeek dayOfWeek, string[] stationNames, AbstractSimulationRates rates)
        {
            //Setup up the simulation and create an initial state
            SimulationState simState = Setup(tramFrequency, turnAroundTime, Constants.BEGIN_TIME, stationNames,dayOfWeek, rates);

            Event e = null;
            //Start the simulation loop
            while (simState.EventQueue.HasEvent())
            {

                var stations = simState.Stations;
                var trams = simState.Trams;
                var centralToPR = simState.Routes.CentralToPR;
                var prToCentral = simState.Routes.PRToCentral;
                var eventQueue = simState.EventQueue;

                //Debug printing
                if (debug && i != 0 && e.EType != Event.EventType.Other)
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
                    
                    
                    Console.ReadLine();
                }

                //Retrieve the next event
                e = simState.EventQueue.Next();
                i++;
                

                //At 9:30PM we shut down the simulation
                if(e.StartTime > Constants.END_TIME) { break; }
                //Execute the event
                e.Execute(simState);

            }

            return simState;
        }

        public SimulationState Setup(int tramFrequency, int turnAroundTime, double startTime, string[] stationNames,DayOfWeek day, AbstractSimulationRates rates)
        {
            //Initialize
            var eventQueue = new EventQueue();
            var stations = new Dictionary<string, Station>();
            for (int i = 0; i < stationNames.Length; i++)
            {
                //Generate a station
                Station station = new Station(stationNames[i]);
                stations.Add(stationNames[i], station);

                //Only generate personarrival events for stations
                //where persons actually arrive
                eventQueue.AddEvent(new ZeroPersonArrival(startTime, stationNames[i], Routes.Dir.ToCS));
                eventQueue.AddEvent(new ZeroPersonArrival(startTime, stationNames[i], Routes.Dir.ToPR));
            }

            //Determine how many trams are needed up uphold the given frequency
            double secondsPerTram = (double)Constants.SECONDS_IN_HOUR / tramFrequency;
            double totalRunTime = Constants.ONE_WAY_DRIVING_TIME * 2 + 2 * turnAroundTime;
            int numberOfTrams = (int)(totalRunTime / secondsPerTram);
            var trams = new Dictionary<int, Tram>();
            var departureTimes = new double[numberOfTrams];
            for (int i = 0; i < numberOfTrams; i++)
            {
                //Make a tram object and generate an initial event for it
                trams[i] = new Tram(i, 0);
                trams[i].Station = stationNames[0];
                trams[i].State = Tram.TramState.AtShuntyard;
                //initial departure time for a given tram
                departureTimes[i] = secondsPerTram * i + Constants.BEGIN_TIME;
                eventQueue.AddEvent(new EnterTrack(i, stations.Values.ToArray()[0].Name, departureTimes[i]));
            }

            //Create the routes CS-PR and PR-CS
            var prToCentral = GenerateRoute(stationNames);
            //All trams wil initially be set in the shuntyard
            prToCentral.Insert(0, new Track(Constants.SHUNTYARD, Constants.PR));
            var centralToPR = GenerateRoute(stationNames.Reverse().ToArray());

            var routes = new Routes(centralToPR, prToCentral);
            var timeTable = new TimeTable(startTime, turnAroundTime, trams.Keys.ToArray(), departureTimes);

            return new SimulationState(trams, stations, eventQueue, routes, rates, timeTable, day);
        }

        static public List<Track> GenerateRoute(string[] stationNames)
        {
            List<Track> route = new List<Track>();
            string from = stationNames[0];
            //Fill a route with tracks
            for (int i = 1; i < stationNames.Length; i++)
            {
                route.Add(new Track(from, stationNames[i]));
                from = stationNames[i];
            }

            return route;
        }

    }
}
