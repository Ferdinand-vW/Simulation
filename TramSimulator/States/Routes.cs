using System;
using System.Collections.Generic;

namespace TramSimulator.States
{
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

        //Return the tram in front of this in the same route
        //returns null if there isn't one
        public int? NextTram(int tramId, string depStation)
        {
            Track t = GetTrack(tramId);
            return NextTram(t, tramId, depStation);   
        }

        public int? NextTram(Track t, int tramId, string depStation)
        {
            int index = t.Trams.IndexOf(tramId);

            if (index == t.Trams.Count - 1) { return null; }
            else { return t.Trams[index + 1]; }
        }

        public string NextStation(int tramId, string station)
        {
            Track t = GetTrack(tramId);
            
            if (t.To == station) { return GetNextTrack(station, t.From).To; }
            else { return t.To; }
        }

        public void MoveToNextTrack(int tramId, string depStation)
        {
            Track t = GetTrack(tramId);

            //Remove the tram if it was previously on a track
            if(t != null) { t.Trams.Remove(tramId); }

            //Find the next track and insert it into the next track
            Track nextTrack = GetNextTrack(depStation, t.From);
            nextTrack.Trams.Insert(0,tramId);
        }

        //Returns the track that contains this tram
        public Track GetTrack(int tramId)
        {
            var track1 = CentralToPR.Find(x => x.Trams.Contains(tramId));
            var track2 = PRToCentral.Find(x => x.Trams.Contains(tramId));

            return (track1 == null ? track2 : track1);
        }

        public Track GetNextTrack(string depStation, string prevStation)
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

            return nextT;
        }

        public static bool ToCS(Dir dir)
        {
            return dir == Dir.ToCS;
        }

        public static bool ToPR(Dir dir)
        {
            return dir == Dir.ToPR;
        }
    }

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
