using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TramSimulator;

namespace TramSimulator.States
{
    // State of Tram includes station i and if it is on track between i and i+1
    [Serializable]
    public class Tram
    {
        public enum TramState { AtStation, Waiting, OnTrack, Delayed, AtShuntyard };

        public string Station { get; set; }
        public TramState State { get; set; }
        public const int CAPACITY = 420;
        readonly int _tramId;
        public int TramId { get { return _tramId; } }
        public double DepartureTime { get; set; }
        public List<int> PersonsOnTram { get; set; }
        public Routes.Dir Direction { get; set; }
        public List<Tuple<double,string>> Times { get; set; }

        public Tram(int tramId, double departureTime)
        {
            this._tramId = tramId;
            this.DepartureTime = departureTime;
            this.PersonsOnTram = new List<int>();
            this.Direction = Routes.Dir.ToCS;
            this.Times = new List<Tuple<double,string>>();

        }
        public List<int> EmptyTram(double emptyRate)
        {
            var personsLeaving = PersonsOnTram.Where(x => Generate.uniform(0, 1) < emptyRate).ToList();
            PersonsOnTram = PersonsOnTram.Except(personsLeaving).ToList();

            return personsLeaving;
        }
        public List<int> FillTram(Queue<int> waitingPersons, double fillRate)
        {
            var entered = new List<int>();

            //The fillrate should never be higher than the number of waiting persons at a station
            //If that happens anyway, then we are okay with getting an exception
            while (PersonsOnTram.Count < CAPACITY && fillRate > 0 && waitingPersons.Count > 0)
            {
                int p = waitingPersons.Dequeue();
                PersonsOnTram.Add(p);
                fillRate -= 1;
                entered.Add(p);
            }

            return entered;

        }
    }
}
