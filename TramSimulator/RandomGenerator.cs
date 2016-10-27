using System;

namespace TramSimulator
{
    //Singleton class for generating random numbers according to some distribution
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
        
        static public double NegExp(double u)
        {
            return Math.Log(1 - Instance.random.NextDouble()) / (-u);
        }
        public static double LogNormalWithoutVariance(double mean) {
            return LogNormal(mean * 1.578399, mean);
        }

        public static double LogNormal(double variance, double mean)
        {
            //  mean and standard deviation of the variable’s natural logarithm
            // https://en.wikipedia.org/wiki/Log-normal_distribution
            double mu = Math.Log(mean / Math.Sqrt(1 + (variance / (mean * mean))));
            double sigma = Math.Sqrt(Math.Log(1 + (variance / (mean * mean))));

            return Math.Exp(Normal(sigma, mu));
        }

        public static double Normal(double sigma, double mu)
        {
            double u1 = Instance.random.NextDouble();
            double u2 = Instance.random.NextDouble();
            double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mu + (sigma * normal);
        }

        static public double Uniform(double low, double high)
        {
            return Instance.random.NextDouble() * (high - low) + low;
        }
    }
}
