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
            if (false)
            {
                Simulation sim = new Simulation(null);
                sim.run(2, null, "monday", new string[] { "Central", "AZR", "PR" });
                return;
            }


            //We probably want a different method of input for the file paths
            //Console.WriteLine("Enter a file path");
            String patha = "C:\\Users\\Rogier\\Desktop\\12a.csv";
            String pathb = "C:\\Users\\Rogier\\Desktop\\12b.csv";

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

            Dictionary<string, double> totalPrognose = new Dictionary<string, double>();
            totalPrognose["PR"] = 15;
            totalPrognose["WKZ"] = 1015;
            totalPrognose["UMC"] = 2660;
            totalPrognose["Heidelberglaan"] = 9138;
            totalPrognose["Padualaan"] = 6855;
            totalPrognose["Kromme Rijn"] = 691;
            totalPrognose["Galgenwaard"] = 606;
            totalPrognose["Vaartscherijn"] = 1261;
            totalPrognose["CS"] = 0;
            Data a = new Data(totalPrognose);
            foreach (PassengerCount pc in passengerCountsA)
                a.AddPC(pc);
            double total = 0;
            for (int i = 0; i < 96; i++)
            {
                double j = a.EnteringFQ(DayOfWeek.Monday, "Heidelberglaan", i * 15 * 60);
                Console.WriteLine("{0}:{1} = {2}", i / 4, i % 4 * 15, j);
                total += j;
            }
            Console.WriteLine("total{0}", total);
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
