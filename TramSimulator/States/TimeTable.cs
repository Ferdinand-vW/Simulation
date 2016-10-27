using System;
using System.Collections.Generic;
using System.Linq;

namespace TramSimulator.States
{
    public class TimeTable
    {
        //Sum of all delays divided by the number of delays
        public double PRAverageDelay { get { return DelaysAtPR.Values.SelectMany(x => x).Sum() / DelaysAtPR.Values.SelectMany(x => x).Count(); } }
        //Maximum delay
        public double PRMaxDelay { get { return DelaysAtPR.Values.SelectMany(x => x).Max(); } }
        public double CSAverageDelay { get { return DelaysAtCS.Values.SelectMany(x => x).Sum() / DelaysAtCS.Values.SelectMany(x => x).Count(); } }
        public double CSMaxDelay { get { return DelaysAtCS.Values.SelectMany(x => x).Max(); } }
        public int DelaysOverOneMinute { get; set; }
        public int NumberOfRounds { get; set; }
        public int TurnAroundTime { get; set; }

        Dictionary<int, double> _expectedDepartureTimes;
        Dictionary<int, List<double>> DelaysAtPR { get; set; }
        Dictionary<int, List<double>> DelaysAtCS { get; set; }


        public TimeTable(double startTime, int turnAroundTime, int[] tramIds, double[] departureTimes)
        {
            this.TurnAroundTime = turnAroundTime;
            //Create an entry for each tram
            this._expectedDepartureTimes = tramIds.Zip(departureTimes, (k,v) => new { k, v})
                                                  .ToDictionary(x => x.k, x => x.v);
            this.DelaysAtPR = tramIds.ToDictionary(x => x, x => new List<double>());
            //Each tram starts with having a delay of 0
            DelaysAtPR.Keys.ToList().ForEach(x => DelaysAtPR[x].Add(0));
            this.DelaysAtCS = tramIds.ToDictionary(x => x, x => new List<double>());
            DelaysAtCS.Keys.ToList().ForEach(x => DelaysAtCS[x].Add(0));
            //renewTimeTable(startTime);
        }

        public double UpdateTimetable(int tramId, string depStation, double arrTime)
        {
            var depTimeOld = _expectedDepartureTimes[tramId];
            //Expected departure time if tram had not been delayed
            var depTimeExpected = depTimeOld + Constants.ONE_WAY_DRIVING_TIME + TurnAroundTime;
            //If it had been delayed then we should leave asap
            var depTimeNew = Math.Max(arrTime, depTimeExpected);
            //Set the new delay time
            _expectedDepartureTimes[tramId] = depTimeExpected;

            //Updates any incurred delays
            DetermineDelays(tramId, depTimeExpected, depTimeNew, depStation);
            NumberOfRounds++;

            return depTimeNew;
        }

        public void DetermineDelays(int tramId, double depTimeExpected, double depTimeNew, string depStation)
        {
            var delay = depTimeNew - depTimeExpected;

            bool moreThanAMinute = false;
            //Store the delay in the list
            if(depStation == Constants.PR) { DelaysAtPR[tramId].Add(delay); }
            else { DelaysAtCS[tramId].Add(delay); }
            //determine if the delay incurred was at least one minute
            if (delay >= Constants.SECONDS_IN_MINUTE) { moreThanAMinute = true; }

            //Increase the number of delays above one minute for this tram
            if (moreThanAMinute) { DelaysOverOneMinute++; }
        }

        public double MaxDelay()
        {
            List<double> prDelays = DelaysAtPR.Values.SelectMany(x => x).ToList();
            List<double> csDelays = DelaysAtCS.Values.SelectMany(x => x).ToList();

            return Math.Max(prDelays.Max(x => x), csDelays.Max(x => x));
        }
    }
}
