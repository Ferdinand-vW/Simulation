using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TramSimulator
{
    public class Data
    {
        Dictionary<DayOfWeek, DayData> days;
        Dictionary<string, double> enterPrognose;
        Dictionary<string, string> stationToBus;
        Dictionary<string, double> exitPrognose;
        public Data(Dictionary<string, double> enterPrognose, Dictionary<string, double> exitPrognose)
        {
            this.enterPrognose = enterPrognose;
            this.exitPrognose = exitPrognose;

            stationToBus = new Dictionary<string, string>();
            stationToBus["PR"] = "AZU";
            stationToBus["WKZ"] = "AZU";
            stationToBus["UMC"] = "AZU";
            stationToBus["Heidelberglaan"] = "Heidelberglaan";
            stationToBus["Padualaan"] = "Padualaan";
            stationToBus["Kromme Rijn"] = "De Kromme Rijn";
            stationToBus["Galgenwaard"] = "Stadion Galgenwaard";
            stationToBus["Vaartscherijn"] = "Bleekstraat";
            stationToBus["CS"] = "CS Centrumzijde";

            days = new Dictionary<DayOfWeek, DayData>();
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                days[day] = new DayData();
            }

        }
        public void AddPC(PassengerCount pc)
        {
            days[pc.Date.DayOfWeek].AddPC(pc);
        }

        public double EnteringFQ(DayOfWeek day, string station, double time)
        {
            string busStop = stationToBus[station];
            return days[day].EnteringFQ(busStop, time) * enterPrognose[station];
        }

        public int EnteringTotal(DayOfWeek day, string station)
        {
            return days[day].EnteringTotal(stationToBus[station]);
        }

        public double DepartingFQ(DayOfWeek day, string station, double time)
        {
            string busStop = stationToBus[station];
            return days[day].DepartingFQ(busStop, time) * exitPrognose[station];
        }

        public double DepartingPercentage(DayOfWeek day, string station, double time)
        {
            string busStop = stationToBus[station];
            return days[day].DepartingFQ(busStop, time);
        }

        public int DepartingTotal(DayOfWeek day, string station)
        {
            return days[day].DepartingTotal(stationToBus[station]);
        }

        public double DepartPercentage(string station)
        {
            return exitPrognose[station];
        }

        class DayData
        {
            Dictionary<int, Min15Block> blocks;
            Dictionary<string, int> totals;

            public DayData()
            {
                totals = new Dictionary<string, int>();
                blocks = new Dictionary<int, Min15Block>();

            }
            public double EnteringFQ(string busStop, double time)
            {
                int min = (int)time / 60;
                //0,15,30 or 45minutes
                min = min - (min % 15);
                //determine the frequency for this block
                double fq = blocks.ContainsKey(min) ? blocks[min].EnteringFQ(busStop) : 0;

                //Some stops don't have any entering passengers, in that case we can just return a rate of 0
                if (totals[busStop] == 0) { return 0; }
                else { return fq / totals[busStop]; }
            }

            public double DepartingFQ(string busStop, double time)
            {
                int min = (int)time / 60;
                min -= min % 15;
                double fq = blocks.ContainsKey(min) ? blocks[min].DepartingFQ(busStop) : 0;

                if(totals[busStop] == 0) { return 0; }
                else { return fq / totals[busStop]; }
            }

            public int EnteringTotal(string busStop)
            {
                return totals[busStop];
            }

            public int DepartingTotal(string busStop)
            {
                return totals[busStop];
            }

            public void AddPC(PassengerCount pc)
            {
                foreach (var kvp in pc.EnteringCounts)
                {
                    //Add enter counts to totals
                    if (totals.ContainsKey(kvp.Key))
                        totals[kvp.Key] += kvp.Value;
                    else {
                        totals[kvp.Key] = kvp.Value;
                    }
                }

                //add enter counts to a block
                int min = pc.Time.Hour * 60 + pc.Time.Minute;
                min = min - (min % 15);
                if (blocks.ContainsKey(min))
                {
                    blocks[min].AddPC(pc);
                }
                else
                {
                    blocks[min] = new Min15Block();
                    blocks[min].AddPC(pc);
                }
            }
            class Min15Block
            {
                List<PassengerCount> PCs;
                public Min15Block()
                {
                    PCs = new List<PassengerCount>();
                }
                public void AddPC(PassengerCount pc)
                {
                    PCs.Add(pc);
                }
                public double EnteringFQ(string busStop)
                {
                    return PCs.Sum(x => x.EnteringCounts[busStop]);
                }

                public double DepartingFQ(string busStop)
                {
                    return PCs.Sum(x => x.DepartingCounts[busStop]);
                }
            }
        }
    }
}
