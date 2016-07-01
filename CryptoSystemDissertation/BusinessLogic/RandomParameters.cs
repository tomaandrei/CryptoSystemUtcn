using System;

namespace CryptoSystemDissertation.BusinessLogic
{
    public class RandomParameters
    {
        private Random random = new Random();

        public double GenerateLambdaRandomNumber()
        {
            var number = random.NextDouble() * (3.99 - 3.58) + 3.58;
            return number;
        }

        public double GenerateXRandomNumber()
        {
            var number = Math.Round((random.NextDouble()), 2);
            return number;
        }

        public int GenerateTRandomNumber()
        {
            var number = 5;
            return number;
        }

        public int GenerateARandomNumber()
        {
            var number = 7;
            return number;
        }

        public int GenerateC0RandomNumber()
        {
            var number = 23;
            return number;
        }

        public int GenerateBRandomNumber()
        {
            var number = 4;
            return number;
        }
    }
}