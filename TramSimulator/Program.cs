//Ferdinand van Walree, 3874389
//Rogier Wuijts, 

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

using TramSimulator.InputModels;
using TramSimulator.Rates;
using TramSimulator.Sim;

namespace TramSimulator
{
    class Program
    {
        const string Path = "../../";

        static void Main(string[] args)
        {
            Console.WriteLine("Start reading passengercount data..");
            String patha = Path + "input/a_data_updated.csv";
            String pathb = Path + "input/b_data_updated.csv";

            Stream streama = File.Open(patha, FileMode.Open);
            Stream streamb = File.Open(pathb, FileMode.Open);

            List<PassengerCount> passengerCountsA = null;
            List<PassengerCount> passengerCountsB = null;

            //Read passengercount data
            var threadA = new Thread(() => { passengerCountsA = Parser.ParsePassengerCounts(streama); });
            threadA.Start();
            var threadB = new Thread(() => { passengerCountsB = Parser.ParsePassengerCounts(streamb); });
            threadB.Start();
            threadA.Join();
            threadB.Join();

            Func<double, double, double> div = (x, y) => x / y;
            string[] stations = new string[] { "PR", "WKZ", "UMC", "Heidelberglaan", "Padualaan", "Kromme Rijn", "Galgenwaard", "Vaartscherijn", "CS" };

            //Read artificial data
            Stream f1 = File.Open(Path + "input/input-data-passengers-01.csv", FileMode.Open);
            ArtInput f1A = Parser.ParseArtInput(f1, stations);
            Stream f15 = File.Open(Path + "input/input-data-passengers-015.csv", FileMode.Open);
            ArtInput f15A = Parser.ParseArtInput(f15, stations);
            Stream f2 = File.Open(Path + "input/input-data-passengers-02.csv", FileMode.Open);
            ArtInput f2A = Parser.ParseArtInput(f2, stations);
            Stream f25 = File.Open(Path + "input/input-data-passengers-025.csv", FileMode.Open);
            ArtInput f25A = Parser.ParseArtInput(f25, stations);
            Stream f3 = File.Open(Path + "input/input-data-passengers-03.csv", FileMode.Open);
            ArtInput f3A = Parser.ParseArtInput(f3, stations);
            Stream f4 = File.Open(Path + "input/input-data-passengers-04.csv", FileMode.Open);
            ArtInput f4A = Parser.ParseArtInput(f4, stations);
            Stream f6 = File.Open(Path + "input/input-data-passengers-06.csv", FileMode.Open);
            ArtInput f6A = Parser.ParseArtInput(f6, stations);

            Console.WriteLine("Finished reading passengercount data");

            //Run artificial simulations
            var results_file1 = RunSimulation("File1.txt", new ArtificialRates(f1A), stations);
            OutputRun(results_file1, Path + "output/", "File1");
            var results_file15 = RunSimulation("File15.txt", new ArtificialRates(f15A), stations);
            OutputRun(results_file15, Path + "output/", "File15");
            var results_file2 = RunSimulation("File2.txt", new ArtificialRates(f2A), stations);
            OutputRun(results_file2, Path + "output/", "File2");
            var results_file25 = RunSimulation("File25.txt", new ArtificialRates(f25A), stations);
            OutputRun(results_file25, Path + "output/", "File25");
            var results_file3 = RunSimulation("File3.txt", new ArtificialRates(f3A), stations);
            OutputRun(results_file3, Path + "output/", "File3");
            var results_file4 = RunSimulation("File4.txt", new ArtificialRates(f4A), stations);
            OutputRun(results_file4, Path + "output/", "File4");
            var results_file6 = RunSimulation("File6.txt", new ArtificialRates(f6A), stations);
            OutputRun(results_file6, Path + "output/", "File6");

            //Run the realistic simulation, where the number of arriving passengers is
            //multiplied by a constant factor
            for (int i = 1; i <= 6; i++) {
                double c = (double)i / 2;
                Data a = dataA(c, passengerCountsA);
                Data b = dataB(c, passengerCountsB);
                var results_realistic = RunSimulation(Path + "realistic.txt", new SimulationRates(a, b, DayOfWeek.Wednesday), stations);
                OutputRun(results_realistic, Path + "output/", "c=" + i );
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
            passengerCountsA.ForEach(x => a.AddPassengerCount(x));
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
            passengerCountsB.ForEach(x => b.AddPassengerCount(x));
            return b;
        }
        public static void OutputRun(Dictionary<string, Dictionary<string, List<double>>> results, string path, string fileNameExt)
        {
            //Output the results for each performance measure
            foreach (var perf in results.Values.ToList()[0].Keys)
            {
                Stream s = File.Create(path + perf + fileNameExt + ".csv");
                using (StreamWriter sw = new StreamWriter(s))
                {
                    var matrix = new List<string>();

                    matrix.Add(";" + String.Join(";", results.Keys));

                    foreach (var kvp in results)
                    {
                        var row = new List<string>();
                        row.Add(kvp.Key);
                        foreach (var kvp2 in results)
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
            
            Console.WriteLine(fileNameExt);
            // Output results for each configuration
            foreach (var kvp in results)
            {
                Stream s = File.Create(path + kvp.Key + fileNameExt + ".csv");
                var resultConfig = kvp.Value;
                using (StreamWriter sw = new StreamWriter(s))
                {
                    var matrix = new List<string>();
                    string[] runs = new string[resultConfig.Values.ToList()[0].Count];
                    for (int i = 0; i < runs.Length; i++)
                    {
                        runs[i] = i.ToString();
                    }
                    matrix.Add(";" + String.Join(";", runs));

                    foreach (var perf in results.Values.ToList()[0].Keys)
                    {
                        var row = new List<string>();
                        row.Add(perf);
                        var rofOfValues = resultConfig[perf];
                        rofOfValues.ForEach(x => row.Add(Math.Round(x, 3).ToString()));
                        matrix.Add(String.Join(";", row));
                    }

                    matrix.ForEach(x => sw.WriteLine(x));
                }
            }

            Stream ss = File.Create(path + "Alles" + fileNameExt + ".csv");
            //Output averages of all performance measures 
            using (StreamWriter sw = new StreamWriter(ss))
            {
                var matrix = new List<string>();
                matrix.Add(";" + String.Join(";", results.Keys.ToList()));

                foreach (var perf in results.Values.ToList()[0].Keys)
                {
                    var row = new List<string>();
                    row.Add(perf);
                    foreach (var kvp in results)
                    {
                        var resultConfig = kvp.Value;
                        var list = resultConfig[perf];
                        row.Add(String.Join(":", ConfidenceIntervalSingle(list)));
                    }
                    matrix.Add(String.Join(";", row));
                }
                matrix.ForEach(x => sw.WriteLine(x));
            }

            Console.WriteLine(fileNameExt + " is done");
        }

        //Run simulations
        public static Dictionary<string, Dictionary<string, List<double>>> RunSimulation(string filename, AbstractSimulationRates rates, string[] stationsArr)
        {
            Simulation sim = new Simulation();
            //debug, frequency, turnaroundtime, day, stations

            var configResults = new Dictionary<string, Dictionary<string, List<double>>>();

            Console.WriteLine("Start simulation " + filename);

            //Uncomment a configuration if you want to test it
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

                //Each configuration is tested 100 times
                for (int i = 0; i < 100; i++)
                {
                    Console.WriteLine("Start run " + (i + 1));
                    //Run a single simulation
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
                    performM["MaxWaitTimes"].Add(persons.Where(x => x.ArrivalTime > 7 * Constants.SECONDS_IN_HOUR).Max(x => x.WaitingTime));
                    //Average waiting time of a person
                    performM["AvgWaitTimes"].Add(persons.Where(x => x.ArrivalTime > 7 * Constants.SECONDS_IN_HOUR).Sum(x => x.WaitingTime) / persons.Count);
                    //Maximum travel time of a person
                    performM["MaxTravelTimes"].Add(persons.Where(x => x.ArrivalTime > 7 * Constants.SECONDS_IN_HOUR).Max(x => x.LeaveTime - x.ArrivalTime));
                    //Average travel time of a person
                    performM["AvgTravelTimes"].Add(persons.Where(x => x.ArrivalTime > 7 * Constants.SECONDS_IN_HOUR).Sum(x => x.LeaveTime - x.ArrivalTime) / persons.Count);
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

            //sig=0.05
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
