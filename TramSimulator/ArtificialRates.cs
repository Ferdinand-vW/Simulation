using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TramSimulator.States;

namespace TramSimulator
{
    public class ArtificialRates : AbstractSimulationRates
    {
        ArtInput _artInput;

        public ArtificialRates(ArtInput artInput)
        {
            _artInput = artInput;
        }

        public override bool nonZeroPercentage(double time, string station, Routes.Dir dir)
        {
            return _artInput.AvgInPerHour(station, time, dir) == 0;
        }

        public override double PersonArrivalRate(string station, Routes.Dir dir, double time)
        {
            return Generate.negexp(_artInput.AvgInPerHour(station, time, dir) / Constants.SECONDS_IN_HOUR);
        }

        public override double TramEmptyRate(DayOfWeek day, string station, Routes.Dir dir, Tram tram, double time)
        {
            return _artInput.DepartPercentage(station, time, dir);
        }
    }
}
