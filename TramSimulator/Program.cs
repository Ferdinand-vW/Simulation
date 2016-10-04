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
            String patha = "F:\\Users\\Rogier\\Desktop\\12a.csv";
            String pathb = "F:\\Users\\Rogier\\Desktop\\12b.csv";

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

            Data a = new Data();
            foreach (PassengerCount pc in passengerCountsA)
                a.AddPC(pc);
            Console.WriteLine( a.EnteringFQ(DayOfWeek.Monday, "Heidelberglaan", 9 * 60 * 60));
            Console.WriteLine(a.EnteringFQ(DayOfWeek.Monday, "Heidelberglaan", 14 * 60 * 60));
            Console.WriteLine(a.EnteringFQ(DayOfWeek.Tuesday, "Heidelberglaan", 14 * 60 * 60));
            Console.WriteLine(a.EnteringFQ(DayOfWeek.Monday, "Rubenslaan", 12 * 60 * 60));
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
