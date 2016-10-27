using System;
using System.Collections.Generic;

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
        public int MaxQueueLengthCS { get; set; }
        public int MaxQueueLengthPR { get; set; }

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
        }

        public static bool IsEndStation(string name)
        {
            return name == "PR" || name == "CS";
        }
    }
}
