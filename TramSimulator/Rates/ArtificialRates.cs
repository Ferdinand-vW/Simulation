using System;

using TramSimulator.States;
using TramSimulator.InputModels;

namespace TramSimulator.Rates
{
    //Contains rates specific to the artificial data
    public class ArtificialRates : AbstractSimulationRates
    {
        ArtInput _artInput;

        public ArtificialRates(ArtInput artInput)
        {
            _artInput = artInput;
        }

        public override bool NonZeroPercentage(double time, string station, Routes.Dir dir)
        {
            return _artInput.AvgInPerHour(station, time, dir) == 0;
        }

        public override double PersonArrivalRate(string station, Routes.Dir dir, double time)
        {
            return Generate.NegExp(_artInput.AvgInPerHour(station, time, dir) / Constants.SECONDS_IN_HOUR);
        }

        public override double TramEmptyRate(DayOfWeek day, string station, Routes.Dir dir, Tram tram, double time)
        {
            return _artInput.DepartPercentage(station, time, dir);
        }
    }
}
