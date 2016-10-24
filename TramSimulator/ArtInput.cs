using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TramSimulator.States;

namespace TramSimulator
{
    public class ArtInput
    {
        public Dictionary<string, StationArtInput> PRCS;
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

        public void AddInput(string station, int direction, int from, double to, double passIn, double passOut)
        {
            if (direction == 0)
            {
                if (!PRCS.ContainsKey(station)) { PRCS[station] = new StationArtInput(); }
                PRCS[station].AddInput(from, to, passIn, passOut);

            }
            else
            {
                if (!CSPR.ContainsKey(station)) { CSPR[station] = new StationArtInput(); }
                CSPR[station].AddInput(from, to, passIn, passOut);
            }
        }
        public double AvgInPerHour(string station, double time, Routes.Dir dir)
        {
            if (dir == Routes.Dir.ToPR)
            {
                return CSPR[station].AvgInPerHour(time);
            }
            else {
                return PRCS[station].AvgInPerHour(time);
            }
        }
        public double DepartPercentage(string station, double time, Routes.Dir dir)
        {
            if (dir == Routes.Dir.ToPR)
            {
                double numberOnTrain = OnTrain(station, time, dir);
                //If no one is on the train, Everybody gets out. 
                return numberOnTrain == 0 ? 1 : CSPR[station].PassOut(time) / numberOnTrain;
            }

            else {
                double numberOnTrain = OnTrain(station, time, dir);
                return numberOnTrain == 0 ? 1 : PRCS[station].PassOut(time) / numberOnTrain;
            }

        }

        // Calculates how many persons are on the tram at a given station at a given time
        public double OnTrain(string station, double time, Routes.Dir dir)
        {
            if (dir == Routes.Dir.ToPR)
            {
                int index = Array.IndexOf(stationsCSPR, station);
                if (index == 0) return 0;
                else {
                    string prevStation = stationsCSPR[index - 1];
                    // The amount of people got off and on the tram at the previous station
                    double extraPersons = ((-CSPR[prevStation].PassOut(time)) + CSPR[prevStation].PassIn(time));
                    // The amount of people on the train is calculated 
                    //with the amout of people where on the train at the previous station
                    return extraPersons + (OnTrain(prevStation, time, dir));
                }
            }
            else {
                int index = Array.IndexOf(stationsPRCS, station);
                if (index == 0) return 0;
                else {
                    string prevStation = stationsPRCS[index - 1];
                    double extraPersons = ((-PRCS[prevStation].PassOut(time)) + PRCS[prevStation].PassIn(time));
                    return extraPersons + (OnTrain(prevStation, time, dir));
                }
            }
        }

        public void Print(double time, Routes.Dir dir)
        {
            foreach (string s in stationsCSPR)
            {
                Console.WriteLine("{0}: {1}", s, DepartPercentage(s, time, dir));
            }
        }
    }

    public class StationArtInput
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
        public void AddInput(int from, double to, double passIn, double passOut)
        {
            passIns[from] = passIn;
            passOuts[from] = passOut;
            fromTo[from] = to;
        }

        public double AvgInPerHour(double StartTime)
        {
            int from = TimeToFrom(StartTime);
            return (double)passIns[from] / (double)(fromTo[from] - from);

        }
        public double PassOut(double time)
        {
            return passOuts[TimeToFrom(time)];
        }
        public double PassIn(double time)
        {
            return passIns[TimeToFrom(time)];
        }



        public int TimeToFrom(double time)
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