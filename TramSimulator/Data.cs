using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TramSimulator
{
    class Data
    {
        Dictionary<DayOfWeek, DayData> days;
        public Data()
        {
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

        public double EnteringFQ(DayOfWeek day, String station, double time)
        {
            return days[day].EnteringFQ(station, time);
        }
        class DayData
        {
            Dictionary<int, Min15Block> blocks;

            public DayData()
            {
                blocks = new Dictionary<int, Min15Block>();
            }
            public double EnteringFQ(String station, double time)
            {
                int min = (int)time / 60;
                min = min - (min % 15);
                return blocks[min].EnteringFQ(station);
            }

            public void AddPC(PassengerCount pc)
            {
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
                public double EnteringFQ(String station)
                {
                    double feq = 0;
                    foreach (PassengerCount pc in PCs)
                        feq += pc.EnteringCounts[station];
                    if (PCs.Count > 0)
                        feq = feq / (double)PCs.Count;
                    return feq;
                }
            }
        }
    }
}
