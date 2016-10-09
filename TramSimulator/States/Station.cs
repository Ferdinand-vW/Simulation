using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator.States
{
    public class Station
    {
        public Queue<Person> WaitingPersonsA { get; set; }
        public Queue<Person> WaitingPersonsB { get; set; }
        public Queue<int> WaitingTramsA { get; private set; }
        public Queue<int> WaitingTramsB { get; private set; }
        readonly String _name;
        public String Name { get { return _name; } }
        public bool TramIsStationedA { get; set; }
        public bool TramIsStationedB { get; set; }

        public Station(String name)
        {
            this._name = name;
            this.WaitingPersonsA = new Queue<Person>();
            this.WaitingTramsA = new Queue<int>();
            this.TramIsStationedA = false;
            this.WaitingPersonsB = new Queue<Person>();
            this.WaitingTramsB = new Queue<int>();
            this.TramIsStationedB = false;
        }

        public bool WaitingTrams(bool typeA, int tramId)
        {
            if (typeA)
                return (WaitingTramsA.Count > 0) ? ((WaitingTramsA.Peek() != tramId)) : false;
            else
                return (WaitingTramsB.Count > 0) ? ((WaitingTramsB.Peek() != tramId)) : false;
        }

        public bool WaitingTrams(bool typeA)
        {
            if (typeA)
                return (WaitingTramsA.Count > 0);
            else
                return (WaitingTramsB.Count > 0);
        }

        public int Dequeue(bool typeA)
        {
            if (typeA)
            {
                return WaitingTramsA.Dequeue();
            }
            else {
                return WaitingTramsB.Dequeue();
            }
        }

        public bool TramIsStationed(bool typeA)
        {
            return typeA ? TramIsStationedA : TramIsStationedB;
        }

    }
}
