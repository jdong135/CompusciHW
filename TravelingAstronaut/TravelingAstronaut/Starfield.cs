using DongUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingAstronaut
{
    /// <summary>
    /// A class to store the positions of stars
    /// </summary>
    public class Starfield
    {
        public Starfield(List<Vector> points)
        {
            Points = points;
            Distances = new double?[points.Count, points.Count];
        }

        public List<Vector> Points { get; }
        public double?[,] Distances;
    }
}
