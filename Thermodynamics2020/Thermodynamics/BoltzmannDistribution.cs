using DongUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thermodynamics
{
    // For level 2
    public class BoltzmannDistribution : RandomGenerator
    {
        private double min;
        private double max;
        private double temperature;
        public BoltzmannDistribution(ParticleContainer cont, double min, double max, double temperature) :
            base(cont)
        {
            this.min = min;
            this.max = max;
            this.temperature = temperature;
        }
        protected override double GetSpeed(ParticleInfo info)
        {
            bool returnedValue = false;
            double randomSpeed = 0;
            while(returnedValue == false)
            {
                double boltzmannConstant = 1.38 * Math.Pow(10, -23);
                // Pick speed between min and max
                randomSpeed = RandomGen.NextDouble(min, max);
                // Find probability of getting that speed
                double probability = 4 * Math.PI * Math.Pow((info.Mass / (2 * Math.PI * boltzmannConstant * temperature)), 1.5);
                probability *= randomSpeed * randomSpeed * Math.Pow(Math.E, -(info.Mass * randomSpeed * randomSpeed) / (2 * boltzmannConstant * temperature));
                // generate random double between 0,1. If <= probability, use that velocity. If not try again.
                double randNum = RandomGen.NextDouble();
                if (randNum <= probability)
                {
                    returnedValue = true;
                }
            }
            return randomSpeed;
        }
    }
}
