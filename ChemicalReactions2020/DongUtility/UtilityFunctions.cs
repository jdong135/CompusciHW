using System;

namespace DongUtility
{
    static public class UtilityFunctions
    {
        static public double DegreesToRadians(double degrees)
        {
            return degrees / 180 * Math.PI;
        }

        static public double RadiansToDegrees(double radians)
        {
            return radians / Math.PI * 180;
        }

        static public double Square(double input)
        {
            return input * input;
        }

        /// <summary>
        /// Raise any number to an integer power (including negative powers)
        /// Thanks to Wikipedia for the algorithm
        /// </summary>
        static public double Pow(double baseNum, int exponent)
        {
            // Calculate numbers in place to avoid unnecessary extra allocation
            if (exponent == 0)
                return 1;
            else if (exponent == 1)
                return baseNum;
            else if (exponent < 0)
                return 1 / Pow(baseNum, -exponent);

            double extraFactor = 1;
            while (exponent > 1)
            {
                if (exponent % 2 == 0)
                {
                    baseNum *= baseNum;
                    exponent /= 2;
                }
                else
                {
                    extraFactor *= baseNum;
                    baseNum *= baseNum;
                    exponent = (exponent - 1) / 2;
                }
            }
            return baseNum * extraFactor;
        }

        static public bool Between(double input, double low, double high)
        {
            return input > low && input < high;
        }

        static public bool BetweenInclusive(double input, double low, double high)
        {
            return input >= low && input <= high;
        }
    }
}
