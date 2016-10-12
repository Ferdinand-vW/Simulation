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

        public Person(int pId, double arrivalTime)
        {
            this.PersonId = pId;
            this.ArrivalTime = arrivalTime;
        }

        public void SetWaitingTime(double time)
        {
            WaitingTime = time - ArrivalTime;
        }
    }
}
