using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TramSimulator.States;

namespace TramSimulator
{
    class ArtInput
    {
        Dictionary<string, StationArtInput> PRCS;
        Dictionary<string, StationArtInput> CSPR;
        string[] stationsPRCS;
        string[] stationsCSPR;

        public ArtInput(string[] stations)
        {
            stationsPRCS = stations;
            stationsCSPR = stations.Reverse().ToArray();
            PRCS = new Dictionary<string, StationArtInput>();
            CSPR = new Dictionary<string, StationArtInput>();
        }

        public void addInput(string station, int direction, int from, double to, double passIn, double passOut)
        {
            if (direction == 0)
            {
                if (!PRCS.ContainsKey(station)) { PRCS[station] = new StationArtInput(); }
                PRCS[station].addInput(from, to, passIn, passOut);

            }
            else
            {
                if (!CSPR.ContainsKey(station)) { CSPR[station] = new StationArtInput(); }
                CSPR[station].addInput(from, to, passIn, passOut);
            }
        }
        public double avgInPerHour(string station, double time, Routes.Dir dir)
        {
            if (dir == Routes.Dir.ToPR)
            {
                return CSPR[station].avgInPerHour(time);
            }
            else {
                return PRCS[station].avgInPerHour(time);
            }
        }
        public double departPercentage(string station, double time, Routes.Dir dir)
        {
            if (dir == Routes.Dir.ToPR)
            {
                double numberOnTrain = onTrain(station, time, dir);
                //If no one is on the train, Everybody gets out. 
                return numberOnTrain == 0 ? 1 : CSPR[station].passOut(time) / numberOnTrain;
            }

            else {
                double numberOnTrain = onTrain(station, time, dir);
                return numberOnTrain == 0 ? 1 : PRCS[station].passOut(time) / numberOnTrain;
            }

        }

        // Calculates how many persons are on the tram at a given station at a given time
        public double onTrain(string station, double time, Routes.Dir dir)
        {
            if (dir == Routes.Dir.ToPR)
            {
                int index = Array.IndexOf(stationsCSPR, station);
                if (index == 0) return 0;
                else {
                    string prevStation = stationsCSPR[index - 1];
                    // The amount of people got off and on the tram at the previous station
                    double extraPersons = (CSPR[prevStation].passOut(time) + CSPR[prevStation].passIn(time));
                    // The amount of people on the train is calculated 
                    //with the amout of people where on the train at the previous station
                    return extraPersons + (onTrain(prevStation, time, dir));
                }
            }
            else {
                int index = Array.IndexOf(stationsPRCS, station);
                if (index == 0) return 0;
                else {
                    string prevStation = stationsPRCS[index - 1];
                    double extraPersons = (PRCS[prevStation].passOut(time) + PRCS[prevStation].passIn(time));
                    return extraPersons + (onTrain(prevStation, time, dir));
                }
            }
        }

        public void print(double time, Routes.Dir dir)
        {
            foreach (string s in stationsCSPR)
            {
                Console.WriteLine("{0}: {1}", s, departPercentage(s, time, dir));
            }
        }


        class StationArtInput
        {
            Dictionary<int, double> passIns;
            Dictionary<int, double> passOuts;
            Dictionary<int, double> fromTo;
            public StationArtInput()
            {
                passIns = new Dictionary<int, double>();
                passOuts = new Dictionary<int, double>();
                fromTo = new Dictionary<int, double>();
            }
            public void addInput(int from, double to, double passIn, double passOut)
            {
                passIns[from] = passIn;
                passOuts[from] = passOut;
                fromTo[from] = to;
            }

            public double avgInPerHour(double StartTime)
            {
                int from = timeToFrom(StartTime);
                return (double)passIns[from] / (double)(fromTo[from] - from);

            }
            public double passOut(double time)
            {
                return passOuts[timeToFrom(time)];
            }
            public double passIn(double time)
            {
                return passIns[timeToFrom(time)];
            }



            public int timeToFrom(double time)
            {
                foreach (var kvp in fromTo)
                {
                    if (kvp.Key * 60 * 60 <= time && time < kvp.Value * 60 * 60)
                    { return kvp.Key; }
                }

                throw new Exception("Invalid time voor Artificeele input");
            }

            public void print()
            {
                foreach (var kvp in passIns)
                {
                    Console.WriteLine("{0}-{1} : {2}", kvp.Key, fromTo[kvp.Key], kvp.Value);
                }
            }
        }
    }
}