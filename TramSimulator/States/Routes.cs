using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator.States
{
    public class Routes
    {
        public List<Track> CentralToPR { get; set; }
        public List<Track> PRToCentral { get; set; }

        public Routes(List<Track> centralToPR, List<Track> prToCentral)
        {
            this.CentralToPR = centralToPR;
            this.PRToCentral = prToCentral;
        }

        public int? NextTram(int tramId, string depStation)
        {
            Track t = GetTrack(tramId);
            int index = t.Trams.IndexOf(tramId);

            if (index == t.Trams.Count - 1) { return null; }
            else { return t.Trams[index + 1]; }
        }

        public string NextStation(int tramId, string station)
        {
            Track t = GetTrack(tramId);
            if (t.To == station)
            {
                return GetNextTrack(station, t.From).To;
            }
            else
            {
                return t.To;
            }
        }

        public void MoveToNextTrack(int tramId, string depStation)
        {
            Track t = GetTrack(tramId);
            //Remove the tram if it was previously on a track
            if(t != null) { t.Trams = t.Trams.Take(t.Trams.Count - 1).ToList(); }

            //Find the next track and insert it into the next track
            Track nextTrack = GetNextTrack(depStation, t.From);
            nextTrack.Trams.Insert(0,tramId);
        }

        public Track GetTrack(int tramId)
        {
            var track1 = CentralToPR.Find(x => x.Trams.Contains(tramId));
            var track2 = PRToCentral.Find(x => x.Trams.Contains(tramId));

            return (track1 == null ? track2 : track1);
        }

        private Track GetNextTrack(string depStation, string prevStation)
        {

            return CentralToPR.Union(PRToCentral).First(x => x.From == depStation && x.To != prevStation || (depStation == "CS" && x.From == "CS") || (depStation == "PR" && x.From == "PR"));
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
