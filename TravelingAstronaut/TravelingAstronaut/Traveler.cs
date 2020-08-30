using System;
using System.Collections.Generic;
using System.Text;

namespace TravelingAstronaut
{
    class Traveler
    {
        int startIndex, endIndex;
        Starfield starfield;
        List<int[]> permutations;
        public double minDistance = double.MaxValue;
        public Path bestPath = null;

        public Traveler(int startIndex, int endIndex, Starfield starfield, List<int[]> permutations)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.starfield = starfield;
            this.permutations = permutations;
        }
        public void computeDistance()
        {
            for(int i = startIndex; i< endIndex; i++)
            {
                var path = new Path(permutations[i]);
                double length = path.TotalDistance(starfield);
                if (length < minDistance)
                {
                    minDistance = length;
                    bestPath = path;
                }
            }
        }
    }
}
