using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator
{
    static class Parser
    {

        public static List<PassengerCount> ParsePassengerCounts(Stream s)
        {
            List<PassengerCount> passengerCounts = new List<PassengerCount>();

            using (StreamReader sr = new StreamReader(s))
            {
                String line;
                //sr.ReadLine(); //throw away Top column
                List<string> stations = parseStations(sr.ReadLine()); //parse column line
                while((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(new char[] { ';' });
                    string trip = values[0];

                    DateTime date = DateTime.Parse(values[1],new CultureInfo("nl-NL")); //Parse as dd-MM-yyyy
                    DateTime time = DateTime.Parse(values[2], new CultureInfo("nl-NL")); //Parse as HH:mm

                    List<int> enteringNumbers = values.Skip(3).Take(7).Select(x => Int32.Parse(x)).ToList();
                    List<int> departingNumbers = values.Skip(10).Select(x => Int32.Parse(x)).ToList();

                    //Constructs dictionaries using stations as keys and the passengers leaving and 
                    //entering as values
                    var enteringStations = stations.Zip(enteringNumbers, (k, v) => new { k, v })
                                                    .ToDictionary(x => x.k, x => x.v);
                    var departingStations = stations.Zip(departingNumbers, (k, v) => new { k, v })
                                                    .ToDictionary(x => x.k, x => x.v);

                    PassengerCount passengerCount = new PassengerCount();
                    passengerCount.Trip = trip;
                    passengerCount.Date = date;
                    passengerCount.Time = time;
                    passengerCount.EnteringCounts = enteringStations;
                    passengerCount.DepartingCounts = departingStations;

                    passengerCounts.Add(passengerCount);
                }
            }

            return passengerCounts;
        }

        private static List<string> parseStations(string line)
        {
            String[] columns = line.Split(new char []{ ';' });
            return columns.Skip(3).Take(9).ToList(); //Throw away Trip/Date/Depart and no need for duplicate stations
        }
    }
}
