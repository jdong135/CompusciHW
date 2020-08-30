using System;
using System.Collections;
using System.Collections.Generic;

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

        // Thanks to https://stackoverflow.com/questions/15486/sorting-an-ilist-in-c-sharp
        static public void Sort<T>(IList<T> list)
        {
            ArrayList.Adapter((IList)list).Sort();
        }

        // Thanks to https://www.nayuki.io/page/next-lexicographical-permutation-algorithm
        static public bool NextPermutation<T>(IList<T> list) where T : IComparable<T>
        {
            // Find non-increasing suffix
            int i = list.Count - 1;
            while (i > 0 && list[i - 1].CompareTo(list[i]) > 0)
                i--;
            if (i <= 0)
                return false;

            // Find successor to pivot
            int j = list.Count - 1;
            while (list[j].CompareTo(list[i - 1]) < 0)
                j--;
            T temp = list[i - 1];
            list[i - 1] = list[j];
            list[j] = temp;

            // Reverse suffix
            j = list.Count - 1;
            while (i < j)
            {
                temp = list[i];
                list[i] = list[j];
                list[j] = temp;
                i++;
                j--;
            }

            return true;
        }

    }
}
