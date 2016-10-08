using System;
using System.Linq;
using System.Collections.Generic;

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

        public double PersonArrivalRate(string station, bool typeA, double time)
        {
            double avgPersonPer15Min;
            if (typeA)
                avgPersonPer15Min = a.EnteringFQ(day, station, time);
            else
                avgPersonPer15Min = b.EnteringFQ(day, station, time);
            double avgPersonPerSec = avgPersonPer15Min / (double)(60 * 15);
            double enteringPercentage = typeA ? a.EnteringFQ(day, station, time) : b.EnteringFQ(day, station, time);

            double distNextEvent = Generate.negexp(avgPersonPerSec);
            return distNextEvent;
        }
        public bool nonZeroPercentage(string station, bool typeA, double time)
        {
            double enteringPercentage = typeA ? a.EnteringFQ(day, station, time) : b.EnteringFQ(day, station, time);
            return enteringPercentage != 0 ;
        }

        public double TramArrivalRate(string depStation, string arrStation)
        {
            return AvgTramArrival(depStation, arrStation);
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
                 throw new Exception("Station "+ depStation + " and "+ arrStation + " does not exist");
        }

        public double DelayRate(string station)
        {
            throw new NotImplementedException();
        }

        public double TramEmptyRate(string station)
        {
            throw new NotImplementedException();
        }

        public double TramFillRate(string station)
        {
            throw new NotImplementedException();
        }


    }
}