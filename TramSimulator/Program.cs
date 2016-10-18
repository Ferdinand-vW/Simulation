//Ferdinand van Walree, 3874389
//Rogier Wuijts, 

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
            Console.WriteLine("Start reading passengercount data..");
            String patha = @"../a_data_updated.csv";
            String pathb = @"../b_data_updated.csv";

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

            Console.WriteLine("Finished reading passengercount data");
            Console.WriteLine("Start simulation");
            Simulation sim = new Simulation(a,b);
            var results = sim.run(false, 15, null, DayOfWeek.Monday, enterPrognoseA.Keys.ToArray());

            var trams = results.TimeTables.Values.ToList();
            var persons = results.Persons.Values.ToList();

            //Maximum delay of a tram
            Console.Write("Maximum delay: ");
            Console.WriteLine(trams.Max(x => Math.Max(x.PRmaxDelay, x.CSmaxDelay)));
            //Average total delay of a tram
            Console.Write("Average delay: ");
            Console.WriteLine(trams.Sum(x => x.PRtotalDelay + x.CStotalDelay) / trams.Count);
            //Max waiting time of a person
            Console.Write("Maximum waiting time: ");
            Console.WriteLine(persons.Max(x => x.WaitingTime));
            //Average waiting time of a person
            Console.Write("Average waiting time: ");
            Console.WriteLine(persons.Sum(x => x.WaitingTime) / persons.Count);
            //Maximum travel time of a person
            Console.Write("Maximum travel time of a person: ");
            Console.WriteLine(persons.Max(x => x.LeaveTime - x.ArrivalTime));
            //Average travel time of a person
            Console.Write("Average travel time of a person: ");
            Console.WriteLine(persons.Sum(x => x.LeaveTime - x.ArrivalTime) / persons.Count);
            
            //Number of people that never left or got on a tram. Also the reason why the above
            //statistic is negative. TODO: figure out why there are so many passengers that never get on
            //or leave a tram
            Console.WriteLine(persons.Where(x => x.LeaveTime == 0).ToList().Count);

            Console.ReadLine();
        }
    }
}
