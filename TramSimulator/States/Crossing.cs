using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.Events;

namespace TramSimulator.States
{
    public class Crossing
    {
        public int? IsBeingCrossedBy { get; set; }
        //Assumption is that there is only a single switch that can be turned.
        //Furthermore, only a single tram can cross each time
        public Routes.Dir Switch { get; set; }
        public Queue<Event> WaitingQueue { get; set; }

        string _endStation;
        

        public Crossing(string endStation)
        {
            this.IsBeingCrossedBy = null;
            if(endStation == "PR") { this.Switch = Routes.Dir.ToPR; }
            else                   { this.Switch = Routes.Dir.ToCS; }
            this.WaitingQueue = new Queue<Event>();
            this._endStation = endStation;
        }
    }
}
