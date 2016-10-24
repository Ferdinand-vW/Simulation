using System;
using System.Linq;
using System.Collections.Generic;

using TramSimulator.States;

namespace TramSimulator
{
    [Serializable]
    public class SimulationRates : AbstractSimulationRates
    {

        Data a;
        Data b;
        DayOfWeek day;
        public SimulationRates(Data a, Data b, DayOfWeek day)
        {
            this.a = a;
            this.b = b;
            this.day = day;
        }

        public override double PersonArrivalRate(string station, Routes.Dir dir, double time)
        {
            double avgPersonPer15Min;
            if (Routes.ToCS(dir))
                avgPersonPer15Min = a.EnteringFQ(day, station, time);
            else
                avgPersonPer15Min = b.EnteringFQ(day, station, time);
            double avgPersonPerSec = avgPersonPer15Min / (double)(60 * 15);
            double distNextEvent = Generate.negexp(avgPersonPerSec);

            return distNextEvent;
        }
        public override bool nonZeroPercentage(double time, string station, Routes.Dir dir) {
            return Routes.ToCS(dir) ? 0 == a.EnteringFQ(day, station, time) 
                                    : 0 == b.EnteringFQ(day, station, time);
        }

        public override double TramEmptyRate(DayOfWeek day, string station, Routes.Dir dir, Tram tram, double time)
        {
            double fillRatio = (double)tram.PersonsOnTram.Count / (double)Tram.CAPACITY;
            //Percentage of people that leave the tram
            return (Routes.ToCS(dir) ? a.DepartPercentage(station)
                                     : b.DepartPercentage(station));
        }

    }
}