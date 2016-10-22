using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator.States
{
    public class Station
    {
        readonly String _name;
        public String Name { get { return _name; } }

        public Queue<int> WaitingPersonsToPR { get; set; }
        public Queue<int> WaitingPersonsToCS { get; set; }
        public Queue<int> WaitingTramsToPR { get; set; }
        public Queue<int> WaitingTramsToCS { get; set; }
        public Queue<int> EnterTrackQueue { get; set; }
        public bool TramIsStationedPR { get; set; }
        public int? TramAtPR { get; set; }
        public bool TramIsStationedCS { get; set; }
        public int? TramAtCS { get; set; }
        public int lastTramPR { get; set; }
        public int lastTramCS { get; set; }

        public Station(String name)
        {
            this._name = name;
            this.WaitingPersonsToPR = new Queue<int>();
            this.WaitingPersonsToCS = new Queue<int>();
            this.WaitingTramsToPR = new Queue<int>();
            this.WaitingTramsToCS = new Queue<int>();
            this.EnterTrackQueue = new Queue<int>();
            this.TramIsStationedPR = false;
            this.TramIsStationedCS = false;
            lastTramCS = -1;
            lastTramPR = -1;
        }

        public static bool IsEndStation(string name)
        {
            return name == "PR" || name == "CS";
        }

        public void PrintQueues(SimulationState simState)
        {
            var sw = simState.sw;
            var crossing = simState.GetCrossing(Name);
            if (WaitingTramsToPR.Count > 0)
            {
                sw.Write("Waiting Trams to PR: ");
                WaitingTramsToPR.ToList().ForEach(x => sw.WriteLine(x + " "));
                sw.WriteLine();
            }

            if (WaitingTramsToCS.Count > 0)
            {
                sw.Write("Waiting Trams to CS: ");
                WaitingTramsToCS.ToList().ForEach(x => sw.WriteLine(x + " "));
                sw.WriteLine();
            }

            if (EnterTrackQueue.Count > 0)
            {
                sw.Write("Waiting Trams to EnterTrack: ");
                EnterTrackQueue.ToList().ForEach(x => sw.Write(x + " "));
                sw.WriteLine();
            }

            if (crossing.WaitingQueue.Count > 0)
            {
                sw.Write("Crossing queue: ");
                crossing.WaitingQueue.ToList().ForEach(x => sw.Write(x + " "));
                sw.WriteLine();
            }

            if(WaitingTramsToCS.Count == 0 && WaitingTramsToPR.Count == 0 && EnterTrackQueue.Count == 0 && crossing.WaitingQueue.Count == 0)
            {
                sw.WriteLine("All queues are empty");
            }

        }
        
    }
}
