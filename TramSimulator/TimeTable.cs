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
        public double PRtotalDelay;
        public double PRmaxDelay;
        public int PRnumberOverOneMinute;
        public double CStotalDelay;
        public double CSmaxDelay;
        public int CSnumberOverOneMinute;
        public int numberOfRounds;

        public double startTime;
        public double halfTime; // voor het wachten op een eindstation
        public double totalTime; //voor de volgende timeTable
        int q;

        public TimeTable(double startTime, int q)
        {
            this.q = q;
            PRtotalDelay = 0;
            PRmaxDelay = 0;
            PRnumberOverOneMinute = 0;
            CStotalDelay = 0;
            CSmaxDelay = 0;
            CSnumberOverOneMinute = 0;
            numberOfRounds = 0;

            renewTimeTable(startTime);
        }

        public void renewTimeTable(double startTime)
        {
            numberOfRounds++;
            double oneWayDrivingTime = 17 * 60;
            double turnAroundTime = q * 60;
            this.startTime = startTime;
            halfTime = startTime + oneWayDrivingTime + turnAroundTime;
            totalTime = halfTime + oneWayDrivingTime + turnAroundTime;
        }
        public void update(double time, string depStation) {
            if (depStation == "PR")
                addTimePR(time);
            else if (depStation == "CS")
                addTimeCS(time);
        }

        public void addTimePR(double time)
        {
            double delay = time - startTime;
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
            double delay = time - halfTime;
            if (delay < 0)
                Console.ReadLine();
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
        }
    }
}
