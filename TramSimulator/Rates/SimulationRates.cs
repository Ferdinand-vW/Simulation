using System;

using TramSimulator.States;
using TramSimulator.InputModels;

namespace TramSimulator.Rates
{
    //Contains rates specific to the realistic input
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
            //calculate the average amount of people arriving within 15minutes
            if (Routes.ToCS(dir)) { avgPersonPer15Min = a.EnteringFreq(day, station, time); }
            else { avgPersonPer15Min = b.EnteringFreq(day, station, time); }

            //Randomize this average according to the poisson distribution in seconds
            double avgPersonPerSec = avgPersonPer15Min / (double)(Constants.SECONDS_IN_MINUTE * 15);
            return Generate.NegExp(avgPersonPerSec);
        }

        public override bool NonZeroPercentage(double time, string station, Routes.Dir dir) {
            //Determine whethere there any passengers arrive
            return Routes.ToCS(dir) ? 0 == a.EnteringFreq(day, station, time) 
                                    : 0 == b.EnteringFreq(day, station, time);
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