﻿//Ferdinand van Walree, 3874389
//Rogier Wuijts, 

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TramSimulator.Events;
using TramSimulator.States;

namespace TramSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start reading passengercount data..");
            String patha = @"../../../a_data_updated.csv";
            String pathb = @"../../../b_data_updated.csv";

            Stream streama = File.Open(patha, FileMode.Open);
            Stream streamb = File.Open(pathb, FileMode.Open);

            List<PassengerCount> passengerCountsA = null;
            List<PassengerCount> passengerCountsB = null;

            var threadA = new Thread(() => { passengerCountsA = Parser.ParsePassengerCounts(streama); });
            threadA.Start();
            var threadB = new Thread(() => { passengerCountsB = Parser.ParsePassengerCounts(streamb); });
            threadB.Start();
            threadA.Join();
            threadB.Join();

            Func<double,double,double> div = (x, y) => x / y;

            Dictionary<string, double> enterPrognoseA = new Dictionary<string, double>();
            Dictionary<string, double> enterPrognoseB = new Dictionary<string, double>();
            Dictionary<string, double> exitPrognoseA = new Dictionary<string, double>();
            Dictionary<string, double> exitPrognoseB = new Dictionary<string, double>();
            enterPrognoseA["PR"] = 15;
            enterPrognoseA["WKZ"] = 1015;
            enterPrognoseA["UMC"] = 2660;
            enterPrognoseA["Heidelberglaan"] = 9138;
            enterPrognoseA["Padualaan"] = 6855;
            enterPrognoseA["Kromme Rijn"] = 691;
            enterPrognoseA["Galgenwaard"] = 606;
            enterPrognoseA["Vaartscherijn"] = 1261;
            enterPrognoseA["CS"] = 0;

            exitPrognoseA["PR"] = 0;
            exitPrognoseA["WKZ"] = 0;
            exitPrognoseA["UMC"] = 0;
            exitPrognoseA["Heidelberglaan"] = 0;
            exitPrognoseA["Padualaan"] = div(236,12827);
            exitPrognoseA["Kromme Rijn"] = div(31,19446);
            exitPrognoseA["Galgenwaard"] = div(265,20106);
            exitPrognoseA["Vaartscherijn"] = div(544,20447);
            exitPrognoseA["CS"] = div(21164,21164);

            enterPrognoseB["CS"] = 19994;
            enterPrognoseB["Vaartscherijn"] = 2337;
            enterPrognoseB["Galgenwaard"] = 359;
            enterPrognoseB["Kromme Rijn"] = 47;
            enterPrognoseB["Padualaan"] = 10;
            enterPrognoseB["Heidelberglaan"] = 8;
            enterPrognoseB["UMC"] = 6;
            enterPrognoseB["WKZ"] = 0;
            enterPrognoseB["PR"] = 0;

            exitPrognoseB["CS"] = 0;
            exitPrognoseB["Vaartscherijn"] = div(1735,19994);
            exitPrognoseB["Galgenwaard"] = div(1288,20596);
            exitPrognoseB["Kromme Rijn"] = div(1039,19667);
            exitPrognoseB["Padualaan"] = div(9672,18674);
            exitPrognoseB["Heidelberglaan"] = div(5789,9011);
            exitPrognoseB["UMC"] = div(2577,3230);
            exitPrognoseB["WKZ"] = div(644,659);
            exitPrognoseB["PR"] = 1;

            Data a = new Data(enterPrognoseA, exitPrognoseA);
            Data b = new Data(enterPrognoseB, exitPrognoseB);
            passengerCountsA.ForEach(x => a.AddPC(x));
            passengerCountsB.ForEach(x => b.AddPC(x));
            string[] stations = enterPrognoseA.Keys.ToArray();

            Stream f1 = File.Open("../../input-data-passengers-01.csv", FileMode.Open);
            ArtInput f1A = Parser.ParseArtInput(f1, stations);
            Console.WriteLine(f1A.PRCS.Count);
            Stream f15 = File.Open("../../input-data-passengers-015.csv", FileMode.Open);
            ArtInput f15A = Parser.ParseArtInput(f15, stations);
            Stream f2 = File.Open("../../input-data-passengers-02.csv", FileMode.Open);
            ArtInput f2A = Parser.ParseArtInput(f2, stations);
            Stream f25 = File.Open("../../input-data-passengers-025.csv", FileMode.Open);
            ArtInput f25A = Parser.ParseArtInput(f25, stations);
            Stream f3 = File.Open("../../input-data-passengers-03.csv", FileMode.Open);
            ArtInput f3A = Parser.ParseArtInput(f3, stations);
            Stream f4 = File.Open("../../input-data-passengers-04.csv", FileMode.Open);
            ArtInput f4A = Parser.ParseArtInput(f4, stations);
            Stream f6 = File.Open("../../input-data-passengers-06.csv", FileMode.Open);
            ArtInput f6A = Parser.ParseArtInput(f6, stations);

            Console.WriteLine("Finished reading passengercount data");

            RunSimulation("realistic.txt", new SimulationRates(a, b, DayOfWeek.Wednesday), stations);
            /*RunSimulation("File1.txt", new ArtificialRates(f1A), stations);
            RunSimulation("File15.txt", new ArtificialRates(f15A), stations);
            RunSimulation("File2.txt", new ArtificialRates(f2A), stations);
            RunSimulation("File25.txt", new ArtificialRates(f25A), stations);
            RunSimulation("File3.txt", new ArtificialRates(f3A), stations);
            RunSimulation("File4.txt", new ArtificialRates(f4A), stations);
            RunSimulation("File6.txt", new ArtificialRates(f6A), stations);*/


            Console.ReadLine();
        }

        public static void RunSimulation(string filename, AbstractSimulationRates rates, string[] stationsArr)
        {
            Simulation sim = new Simulation();
            //debug, frequency, turnaroundtime, day, stations

            List<double> csMaxDelays = new List<double>();
            List<double> prMaxDelays = new List<double>();
            List<double> csAvgDelays = new List<double>();
            List<double> prAvgDelays = new List<double>();
            List<double> prcDelaysOverOneMinute = new List<double>();
            List<double> maxWaitTimes = new List<double>();
            List<double> avgWaitTimes = new List<double>();
            List<double> maxTravelTimes = new List<double>();
            List<double> avgTravelTimes = new List<double>();
            List<double> csMaxQueueSizes = new List<double>();
            List<double> prMaxQueueSizes = new List<double>();
            Console.WriteLine("Start simulation " + filename);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Start run " + (i + 1));
                var results = sim.run(false, 12, 240, DayOfWeek.Wednesday, stationsArr, rates);
                var trams = results.Trams;
                var timetable = results.TimeTable;
                var persons = results.Persons.Values.ToList();
                var stations = results.Stations;

                //Maximum delay of a tram
                csMaxDelays.Add(timetable.CSMaxDelay);
                prMaxDelays.Add(timetable.PRMaxDelay);
                //Average total delay of a tram
                csAvgDelays.Add(timetable.CSAverageDelay);
                prAvgDelays.Add(timetable.PRAverageDelay);
                //Percentage delays over one minute
                prcDelaysOverOneMinute.Add((double)timetable.DelaysOverOneMinute / timetable.NumberOfRounds);
                //Max waiting time of a person
                maxWaitTimes.Add(persons.Max(x => x.WaitingTime));
                //Average waiting time of a person
                avgWaitTimes.Add(persons.Sum(x => x.WaitingTime) / persons.Count);
                //Maximum travel time of a person
                maxTravelTimes.Add(persons.Max(x => x.LeaveTime - x.ArrivalTime));
                //Average travel time of a person
                avgTravelTimes.Add(persons.Sum(x => x.LeaveTime - x.ArrivalTime) / persons.Count);
                //Maximum queue length
                csMaxQueueSizes.Add(stations.Values.ToList().Max(x => x.MaxQueueLengthCS));
                prMaxQueueSizes.Add(stations.Values.ToList().Max(x => x.MaxQueueLengthPR));
            }

            Console.WriteLine("Simulation completed. Start writing output");
            Stream f = File.Create(filename);
            using (StreamWriter sw = new StreamWriter(f))
            {

                sw.WriteLine(String.Join(",", csMaxDelays.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", prMaxDelays.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", csAvgDelays.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", prAvgDelays.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", prcDelaysOverOneMinute.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", maxWaitTimes.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", avgWaitTimes.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", maxTravelTimes.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", avgTravelTimes.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", csMaxQueueSizes.Select(x => x.ToString())));
                sw.WriteLine(String.Join(",", prMaxQueueSizes.Select(x => x.ToString())));
            }
            Console.WriteLine("Output has been written. Simulation over...");
        }
    }
}
