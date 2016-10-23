using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TramSimulator.Events;
using TramSimulator.States;

namespace TramSimulator
{
    public class TimeTable
    {
        public double PRAverageDelay { get { return DelaysAtPR.Values.SelectMany(x => x).Sum() / DelaysAtPR.Values.SelectMany(x => x).Count(); } }
        public double PRMaxDelay { get { return DelaysAtPR.Values.SelectMany(x => x).Max(); } }
        public double CSAverageDelay { get { return DelaysAtCS.Values.SelectMany(x => x).Sum() / DelaysAtCS.Values.SelectMany(x => x).Count(); } }
        public double CSMaxDelay { get { return DelaysAtCS.Values.SelectMany(x => x).Max(); } }
        public int DelaysOverOneMinute
        {
            get
            {
                return DelaysAboveOneMinutePR.Values.
                        Union(DelaysAboveOneMinuteCS.Values).
                        Where(x => x > 0).ToList().Count;
            }
        }
        public int NumberOfRounds { get; set; }
        public int TurnAroundTime { get; set; }

        Dictionary<int, double> _expectedDepartureTimes;
        Dictionary<int, List<double>> DelaysAtPR { get; set; }
        Dictionary<int, List<double>> DelaysAtCS { get; set; }
        Dictionary<int, int> DelaysAboveOneMinutePR { get; set; }
        Dictionary<int, int> DelaysAboveOneMinuteCS { get; set; }


        public TimeTable(double startTime, int turnAroundTime, int[] tramIds, double[] departureTimes)
        {
            this.NumberOfRounds = 0;
            this.TurnAroundTime = turnAroundTime;
            this._expectedDepartureTimes = tramIds.Zip(departureTimes, (k,v) => new { k, v})
                                                  .ToDictionary(x => x.k, x => x.v);
            this.DelaysAtPR = tramIds.ToDictionary(x => x, x => new List<double>());
            //Each tram starts with having a delay of 0
            DelaysAtPR.Keys.ToList().ForEach(x => DelaysAtPR[x].Add(0));
            this.DelaysAtCS = tramIds.ToDictionary(x => x, x => new List<double>());
            DelaysAtCS.Keys.ToList().ForEach(x => DelaysAtCS[x].Add(0));
            this.DelaysAboveOneMinutePR = tramIds.Select(x => Tuple.Create(x, 0)).ToDictionary(x => x.Item1, x => x.Item2);
            this.DelaysAboveOneMinuteCS = tramIds.Select(x => Tuple.Create(x, 0)).ToDictionary(x => x.Item1, x => x.Item2);
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
            if (moreThanAMinute)
            {
                if(depStation == Constants.PR) { DelaysAboveOneMinutePR[tramId]++; }
                else { DelaysAboveOneMinuteCS[tramId]++; }
            }
        }

        public double MaxDelay()
        {
            List<double> prDelays = DelaysAtPR.Values.SelectMany(x => x).ToList();
            List<double> csDelays = DelaysAtCS.Values.SelectMany(x => x).ToList();

            return Math.Max(prDelays.Max(x => x), csDelays.Max(x => x));
        }


        /*
        public void renewTimeTable(double startTime)
        {
            NumberOfRounds++;
            _startTime = startTime;
            _halfTime = startTime + Constants.ONE_WAY_DRIVING_TIME + _turnAroundTime;
            _totalTime = _halfTime + Constants.ONE_WAY_DRIVING_TIME + _turnAroundTime;
        }
        public void update(double time, string depStation) {
            if   (depStation == Constants.PR) { addTimePR(time); }
            else                              { addTimeCS(time); }
        }
        

        public void addTimePR(double time)
        {
            double delay = time - _startTime;
            if (delay > 0)
            {
                PRtotalDelay += delay;
            }
            if (delay > 1)
            {
                PRnumberOverOneMinute += 1;
            }
            if (delay > PRmaxDelay)
                PRmaxDelay = delay;
        }
        public void addTimeCS(double time)
        {
            double delay = time - _halfTime;
            if (delay < 0)
                //Console.ReadLine();
                throw new Exception("Negative delay in time table");
            if (delay > 0)
            {
                CStotalDelay += delay;
            }
            if (delay > 60)
            {
                CSnumberOverOneMinute += 1;
            }
            if (delay > CSmaxDelay)
                CSmaxDelay = delay;
        }*/
    }
}
