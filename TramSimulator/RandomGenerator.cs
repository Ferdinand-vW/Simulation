using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TramSimulator
{
    
    class Generate
    {
        private static Generate _generator;
        private Random random = new Random();

        //Don't allow outside instantiation
        private Generate() { }

        //Creates a singleton instance when it is first called
        public static Generate Instance
        {
            get
            {
                if (_generator == null) { _generator = new Generate(); }
                return _generator;
            }
        }

        //Called with Generate.negexp(u)
        static public double negexp(double u)
        {
            //double lambda = (double)1 / u;
            return Math.Log(1 - Instance.random.NextDouble()) / (-u);
        }
        //Called with Generate.uniform(low,high)
        static public double uniform(double low, double high)
        {
            return Instance.random.NextDouble() * (high - low) + low;
        }
    }
}
