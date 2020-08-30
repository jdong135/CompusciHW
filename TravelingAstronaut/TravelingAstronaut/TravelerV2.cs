using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingAstronaut
{
    class TravelerV2
    {
        int startIndex, endIndex;
        Starfield starfield;
        public double minDistance = double.MaxValue;
        public Path bestPath = null;
        public int[] points;
        public int ID;

        public TravelerV2(int startIndex, int endIndex, Starfield starfield, int[] points, int ID)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.starfield = starfield;
            this.points = (int[]) points.Clone();
            this.ID = ID;
            
        }
        public void computeDistance()
        {
            do
            {
                //For showing asynchrony for level 2
                Console.WriteLine("Thread: " + ID);
                var path = new Path(points);
                if (points[0] >= startIndex && points[0] <= endIndex)
                {
                    double length = path.TotalDistance(starfield);
                    if (length < minDistance)
                    {
                        minDistance = length;
                        bestPath = path;
                    }
                }
            } while (DongUtility.UtilityFunctions.NextPermutation(points));


            //for (int i = startIndex; i < endIndex; i++)
            //{
            //    var path = new Path(permutations[i]);
            //    double length = path.TotalDistance(starfield);
            //    if (length < minDistance)
            //    {
            //        minDistance = length;
            //        bestPath = path;
            //    }
            //}
        }
    }
}
