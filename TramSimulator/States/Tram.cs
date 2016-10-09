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
        public void EmptyTram(double emptyRate)
        {
            int i = (int)(PersonsOnTram.Count * emptyRate);
            for (int j = 0; j < i; j++)
            {
                PersonsOnTram.RemoveAt((int)Generate.uniform(0, PersonsOnTram.Count - 1));
            }
        }
        public void FillTram(Queue<Person> waitingPersons, double fillRate)
        {
            //The fillrate should never be higher than the number of waiting persons at a station
            //If that happens anyway, then we are okay with getting an exception
            while (PersonsOnTram.Count < CAPACITY && fillRate > 0 && waitingPersons.Count > 0)
            {
                Person p = waitingPersons.Dequeue();
                PersonsOnTram.Add(p);
                fillRate -= 1;
            }

        }
    }
}
