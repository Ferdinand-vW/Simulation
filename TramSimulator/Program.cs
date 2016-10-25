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
            String patha = @"../../a_data_updated.csv";
            String pathb = @"../../b_data_updated.csv";

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

            Func<double, double, double> div = (x, y) => x / y;
            string[] stations = new string[] { "PR", "WKZ", "UMC", "Heidelberglaan", "Padualaan", "Kromme Rijn", "Galgenwaard", "Vaartscherijn", "CS" };


            //Stream f1 = File.Open(@"F:\Users\Rogier\Source\Repos\Simulation2\TramSimulator\input-data-passengers-01.csv", FileMode.Open);
            //ArtInput f1A = Parser.ParseArtInput(f1, stations);
            //Console.WriteLine(f1A.PRCS.Count);
            //Stream f15 = File.Open(@"F:\Users\Rogier\Source\Repos\Simulation2\TramSimulator\input-data-passengers-015.csv", FileMode.Open);
            //ArtInput f15A = Parser.ParseArtInput(f15, stations);
            //Stream f2 = File.Open(@"F:\Users\Rogier\Source\Repos\Simulation2\TramSimulator\input-data-passengers-02.csv", FileMode.Open);
            //ArtInput f2A = Parser.ParseArtInput(f2, stations);
            //Stream f25 = File.Open(@"F:\Users\Rogier\Source\Repos\Simulation2\TramSimulator\input-data-passengers-025.csv", FileMode.Open);
            //ArtInput f25A = Parser.ParseArtInput(f25, stations);
            //Stream f3 = File.Open(@"F:\Users\Rogier\Source\Repos\Simulation2\TramSimulator\input-data-passengers-03.csv", FileMode.Open);
            //ArtInput f3A = Parser.ParseArtInput(f3, stations);
            //Stream f4 = File.Open(@"F:\Users\Rogier\Source\Repos\Simulation2\TramSimulator\input-data-passengers-04.csv", FileMode.Open);
            //ArtInput f4A = Parser.ParseArtInput(f4, stations);
            //Stream f6 = File.Open(@"F:\Users\Rogier\Source\Repos\Simulation2\TramSimulator\input-data-passengers-06.csv", FileMode.Open);
            //ArtInput f6A = Parser.ParseArtInput(f6, stations);

            Console.WriteLine("Finished reading passengercount data");



            //var results_file1 = RunSimulation("File1.txt", new ArtificialRates(f1A), stations);
            //outputRun(results_file1, "File1");
            //var results_file15 = RunSimulation("File15.txt", new ArtificialRates(f15A), stations);
            //outputRun(results_file15, "File15");
            //var results_file2 = RunSimulation("File2.txt", new ArtificialRates(f2A), stations);
            //outputRun(results_file2, "File2");
            //var results_file25 = RunSimulation("File25.txt", new ArtificialRates(f25A), stations);
            //outputRun(results_file25, "File25");
            //var results_file3 = RunSimulation("File3.txt", new ArtificialRates(f3A), stations);
            //outputRun(results_file3, "File3");
            //var results_file4 = RunSimulation("File4.txt", new ArtificialRates(f4A), stations);
            //outputRun(results_file4, "File4");
            //var results_file6 = RunSimulation("File6.txt", new ArtificialRates(f6A), stations);
            //outputRun(results_file6, "File6");


            for (int i = 1; i <= 6; i++) {
                double c = (double)i / 2;
                Data a = dataA(c, passengerCountsA);
                Data b = dataB(c, passengerCountsB);
                var results_realistic = RunSimulation("realistic.txt", new SimulationRates(a, b, DayOfWeek.Wednesday), stations);
                outputRun(results_realistic, "c=" + i );
            }



        }

        public static Data dataA(double c, List<PassengerCount> passengerCountsA) {
            Func<double, double, double> div = (x, y) => x / y;
            Dictionary<string, double> enterPrognoseA = new Dictionary<string, double>();

            Dictionary<string, double> exitPrognoseA = new Dictionary<string, double>();

            enterPrognoseA["PR"] = 15 * c;
            enterPrognoseA["WKZ"] = 1015 * c;
            enterPrognoseA["UMC"] = 2660 * c;
            enterPrognoseA["Heidelberglaan"] = 9138 * c;
            enterPrognoseA["Padualaan"] = 6855 * c;
            enterPrognoseA["Kromme Rijn"] = 691 * c;
            enterPrognoseA["Galgenwaard"] = 606 * c;
            enterPrognoseA["Vaartscherijn"] = 1261 * c;
            enterPrognoseA["CS"] = 0 * c;

            exitPrognoseA["PR"] = 0;
            exitPrognoseA["WKZ"] = 0;
            exitPrognoseA["UMC"] = 0;
            exitPrognoseA["Heidelberglaan"] = 0;
            exitPrognoseA["Padualaan"] = div(236, 12827);
            exitPrognoseA["Kromme Rijn"] = div(31, 19446);
            exitPrognoseA["Galgenwaard"] = div(265, 20106);
            exitPrognoseA["Vaartscherijn"] = div(544, 20447);
            exitPrognoseA["CS"] = div(21164, 21164);


            Data a = new Data(enterPrognoseA, exitPrognoseA);
            passengerCountsA.ForEach(x => a.AddPC(x));
            return a;
        }
        public static Data dataB(double c, List<PassengerCount> passengerCountsB)
        {
            Func<double, double, double> div = (x, y) => x / y;
            Dictionary<string, double> enterPrognoseB = new Dictionary<string, double>();
            Dictionary<string, double> exitPrognoseB = new Dictionary<string, double>();
            enterPrognoseB["CS"] = 19994 * c;
            enterPrognoseB["Vaartscherijn"] = 2337 * c;
            enterPrognoseB["Galgenwaard"] = 359 * c;
            enterPrognoseB["Kromme Rijn"] = 47 * c;
            enterPrognoseB["Padualaan"] = 10 * c;
            enterPrognoseB["Heidelberglaan"] = 8 * c;
            enterPrognoseB["UMC"] = 6 * c;
            enterPrognoseB["WKZ"] = 0 * c;
            enterPrognoseB["PR"] = 0 * c;

            exitPrognoseB["CS"] = 0;
            exitPrognoseB["Vaartscherijn"] = div(1735, 19994);
            exitPrognoseB["Galgenwaard"] = div(1288, 20596);
            exitPrognoseB["Kromme Rijn"] = div(1039, 19667);
            exitPrognoseB["Padualaan"] = div(9672, 18674);
            exitPrognoseB["Heidelberglaan"] = div(5789, 9011);
            exitPrognoseB["UMC"] = div(2577, 3230);
            exitPrognoseB["WKZ"] = div(644, 659);
            exitPrognoseB["PR"] = 1;



            Data b = new Data(enterPrognoseB, exitPrognoseB);
            passengerCountsB.ForEach(x => b.AddPC(x));
            return b;
        }
        public static void outputRun(Dictionary<string, Dictionary<string, List<double>>> results_realistic, string fileNameExt)
        {
            /*
            foreach (var perf in results_realistic.Values.ToList()[0].Keys)
            {
                Stream s = File.Create(perf + fileNameExt + ".csv");
                using (StreamWriter sw = new StreamWriter(s))
                {
                    var matrix = new List<string>();

                    matrix.Add(";" + String.Join(";", results_realistic.Keys));

                    foreach (var kvp in results_realistic)
                    {
                        var row = new List<string>();
                        row.Add(kvp.Key);
                        foreach (var kvp2 in results_realistic)
                        {
                            var pf1 = kvp.Value[perf];
                            var pf2 = kvp2.Value[perf];
                            var conf = ConfidenceInterval(pf1, pf2);

                            row.Add(String.Join(":", conf));
                        }

                        matrix.Add(String.Join(";", row));
                    }

                    matrix.ForEach(x => sw.WriteLine(x));
                }
            }
            */
            Console.WriteLine(fileNameExt);
            // alle runs per configuratie
            foreach (var kvp in results_realistic)
            {
                Stream s = File.Create(kvp.Key + fileNameExt + ".csv");
                var results = kvp.Value;
                using (StreamWriter sw = new StreamWriter(s))
                {
                    var matrix = new List<string>();
                    string[] runs = new string[results.Values.ToList()[0].Count];
                    for (int i = 0; i < runs.Length; i++)
                    {
                        runs[i] = i.ToString();
                    }
                    matrix.Add(";" + String.Join(";", runs));

                    foreach (var perf in results_realistic.Values.ToList()[0].Keys)
                    {
                        var row = new List<string>();
                        row.Add(perf);
                        var rofOfValues = results[perf];
                        rofOfValues.ForEach(x => row.Add(Math.Round(x, 3).ToString()));
                        matrix.Add(String.Join(";", row));
                    }

                    matrix.ForEach(x => sw.WriteLine(x));
                }
            }

            //Alle gemiddelde van alle configuraties

            Stream ss = File.Create("Alles" + fileNameExt + ".csv");

            using (StreamWriter sw = new StreamWriter(ss))
            {
                var matrix = new List<string>();

                matrix.Add(";" + String.Join(";", results_realistic.Keys.ToList()));




                foreach (var perf in results_realistic.Values.ToList()[0].Keys)
                {
                    var row = new List<string>();
                    row.Add(perf);
                    foreach (var kvp in results_realistic)
                    {
                        var results = kvp.Value;
                        var list = results[perf];
                        row.Add(String.Join(":", ConfidenceIntervalSingle(list)));
                    }
                    matrix.Add(String.Join(";", row));
                }
                matrix.ForEach(x => sw.WriteLine(x));
            }

            Console.WriteLine(fileNameExt + " is done");
        }

        public static Dictionary<string, Dictionary<string, List<double>>> RunSimulation(string filename, AbstractSimulationRates rates, string[] stationsArr)
        {
            Simulation sim = new Simulation();
            //debug, frequency, turnaroundtime, day, stations

            var configResults = new Dictionary<string, Dictionary<string, List<double>>>();

            Console.WriteLine("Start simulation " + filename);

            var configs = new List<List<double>>{
                //new List<double> { 16, 4},
                //new List<double> { 16, 5},
                new List<double> { 16, 6},
                //new List<double> { 18, 4},
                //new List<double> { 18, 5},
                //new List<double> { 18, 6},
                //new List<double> { 20, 4},
                //new List<double> { 20, 5},
                new List<double> { 20, 6},
                //new List<double> { 22, 4},
                //new List<double> { 22, 5},
                //new List<double> { 22, 6},
                //new List<double> { 24, 4},
                //new List<double> { 24, 5},
                new List<double> { 24, 6}};

            for (int j = 0; j < configs.Count; j++)
            {
                var performM = new Dictionary<string, List<double>>();
                performM.Add("CSMaxDelays", new List<double>());
                performM.Add("PRMaxDelays", new List<double>());
                performM.Add("CSAvgDelays", new List<double>());
                performM.Add("PRAvgDelays", new List<double>());
                performM.Add("PrcOverOneMinuteDelay", new List<double>());
                performM.Add("MaxWaitTimes", new List<double>());
                performM.Add("AvgWaitTimes", new List<double>());
                performM.Add("MaxTravelTimes", new List<double>());
                performM.Add("AvgTravelTimes", new List<double>());
                performM.Add("CSMaxQueueSizes", new List<double>());
                performM.Add("PRMaxQueueSizes", new List<double>());

                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("Start run " + (i + 1));
                    var results = sim.run(false, (int)configs[j][0], (int)configs[j][1] * 60, DayOfWeek.Wednesday, stationsArr, rates);
                    var trams = results.Trams;
                    var timetable = results.TimeTable;
                    var persons = results.Persons.Values.ToList();
                    var stations = results.Stations;

                    //Maximum delay of a tram
                    performM["CSMaxDelays"].Add(timetable.CSMaxDelay);
                    performM["PRMaxDelays"].Add(timetable.PRMaxDelay);
                    //Average total delay of a tram
                    performM["CSAvgDelays"].Add(timetable.CSAverageDelay);
                    performM["PRAvgDelays"].Add(timetable.PRAverageDelay);
                    //Percentage delays over one minute
                    performM["PrcOverOneMinuteDelay"].Add((double)timetable.DelaysOverOneMinute / timetable.NumberOfRounds);
                    //Max waiting time of a person
                    performM["MaxWaitTimes"].Add(persons.Where(x => x.ArrivalTime > 7 * 3600).Max(x => x.WaitingTime));
                    //Average waiting time of a person
                    performM["AvgWaitTimes"].Add(persons.Where(x => x.ArrivalTime > 7 * 3600).Sum(x => x.WaitingTime) / persons.Count);
                    //Maximum travel time of a person
                    performM["MaxTravelTimes"].Add(persons.Where(x => x.ArrivalTime > 7 * 3600).Max(x => x.LeaveTime - x.ArrivalTime));
                    //Average travel time of a person
                    performM["AvgTravelTimes"].Add(persons.Where(x => x.ArrivalTime > 7 * 3600).Sum(x => x.LeaveTime - x.ArrivalTime) / persons.Count);
                    //Maximum queue length
                    performM["CSMaxQueueSizes"].Add(stations.Values.ToList().Max(x => x.MaxQueueLengthCS));
                    performM["PRMaxQueueSizes"].Add(stations.Values.ToList().Max(x => x.MaxQueueLengthPR));
                }

                configResults.Add("f=" + configs[j][0] + ",q=" + configs[j][1], performM);
            }

            Console.WriteLine("Simulation completed. Start writing output");

            return configResults;
            //return new List<double>{csMaxDelays, prMaxDelays, csAvgDelays, prAvgDelays, prcDelaysOverOneMinute,
            //maxWaitTimes, avgWaitTimes, maxTravelTimes, avgTravelTimes, csMaxQueueSizes, prMaxQueueSizes};
        }

        public static List<double> ConfidenceInterval(List<double> l1, List<double> l2)
        {
            var diffL = l1.Select((x, i) => x - l2[i]).ToList();
            var diffAvg = diffL.Sum() / l1.Count;
            var variance = diffL.Sum(x => Math.Pow(x - diffAvg, 2)) / (diffL.Count - 1);

            var tval = 1.984;
            var low = Math.Round(diffAvg - tval * Math.Sqrt(variance / diffL.Count), 3);
            var high = Math.Round(diffAvg + tval * Math.Sqrt(variance / diffL.Count), 3);

            return new List<double> { low, high };
        }
        public static List<double> ConfidenceIntervalSingle(List<double> l1)
        {
            var diffAvg = l1.Sum() / l1.Count;
            var variance = l1.Sum(x => Math.Pow(x - diffAvg, 2)) / (l1.Count - 1);

            var tval = 1.984;
            var low = Math.Round(diffAvg - tval * Math.Sqrt(variance / l1.Count), 3);
            var high = Math.Round(diffAvg + tval * Math.Sqrt(variance / l1.Count), 3);

            return new List<double> { low, high };
        }
    }
}
