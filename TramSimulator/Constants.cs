using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator
{
    public class Constants
    {
        public const int BEGIN_TIME = 6 * 60 * 60; //6AM
        public const int END_TIME = 21 * 60 * 60 + 30 * 60; //9.30PM
        public const int SECONDS_IN_HOUR = 3600;
        public const int SECONDS_IN_MINUTE = 60;
        public const int ONE_WAY_DRIVING_TIME = 17 * 60;
        public const int TIME_IN_BETWEEN = 40;
        public const int ACTUAL_TURNAROUND_TIME = 180;
        public const string CS = "CS";
        public const string PR = "PR";
        public const string SHUNTYARD = "shuntyard";
    }
}
