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
            var val = Math.Log(1 - Instance.random.NextDouble()) / (-u);
            if(Double.IsNaN(val))
            {
                Console.WriteLine();
            }
            return val;
        }
        public static double logNormalWithoutVariance(double mean) {
            return logNormal(mean * 1.578399, mean);
        }

        public static double logNormal(double variance, double mean)
        {
            //  mean and standard deviation of the variable’s natural logarithm
            // https://en.wikipedia.org/wiki/Log-normal_distribution
            double mu = Math.Log(mean / Math.Sqrt(1 + (variance / (mean * mean))));
            double sigma = Math.Sqrt(Math.Log(1 + (variance / (mean * mean))));

            var val = Math.Exp(normal(sigma, mu));
            if(Double.IsNaN(val))
            {
                Console.WriteLine();
            }
            return val;
        }

        public static double normal(double sigma, double mu)
        {
            double u1 = Instance.random.NextDouble();
            double u2 = Instance.random.NextDouble();
            double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mu + (sigma * normal);
        }
        //Called with Generate.uniform(low,high)
        static public double uniform(double low, double high)
        {
            return Instance.random.NextDouble() * (high - low) + low;
        }
    }
}
