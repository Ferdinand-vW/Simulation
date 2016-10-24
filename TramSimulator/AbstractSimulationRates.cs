﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator
{
    public abstract class AbstractSimulationRates
    {
        public abstract double PersonArrivalRate(string station, Routes.Dir dir, double time);
        public abstract bool nonZeroPercentage(double time, string station, Routes.Dir dir);

        public double TramArrivalRate(string depStation, string arrStation)
        {
            return Generate.logNormalWithoutVariance(AvgTramArrival(depStation, arrStation));
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
            else
            {
                Console.WriteLine("what??");
            }
            return Double.MaxValue;
        }

        public bool DoorMalfunction()
        {
            return Generate.uniform(0, 1) < 0.03;
        }

        public abstract double TramEmptyRate(DayOfWeek day, string station, Routes.Dir dir, Tram tram, double time);

        public double DwellTime(int pIn, int pOut, int transfer)
        {
            //var time = 23 * Math.Pow(10, -5) * Math.Pow(transfer, 2) * (pIn + pOut) * Constants.SECONDS_IN_HOUR;
            var time = 12.5 + 0.22 * pIn + 0.13 * pOut;

            return time;
        }

        public int TramFillRate(Station station, Tram tram)
        {
            var waitingPersons = Routes.ToCS(tram.Direction) ? station.WaitingPersonsToCS
                                                             : station.WaitingPersonsToPR;
            var fill = Math.Min(Tram.CAPACITY - tram.PersonsOnTram.Count, waitingPersons.Count);

            return fill;
        }
    }
}