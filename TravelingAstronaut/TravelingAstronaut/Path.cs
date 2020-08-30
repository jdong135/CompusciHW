using DongUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingAstronaut
{
    /// <summary>
    /// A simple path that contains a set of indices in a fixed order
    /// </summary>
    public class Path
    {
        /// <summary>
        /// The points in the path, as indices to the Starfield's Points list.
        /// </summary>
        public List<int> Points { get; } = new List<int>();
        static object locker = new object();

        public Path()
        { }

        public Path(params int[] points)
        {
            Points.AddRange(points);
        }

        public override string ToString()
        {
            string response = "{ ";
            foreach (var point in Points)
            {
                response += (point + ", ");
            }

            // truncate final comma
            response = response.Substring(0, response.Length - 2);
            response += " }";

            return response;
        }

        /// <summary>
        /// The total length of the path
        /// </summary>
        /// <param name="starfield">Needed because the path stores only integers, not vectors</param>
        public double TotalDistance(Starfield starfield)
        {
            double total = 0;
            //for (int i = 0; i < Points.Count - 1; i++)
            //    total += Distance(Points[i], Points[i + 1], starfield);
            for(int i=0; i<Points.Count - 1; i++)
            {
                if (starfield.Distances[Points[i], Points[i+1]] == null)
                {
                    total += Distance(Points[i], Points[i + 1], starfield);
                }
                else
                {
                    total += (double) starfield.Distances[Points[i], Points[i + 1]];
                }
            }

            return total;
        }
        
        /// <summary>
        /// The distance between two points, given as indices into the Starfield's list
        /// </summary>
        private double Distance(int p1, int p2, Starfield starfield)
        {
            double dist = Vector.Distance(starfield.Points[p1], starfield.Points[p2]);
            lock (locker)
            {
                    starfield.Distances[p1, p2] = dist;
                    starfield.Distances[p2, p1] = dist;
            }
            return dist;
        }
    }
}
