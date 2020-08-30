using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Shapes
{
    /// <summary>
    /// Creates a tubular shape for a given parametric function
    /// </summary>
    public class FunctionShape3D : Shape3D
    {
        /// <summary>
        /// A prototype for a parametric function in three dimensions
        /// </summary>
        public delegate Vector3D Path(double parameter);

        /// <summary>
        /// The function itself
        /// </summary>
        protected Path Function { get; set; }
        /// <summary>
        /// The initial value of the parameter
        /// </summary>
        private readonly double begin;
        /// <summary>
        /// The final value of the parameter
        /// </summary>
        private readonly double end;
        /// <summary>
        /// The number of steps in the parameter to render
        /// </summary>
        private readonly int nsteps;

        /// <summary>
        /// The number of points in each circle at each parameter point
        /// </summary>
        static public int CirclePoints { get; set; } = 8;
        /// <summary>
        /// The radius of the tube formed around the function
        /// </summary>
        static public double CircleRadius { get; set; } = .5;

        /// <param name="function">The parametric function to draw</param>
        /// <param name="begin">The initial value of the parameter</param>
        /// <param name="end">The final value of the parameter</param>
        /// <param name="nsteps">The number of steps to render</param>
        /// <param name="name">The name of the function - leave blank if the identical mesh will not be repeated</param>
        public FunctionShape3D(Path function, double begin, double end, int nsteps, string name = "") :
            base(name, true)
        {
            Function = function;
            if (end <= begin)
                throw new ArgumentException("Begin must be less than end!");

            this.begin = begin;
            this.end = end;
            if (nsteps <= 1)
                throw new ArgumentException("Nsteps must be greater than 1!");
            this.nsteps = nsteps;
        }

        protected override List<Vertex> MakeVertices()
        {
            var centerPoints = new List<Vector3D>();
            var vertices = new List<Vertex>();

            double step = (end - begin) / nsteps;
            double currentParam = begin;
            for (int i = 0; i <= nsteps; ++i, currentParam += step)
            {
                centerPoints.Add(Function(currentParam));
            }

            // Initial center point for end cap
            vertices.Add(new Vertex((Point3D)centerPoints[0], GetSlope(centerPoints, 0), new Point(.5, .5)));

            // Add circles
            for (int i = 0; i < centerPoints.Count; ++i)
            {
                double vCoord = (double)i / centerPoints.Count;
                var slope = GetSlope(centerPoints, i);
                AddCircle(vertices, centerPoints[i], slope, vCoord);
            }

            // Final center point for end cap
            vertices.Add(new Vertex((Point3D)centerPoints.Last(), GetSlope(centerPoints, centerPoints.Count - 1),
                new Point(.5, .5)));

            return vertices;
        }

        /// <summary>
        /// Adds a circle of vertices around a given point with a given normal
        /// </summary>
        /// <param name="vertices">The list of vertices that will be added to</param>
        /// <param name="centerPoint">The center of the circle</param>
        /// <param name="slope">The normal from the circle</param>
        /// <param name="vCoord">The v part of the uv coordinates to use</param>
        private void AddCircle(List<Vertex> vertices, Vector3D centerPoint, Vector3D slope, double vCoord)
        {
            double phiStep = 2 * Math.PI / CirclePoints;
            double phi = 0;
            for (int i = 0; i < CirclePoints; ++i, phi += phiStep)
            {
                var perp = GetPerpendicular(centerPoint, slope, phi);
                var scaledPerp = perp / perp.Length * CircleRadius;
                var point = centerPoint + scaledPerp;
                double uCoord = (double)i / CirclePoints;
                vertices.Add(new Vertex((Point3D)point, perp, new Point(uCoord, vCoord)));
            }
        }

        /// <summary>
        /// Find the tangent line at the point
        /// </summary>
        /// <param name="centerPoints">The list of all the points, used to extract slopes</param>
        /// <param name="index">The index out of the collection at which to find the tangent</param>
        private Vector3D GetSlope(List<Vector3D> centerPoints, int index)
        {
            if (index == 0)
            {
                return centerPoints[1] - centerPoints[0];
            }
            if (index == centerPoints.Count - 1)
            {
                return centerPoints[index] - centerPoints[index - 1];
            }
            var slope1 = centerPoints[index] - centerPoints[index - 1];
            var slope2 = centerPoints[index + 1] - centerPoints[index];
            return (slope1 + slope2) / 2;
        }

        /// <summary>
        /// Find the line perpendicular to the function at a point
        /// </summary>
        /// <param name="centerPoint">The center of the circle at this point</param>
        /// <param name="slope">The normal line at this point</param>
        /// <param name="phi">The angle to create the perpendicular line</param>
        /// <returns></returns>
        private Vector3D GetPerpendicular(Vector3D centerPoint, Vector3D slope, double phi)
        {
            var phi0Normal = Vector3D.CrossProduct(slope, new Vector3D(1, 0, 0));
            if (phi0Normal.LengthSquared == 0)
            {
                phi0Normal = Vector3D.CrossProduct(slope, new Vector3D(0, 1, 0));
            }

            var rotation = new AxisAngleRotation3D(slope, DongUtility.UtilityFunctions.RadiansToDegrees(phi));
            var transform = new RotateTransform3D(rotation);
            return (Vector3D)transform.Transform((Point3D)phi0Normal);
        }

        protected override Int32Collection MakeTriangles()
        {
            var triangles = new Int32Collection();

            // First end cap
            for (int i = 0; i < CirclePoints; ++i)
            {
                triangles.Add(0);
                int loopAround = (i == CirclePoints - 1 ? 0 : i + 1);
                triangles.Add(loopAround + 1);
                triangles.Add(i + 1);
            }

            // Now sides
            for (int i = 0; i < nsteps; ++i)
            {
                int offset = 1 + i * CirclePoints;
                for (int j = 0; j < CirclePoints; ++j)
                {
                    int loopAround = (j == CirclePoints - 1 ? 0 : j + 1);
                    triangles.Add(offset + j);
                    triangles.Add(offset + loopAround);
                    triangles.Add(offset + loopAround + CirclePoints);

                    triangles.Add(offset + loopAround + CirclePoints);
                    triangles.Add(offset + j + CirclePoints);
                    triangles.Add(offset + j);
                }
            }

            // Last end cap
            for (int i = 0; i < CirclePoints; ++i)
            {
                int offset = 1 + (nsteps + 1) * CirclePoints;
                triangles.Add(0);
                int loopAround = (i == CirclePoints - 1 ? 0 : i + 1);
                triangles.Add(loopAround + 1);
                triangles.Add(i + 1);
            }

            return triangles;
        }
    }
}
