using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CryptoSystemDissertation.BusinessLogic
{
    public class RandomParameters
    {
        private Random random = new Random();

        public double GenerateLambdaRandomNumber()
        {
            var number = random.NextDouble() * (4 - 3.58) + 3.58;
            return number;
        }

        public double GenerateXRandomNumber()
        {
            var number = Math.Round((random.NextDouble()), 2);
            return number;
        }
    }
}