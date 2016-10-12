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

        public ArtInput()
        {
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

        public void print()
        {
            foreach (var kvp in PRCS)
            {
                Console.WriteLine(kvp.Key);
                kvp.Value.print();
            }
            foreach (var kvp in CSPR)
            {
                Console.WriteLine(kvp.Key);
                kvp.Value.print();
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

            public double avgOutPerHour(double StartTime)
            {
                int from = timeToFrom(StartTime);
                return (double)passOuts[from] / (double)(fromTo[from] - from);
            }


            public int timeToFrom(double StartTime)
            {
                int from;
                if (StartTime > 18 * 60 * 60) { from = 18; }
                else if (StartTime > 16 * 60 * 60) { from = 16; }
                else if (StartTime > 9 * 60 * 60) { from = 9; }
                else if (StartTime > 7 * 60 * 60) { from = 7; }
                else { from = 6; }

                return from;
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