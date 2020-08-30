using DongUtility;
using System;
using System.Collections.Generic;

namespace TravelingAstronaut
{
    /// <summary>
    /// A class to generate starfields for use in the traveling astronaut problem
    /// </summary>
    public class StarfieldGenerator
    {
        private readonly Random random;

        /// <param name="studentID">Put your student ID here to seed the random number generator</param>
        public StarfieldGenerator(int studentID)
        {
            random = new Random(studentID);
        }

        private const double minRad = 0;
        private const double maxRad = 100;

        /// <summary>
        /// Generates a fixed number of points - note that the points are not uniformly distributed!
        /// </summary>
        public Starfield GeneratePoints(int nPoints)
        {
            var list = new List<Vector>();
            for (int i = 0; i < nPoints; ++i)
            {
                double radius = random.NextDouble(minRad, maxRad);
                list.Add(Vector.RandomDirection(radius, random));
            }
            return new Starfield(list);
        }
    }
}
