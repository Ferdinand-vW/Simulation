using System;
using System.Linq;
using System.Collections.Generic;

using TramSimulator.States;

namespace TramSimulator
{
    public class SimulationRates
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

        public double PersonArrivalRate(string station, Routes.Dir dir, double time)
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
        public bool nonZeroPercentage(double time, string station, Routes.Dir dir) {
            return Routes.ToCS(dir) ? 0 == a.EnteringFQ(day, station, time) 
                                    : 0 == b.EnteringFQ(day, station, time);
        }

        public double TramArrivalRate(string depStation, string arrStation)
        {
            return Generate.logNormalWitouthVariance(AvgTramArrival(depStation, arrStation));
        }

        public double AvgTramArrival(string depStation, string arrStation)
        {
            if (depStation == "CS" && arrStation == "Vaartscherijn")
                return 134;
            else if (depStation == "Vaartscherijn" && arrStation == "Galgenwaard")
                return 243;
            else if (depStation == "Galgenwaard" && arrStation == "Kromme Rijn")
                return 59;
            else if (depStation == "Kromme Rijn" && arrStation == "Padualaan")
                return 101;
            else if (depStation == "Padualaan" && arrStation == "Heidelberglaan")
                return 60;
            else if (depStation == "Heidelberglaan" && arrStation == "UMC")
                return 86;
            else if (depStation == "UMC" && arrStation == "WKZ")
                return 78;
            else if (depStation == "WKZ" && arrStation == "PR")
                return 113;
            else if (depStation == "PR" && arrStation == "WKZ")
                return 110;
            else if (depStation == "WKZ" && arrStation == "UMC")
                return 78;
            else if (depStation == "UMC" && arrStation == "Heidelberglaan")
                return 82;
            else if (depStation == "Heidelberglaan" && arrStation == "Padualaan")
                return 60;
            else if (depStation == "Padualaan" && arrStation == "Kromme Rijn")
                return 100;
            else if (depStation == "Kromme Rijn" && arrStation == "Galgenwaard")
                return 59;
            else if (depStation == "Galgenwaard" && arrStation == "Vaartscherijn")
                return 243;
            else if (depStation == "Vaartscherijn" && arrStation == "CS")
                return 135;
            return Double.MaxValue;
        }

        public bool DoorMalfunction()
        {
            return Generate.uniform(0, 5) == 0;
        }

        public double TramEmptyRate(DayOfWeek day, string station, Routes.Dir dir, Tram tram, double time)
        {
            double fillRatio = (double)tram.PersonsOnTram.Count / (double)Tram.CAPACITY;
            //Percentage of people that leave the tram
            return (Routes.ToCS(dir) ? a.DepartPercentage(station)
                                     : b.DepartPercentage(station));
        }

        public double TramEmptyTime(int npss)
        {
            return ((double)npss / (double)Tram.CAPACITY) * 60;
        }

        public int TramFillRate(Station station, Tram tram)
        {
            var waitingPersons = Routes.ToCS(tram.Direction) ? station.WaitingPersonsToCS
                                                             : station.WaitingPersonsToPR;
            return Math.Min(Tram.CAPACITY - tram.PersonsOnTram.Count, waitingPersons.Count);
        }

        public double TramFillTime(int npss)
        {
            return ((double)npss / (double)Tram.CAPACITY) * 60;
        }


    }
}