using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator;

namespace TramSimulator.States
{
    // State of Tram includes station i and if it is on track between i and i+1
    public class Tram
    {
        public enum TramState { AtStation, Waiting, OnTrack, Delayed, AtShuntyard };

        public string Station { get; set; }
        public TramState State { get; set; }
        public const int CAPACITY = 420;
        readonly int _tramId;
        public int TramId { get { return _tramId; } }
        public double DepartureTime { get; set; }
        public List<Person> PersonsOnTram { get; set; }
        public Routes.Dir Direction { get; set; }

        public Tram(int tramId, double departureTime)
        {
            this._tramId = tramId;
            this.DepartureTime = departureTime;
            this.PersonsOnTram = new List<Person>();
            this.Direction = Routes.Dir.ToCS;

        }
        public int EmptyTram(double emptyRate)
        {
            int n = 0;
            for (int i = 0; i < PersonsOnTram.Count; i++)
            {
                if (Generate.uniform(0, 1) <= emptyRate)
                {
                    n++;
                    PersonsOnTram.RemoveAt(i);
                }
            }
            return n;
        }
        public int FillTram(Queue<Person> waitingPersons)
        {
            int n = 0;
            while (PersonsOnTram.Count < CAPACITY && waitingPersons.Count > 0)
            {
                n++;
                Person p = waitingPersons.Dequeue();
                PersonsOnTram.Add(p);
            }
            return n;
        }
    }
}
