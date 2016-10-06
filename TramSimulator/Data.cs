using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TramSimulator
{
    class Data
    {
        Dictionary<DayOfWeek, DayData> days;
        Dictionary<string, double> totalPrognose;
        public Data(Dictionary<string, double> totalPrognose)
        {
            this.totalPrognose = totalPrognose;
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
            return days[day].EnteringFQ(station, time) * totalPrognose[station];
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
            public double EnteringFQ(string station, double time)
            {
                int min = (int)time / 60;
                min = min - (min % 15);
                double fq = blocks.ContainsKey(min) ? blocks[min].EnteringFQ(station) : 0;
                return fq /totals[station];
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
                public double EnteringFQ(string station)
                {
                    //niet correct
                    double feq = 0;
                    foreach (PassengerCount pc in PCs)
                        feq += pc.EnteringCounts[station];
                    return feq;
                }
            }
        }
    }
}
