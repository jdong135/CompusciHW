using System;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace TravelingAstronaut
{
    class Program
    {
        static List<int[]> AllPermutations(int[] points, int numPermutations)
        {
            // Generate all possible paths
            List<int[]> permutations = new List<int[]>();
            
            do
            {
                int[] addThis = new int[points.Length];
                int index = 0;
                foreach(int i in points)
                {
                    addThis[index] = i;
                    index += 1;
                }
                permutations.Add(addThis);
            } while (DongUtility.UtilityFunctions.NextPermutation(points));
            //for (int i = 0; i < permutations.Count; i++)
            //{
            //    foreach (int integ in permutations[i])
            //    {
            //        Console.Write(integ + " ");
            //    }
            //    Console.WriteLine();
            //}
            return permutations;
        }
        static void Level1(int nPoints, Starfield starfield, int numThreads)
        {
            // Create an array in order
            var points = new int[nPoints];
            for (int i = 0; i < nPoints; ++i)
            {
                points[i] = i;
            }
            //int numPermutations = 1;
            //int count = 1;
            //while (count <= nPoints)
            //{
            //    numPermutations *= count;
            //    count += 1;
            //}
            //List<int[]> permutations = AllPermutations(points, numPermutations);
            List<TravelerV2> travelerV2s = new List<TravelerV2>();
            List<Thread> threads = new List<Thread>();
            var watch = Stopwatch.StartNew();
            double spacing = (double) nPoints / numThreads;
            for (int i = 0; i<numThreads; i++)
            {
                //If you had 10 points and 3 threads, spacing = 3.33
                //[0,3.33->3], [3,6.66->7], [7, 10]
                travelerV2s.Add(new TravelerV2((int) Math.Round(spacing * i), (int) Math.Round(spacing * (i + 1)), starfield, points, i));
                threads.Add(new Thread(travelerV2s[i].computeDistance));
                threads[i].Start();
            }
            foreach(Thread thread in threads)
            {
                thread.Join();
            }
            var time = watch.ElapsedMilliseconds;
            Console.WriteLine(time / 1000.0 + "s");
            double minDistance = double.MaxValue;
            Path bestPath = null;
            foreach(TravelerV2 traveler in travelerV2s)
            {
                if(traveler.minDistance < minDistance)
                {
                    minDistance = traveler.minDistance;
                    bestPath = traveler.bestPath;
                }
            }
            Console.WriteLine(minDistance);
            Console.WriteLine(bestPath);
        }
        static void Main(string[] args)
        {
            const int myStudentID = 120061;
            const int nPoints = 10;

            var generator = new StarfieldGenerator(myStudentID);

            // Create the starfield
            var starfield = generator.GeneratePoints(nPoints);

            Level1(nPoints, starfield, 5);
            //// Set up timer
            //var watch = Stopwatch.StartNew();

            //// Create an array in order
            //var points = new int[nPoints];
            //for (int i = 0; i < nPoints; ++i)
            //    points[i] = i;

            //double minValue = double.MaxValue;
            //Path bestPath = null;
            //int counter = 0;

            //// Loop over all possible choices
            //do
            //{
            //    var path = new Path(points);
            //    double length = path.TotalDistance(starfield);
            //    if (length < minValue)
            //    {
            //        minValue = length;
            //        bestPath = path;
            //    }
            //    ++counter;
            //} while (DongUtility.UtilityFunctions.NextPermutation(points));

            //// Get time on timer
            //var time = watch.ElapsedMilliseconds;

            //Console.WriteLine("Minimum length: " + minValue);
            //Console.WriteLine("Minimum path: " + bestPath);
            //Console.WriteLine(counter + " tries");
            //Console.WriteLine("Time elapsed: " + time / 1000.0 + " s");
            //Console.ReadKey();
        }
    }
}
