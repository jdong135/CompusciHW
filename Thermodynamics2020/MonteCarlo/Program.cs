using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteCarlo
{
    class Program
    {
        static void Main(string[] args)
        {
            const double radius = 1;
            const int nPoints = 1000000000;

            Random generator = new Random();

            int nInsideCircle = 0;

            for (int i = 0; i < nPoints; ++i)
            {
                double xCoord = generator.NextDouble() * 2 * radius - radius;
                double yCoord = generator.NextDouble() * 2 * radius - radius;

                if (xCoord * xCoord + yCoord * yCoord <= radius * radius)
                {
                    ++nInsideCircle;
                }
                     
            }

            const double areaOfSquare = (2 * radius) * (2 * radius);
            double ratio = (double)nInsideCircle / nPoints;
            double areaOfCircle = ratio * areaOfSquare;

            Console.WriteLine("Area of circle: " + areaOfCircle);
            Console.ReadKey();
        }
    }
}
