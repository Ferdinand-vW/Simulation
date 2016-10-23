using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator.States
{
    public class Person
    {
        public int PersonId { get; private set; }
        public double ArrivalTime { get; }
        public double WaitingTime { get; private set; }
        public double EnteredTramTime { get; set; }
        public double LeaveTime { get; set; }
        public string ArrivedAt { get; set; }
        public string LeftAt { get; set; }
        public int QueueLengthAtArrival { get; set; }
        public List<Tuple<int,int>> PassedTrams { get; set; }

        public Person(int pId, double arrivalTime)
        {
            this.PersonId = pId;
            this.ArrivalTime = arrivalTime;
            this.PassedTrams = new List<Tuple<int, int>>();
        }

        public void SetWaitingTime(double time)
        {
            WaitingTime = time - ArrivalTime;
        }
    }
}
