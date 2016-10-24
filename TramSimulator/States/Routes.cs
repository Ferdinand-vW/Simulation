using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator.States
{
    [Serializable]
    public class Routes
    {
        public List<Track> CentralToPR { get; set; }
        public List<Track> PRToCentral { get; set; }

        public enum Dir { ToPR, ToCS };

        public Routes(List<Track> centralToPR, List<Track> prToCentral)
        {
            this.CentralToPR = centralToPR;
            this.PRToCentral = prToCentral;
        }

        public int? NextTram(int tramId, string depStation, SimulationState simState)
        {
            Track t = GetTrack(tramId);
            return NextTram(t, tramId, depStation, simState);   
        }

        public int? NextTram(Track t, int tramId, string depStation, SimulationState simState)
        {
            int index = t.Trams.IndexOf(tramId);

            if (index == t.Trams.Count - 1) { return null; }
            else { return t.Trams[index + 1]; }
        }

        public string NextStation(int tramId, string station)
        {
            Track t = GetTrack(tramId);
            if(station == "PR" || station == "CS")
            {
                var t2 = GetNextTrack(station, t.From, t);
                if(t.To == "CS" && t2.From != "CS" || (t.To == "PR" && t2.From != "PR"))
                {
                    Console.WriteLine();
                }
            }
            else
            {
                var t2 = GetNextTrack(station, t.From, t);
                if(t.To != t2.From)
                {
                    Console.WriteLine();
                }
            }
            if(t == null) { throw new Exception(); }
            if (t.To == station)
            {
                return GetNextTrack(station, t.From, t).To;
            }
            else
            {
                return t.To;
            }
        }

        public static bool ToCS(Dir dir)
        {
            return dir == Dir.ToCS;
        }

        public static bool ToPR(Dir dir)
        {
            return dir == Dir.ToPR;
        }

        public void MoveToNextTrack(int tramId, string depStation, SimulationState simState)
        {
            Track t = GetTrack(tramId);
            //Remove the tram if it was previously on a track
            /*if (t.Trams.Last() != tramId)
            {
                simState.sw.Flush();
                simState.sw.Close();
                Console.WriteLine("test");
            }*/

            if(t != null) { t.Trams.Remove(tramId); }

            //Find the next track and insert it into the next track
            Track nextTrack = GetNextTrack(depStation, t.From, t);
            nextTrack.Trams.Insert(0,tramId);
            if((depStation == "WKZ" && t.From == "PR" && t.To == "WKZ" && nextTrack.From != "WKZ") || (depStation == "WKZ" && t.From != "UMC" && t.To == "WKZ" && nextTrack.From != "WKZ") ||
                (depStation == "UMC" && t.From != "Heidelberglaan" && t.To == "UMC" && nextTrack.From != "UMC") || (depStation == "Heidelberglaan" && t.From != "Padualaan" && t.To == "Heidelberglaan" && nextTrack.From != "Heidelberglaan") ||
                (depStation == "Padualaan" && t.From != "Kromme Rijn" && t.To == "Padualaan" && nextTrack.From != "Padualaan") || (depStation == "Kromme Rijn" && t.From != "Galgenwaard" && t.To == "Kromme Rijn" && nextTrack.From != "Kromme Rijn") ||
                (depStation == "Galgenwaard" && t.From != "Vaartscherijn" && t.To == "Galgenwaard" && nextTrack.From != "Galgenwaard") || (depStation == "Vaartscherijn" && t.From != "CS" && t.To == "Vaartscherijn" && nextTrack.From != "Vaartscherijn") ||
                (depStation == "CS" && t.From != "Vaartscherijn" && t.To == "CS" && nextTrack.From != "CS") || (depStation == "Vaartscherijn" && t.From != "CS" && t.To == "Vaartscherijn" && nextTrack.From != "Vaartscherijn") ||
                (depStation == "Galgenwaard" && t.From != "Vaartscherijn" && t.To == "Galgenwaard" && nextTrack.From != "Galgenwaard") || (depStation == "Kromme Rijn" && t.From != "Galgenwaard" && t.To == "Kromme Rijn" && nextTrack.From != "Kromme Rijn") ||
                (depStation == "Padualaan" && t.From != "Kromme Rijn" && t.To == "Padualaan" && nextTrack.From != "Padualaan") || (depStation == "Heidelberglaan" && t.From != "Padualaan" && t.To == "Heidelberglaan" && nextTrack.From != "Heidelberglaan") ||
                (depStation == "UMC" && t.From != "Heidelberglaan" && t.To == "UMC" && nextTrack.From != "UMC") || (depStation == "WKZ" && t.From != "UMC" && t.To == "WKZ" && nextTrack.From != "WKZ") ||
                (depStation == "PR" && t.From != "WKZ" && t.To == "PR" && nextTrack.From != "PR"))
            {
                Console.WriteLine();
            }
            if(depStation != t.To || nextTrack.From != depStation || t.To != nextTrack.From || (t.To == "CS" && nextTrack.From != "CS") || (t.To == "PR" && nextTrack.From != "PR"))
            {
                Console.WriteLine();
            }
            if ((depStation != "Vaartscherijn" && GetTrack(tramId).From == "Vaartscherijn" && GetTrack(tramId).To == "Galgenwaard") ||
                (depStation != "CS" && GetTrack(tramId).From == "CS" && GetTrack(tramId).To == "Vaartscherijn") ||
                (depStation != "Vaartscherijn" && GetTrack(tramId).From == "Vaartscherijn" && GetTrack(tramId).To == "CS") ||
                (depStation != "Galgenwaard" && GetTrack(tramId).From == "Galgenwaard" && GetTrack(tramId).To == "Vaartscherijn") ||
                (depStation != "Kromme Rijn" && GetTrack(tramId).From == "Kromme Rijn" && GetTrack(tramId).To == "Galgenwaard") ||
                (depStation != "Padualaan" && GetTrack(tramId).From == "Padualaan" && GetTrack(tramId).To == "Kromme Rijn") ||
                (depStation != "Heidelberglaan" && GetTrack(tramId).From == "Heidelberglaan" && GetTrack(tramId).To == "Padualaan") ||
                (depStation != "UMC" && GetTrack(tramId).From == "UMC" && GetTrack(tramId).To == "Heidelberglaan") ||
                (depStation != "WKZ" && GetTrack(tramId).From == "WKZ" && GetTrack(tramId).To == "UMC") ||
                (depStation != "PR" && GetTrack(tramId).From == "PR" && GetTrack(tramId).To == "WKZ") ||
                (depStation != "WKZ" && GetTrack(tramId).From == "WKZ" && GetTrack(tramId).To == "PR") ||
                (depStation != "UMC" && GetTrack(tramId).From == "UMC" && GetTrack(tramId).To == "WKZ") ||
                (depStation != "Heidelberglaan" && GetTrack(tramId).From == "Heidelberglaan" && GetTrack(tramId).To == "UMC") ||
                (depStation != "Padualaan" && GetTrack(tramId).From == "Padualaan" && GetTrack(tramId).To == "Heidelberglaan") ||
                (depStation != "Kromme Rijn" && GetTrack(tramId).From == "Kromme Rijn" && GetTrack(tramId).To == "Padualaan") ||
                (depStation != "Galgenwaard" && GetTrack(tramId).From == "Galgenwaard" && GetTrack(tramId).To == "Kromme Rijn") ||
                (depStation != "Vaartscherijn" && GetTrack(tramId).From == "Vaartscherijn" && GetTrack(tramId).To == "Galgenwaard"))
            {
                Console.WriteLine();
            }

        }

        public Track GetTrack(int tramId)
        {
            var track1 = CentralToPR.Find(x => x.Trams.Contains(tramId));
            var track2 = PRToCentral.Find(x => x.Trams.Contains(tramId));

            return (track1 == null ? track2 : track1);
        }

        public bool OnA(int tramId)
        {
            var track1 = CentralToPR.Find(x => x.Trams.Contains(tramId));
            return track1 == null ;
        }

        public Track GetNextTrack(string depStation, string prevStation, Track t)
        {
            //Look for the track in CS - PR
            Track nextT = CentralToPR.Find(x => x.From == depStation && x.To != prevStation);
            if(nextT == null)
            {
                //If it is empty, then check in PR - CS
                nextT = PRToCentral.Find(x => x.From == depStation && x.To != prevStation);
            }
            //If both are empty, then the tram is at the last track of a route, so the next track would be the first
            //route in the other route
            if(nextT == null)
            {
                if(depStation == Constants.CS) {
                    nextT = CentralToPR[0]; }
                else if(depStation == Constants.PR) { nextT = PRToCentral[1]; }
                else { throw new Exception(); }
            }

            if(depStation == "PR" && nextT.To == "CS")
            {
                Console.WriteLine();
            }
            return nextT;
        }
    }

    [Serializable]
    public class Track
    {
        public string From { get; set; }
        public string To { get; set; }
        public List<int> Trams { get; set; }

        public Track(string from, string to)
        {
            this.From = from;
            this.To = to;
            this.Trams = new List<int>();
        }
    }
}
