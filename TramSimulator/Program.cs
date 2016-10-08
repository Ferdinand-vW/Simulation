//Ferdinand van Walree, 3874389
//Rogier Wuijts 

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;

using TramSimulator.Events;
using TramSimulator.States;

namespace TramSimulator
{
    class Program
    {
        static void Main(string[] args)


        {

            String patha = @"F:\Users\Rogier\Source\Repos\Simulation2\a_data_updated.csv";
            String pathb = @"F:\Users\Rogier\Source\Repos\Simulation2\b_data_updated.csv";

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
            exitPrognoseA["Padualaan"] = 236 / 12827;
            exitPrognoseA["Kromme Rijn"] = 31 / 19446;
            exitPrognoseA["Galgenwaard"] = 265 / 20106;
            exitPrognoseA["Vaartscherijn"] = 544 / 20447;
            exitPrognoseA["CS"] = 1;

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
            exitPrognoseB["Vaartscherijn"] = 1735 / 19994;
            exitPrognoseB["Galgenwaard"] = 1288 / 20596;
            exitPrognoseB["Kromme Rijn"] = 1039 / 19667;
            exitPrognoseB["Padualaan"] = 9672 / 18674;
            exitPrognoseB["Heidelberglaan"] = 5789 / 9011;
            exitPrognoseB["UMC"] = 2577 / 3230;
            exitPrognoseB["WKZ"] = 644 / 659;
            exitPrognoseB["PR"] = 1;


            Data a = new Data(enterPrognoseA, exitPrognoseA);
            Data b = new Data(enterPrognoseB, exitPrognoseB);
            foreach (PassengerCount pc in passengerCountsA)
                a.AddPC(pc);
            foreach (PassengerCount pc in passengerCountsB)
                b.AddPC(pc);


            // SimulationRates rates = new SimulationRates(a, b, DayOfWeek.Monday);
            //for (int i = 60 * 60 * 6; i < 21 * 60 * 60; i = i + 15 * 60)
            //{
            //   if (rates.nonZeroPercentage("Heidelberglaan", true, i))
            //  {
            //       double dist = rates.PersonArrivalRate("Heidelberglaan", true, i);
            //       Console.WriteLine(dist);
            //   }
            // }
            //Console.ReadLine();
            //return;
            Simulation sim = new Simulation(a,b);
            sim.run(10, null, DayOfWeek.Monday, new string[] { "PR", "WKZ", "UMC", "Heidelberglaan", "Padualaan", "Kromme Rijn", "Galgenwaard", "Vaartscherijn", "CS" });
            return;
            //Some example output
            //Console.WriteLine("Number of passengercounts: " + passengerCountsA.Count);
            //Console.WriteLine(passengerCountsA[0].Trip);
            //Console.WriteLine(passengerCountsA[0].Date);
            //Console.WriteLine(passengerCountsA[0].Time.Hour);
            //foreach (var kvp in passengerCountsA[0].EnteringCounts)
            //{
            //    Console.WriteLine(kvp.Key + ": " + kvp.Value);
            //}
            //foreach (var kvp in passengerCountsA[0].DepartingCounts)
            //{
            //    Console.WriteLine(kvp.Key + ": " + kvp.Value);
            //}
            Console.ReadLine();
        }


    }
}
