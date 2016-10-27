using System;

using TramSimulator.States;

namespace TramSimulator.Rates
{
    public abstract class AbstractSimulationRates
    {
        //These must be implemented by subclasses
        public abstract double PersonArrivalRate(string station, Routes.Dir dir, double time);
        public abstract bool NonZeroPercentage(double time, string station, Routes.Dir dir);
        public abstract double TramEmptyRate(DayOfWeek day, string station, Routes.Dir dir, Tram tram, double time);

        //Return a randomised driving time according to a lognormal distribution
        public double TramArrivalRate(string depStation, string arrStation)
        {
            return Generate.LogNormalWithoutVariance(AvgTramArrival(depStation, arrStation));
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

            //Something went wrong if it reached this point
            return Double.MaxValue;
        }

        //There exists a 3% chance that the door becomes stuck
        public bool DoorMalfunction()
        {
            return Generate.Uniform(0, 1) < 0.03;
        }

        //Time it takes for passengers to leave and enter the tram
        public double DwellTime(int pIn, int pOut, int transfer)
        {
            //Second dwelltime formula
            //var time = 23 * Math.Pow(10, -5) * Math.Pow(transfer, 2) * (pIn + pOut) * Constants.SECONDS_IN_HOUR;
            return 12.5 + 0.22 * pIn + 0.13 * pOut;
        }

        public int TramFillRate(Station station, Tram tram)
        {
            var waitingPersons = Routes.ToCS(tram.Direction) ? station.WaitingPersonsToCS
                                                             : station.WaitingPersonsToPR;
            return Math.Min(Tram.CAPACITY - tram.PersonsOnTram.Count, waitingPersons.Count);
        }
    }
}
