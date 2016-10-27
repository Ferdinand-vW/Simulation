using System;
using System.Collections.Generic;

namespace TramSimulator.InputModels
{
    public class PassengerCount
    {
        public string Trip { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public Dictionary<string, int> EnteringCounts { get; set; }
        public Dictionary<string, int> DepartingCounts { get; set; }
    }
}
