using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator.States;

namespace TramSimulator.Events
{
    public class EnterTrack : Event
    {
        int _tramId;
        string _station;
        int q ;
        public EnterTrack(int tramId, double startTime, string station, int q)
        {
            this._tramId = tramId;
            this._station = station;
            this.StartTime = startTime;
            this.q = q;
        }

        public override void execute(SimulationState simState)
        {
            //Insert the tram into the route
            var route = simState.Routes.PRToCentral;
            route[0].Trams.Add(_tramId);
            var tram = simState.Trams[_tramId];
            tram.State = Tram.TramState.AtStation;
            //Add an initial event
            var eventQueue = simState.EventQueue;
            
            simState.Stations[_station].TramIsStationedCS = true;
            simState.TimeTables[_tramId] = new TimeTable(StartTime,q);
            eventQueue.AddEvent(new TramExpectedDeparture(_tramId, StartTime, _station));
        }

        public override string ToString()
        {
            return "Tram " + _tramId + " enters " + _station + " at " + StartTime;
        }
    }
}
