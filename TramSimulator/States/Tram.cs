using System.Collections.Generic;
using System.Linq;

namespace TramSimulator.States
{
    public class Tram
    {
        public enum TramState { AtStation, Waiting, OnTrack, Delayed, AtShuntyard };
        public const int CAPACITY = 420;

        //Tram identier
        readonly int _tramId;
        public int TramId { get { return _tramId; } }

        //Tram state
        public string Station { get; set; }
        public TramState State { get; set; }
        public double DepartureTime { get; set; }
        public List<int> PersonsOnTram { get; set; }
        public Routes.Dir Direction { get; set; }

        public Tram(int tramId, double departureTime)
        {
            this._tramId = tramId;
            this.DepartureTime = departureTime;
            this.PersonsOnTram = new List<int>();
            this.Direction = Routes.Dir.ToCS;
        }

        //Empty the tram given some empty percentage
        public List<int> EmptyTram(double emptyRate)
        {
            //Randomly choose passengers to leave the tram
            var personsLeaving = PersonsOnTram.Where(x => Generate.Uniform(0, 1) < emptyRate).ToList();
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
