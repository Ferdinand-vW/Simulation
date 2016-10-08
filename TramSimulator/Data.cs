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
        public double departPercentage(string station) {
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
                min = min - (min % 15);
                double fq = blocks.ContainsKey(min) ? blocks[min].EnteringFQ(busStop) : 0;
                return fq /totals[busStop];
            }

            public void AddPC(PassengerCount pc)
            {
                foreach (var kvp in pc.EnteringCounts)
                {
                    if (totals.ContainsKey(kvp.Key))
                        totals[kvp.Key] += kvp.Value;
                    else {
                        totals[kvp.Key] = kvp.Value;
                    }
                }

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
                    //niet correct
                    double feq = 0;
                    foreach (PassengerCount pc in PCs)
                        feq += pc.EnteringCounts[busStop];
                    return feq;
                }
            }
        }
    }
}
