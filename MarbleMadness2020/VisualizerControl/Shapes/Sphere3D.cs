using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Shapes
{
    /// <summary>
    /// A simple sphere
    /// </summary>
    public class Sphere3D : Shape3D
    {
        /// <summary>
        /// The number of segments (in both phi and theta) to divide the sphere into
        /// </summary>
        static public int NSegments { get; set; } = 16;

        public Sphere3D() :
            base("Sphere")
        { }

        protected override List<Vertex> MakeVertices()
        {
            var list = new List<Vertex>();

            // I use the physics convention where phi is the azimuthal angle and theta is the polar angle
            double thetaSeg = Math.PI / NSegments;
            double phiSeg = 2 * Math.PI / NSegments;

            list.Add(new Vertex(new Point3D(0, 0, 1), new Vector3D(0, 0, 1), new Point(0, 0)));

            for (double itheta = thetaSeg; itheta < Math.PI; itheta += thetaSeg)
            {
                for (double iphi = 0; iphi < 2 * Math.PI; iphi += phiSeg)
                {
                    double x = Math.Cos(iphi) * Math.Sin(itheta);
                    double y = Math.Sin(iphi) * Math.Sin(itheta);
                    double z = Math.Cos(itheta);

                    Point uvPoint = new Point(iphi / (2 * Math.PI), itheta / Math.PI);

                    list.Add(new Vertex(new Point3D(x, y, z), new Vector3D(x, y, z), uvPoint));
                }
            }

            list.Add(new Vertex(new Point3D(0, 0, -1), new Vector3D(0, 0, -1), new Point(1, 1)));

            return list;
        }

        protected override Int32Collection MakeTriangles()
        {
            var list = new Int32Collection();

            // Top ring
            for (int index = 1; index <= NSegments; ++index)
            {
                list.Add(0);
                list.Add(index);
                list.Add(index == NSegments ? 1 : index + 1);
            }

            // Middle section
            int maxTheta = NSegments * (NSegments - 2); // Index of the last point of the second-to-last theta ring
            for (int thetaIndex = 1; thetaIndex <= maxTheta; thetaIndex += NSegments)
            {
                for (int phiIndex = 0; phiIndex < NSegments; ++phiIndex)
                {
                    int thisPoint = thetaIndex + phiIndex;
                    int nextPhi = phiIndex == NSegments - 1 ? thetaIndex : thisPoint + 1;
                    int nextTheta = thisPoint + NSegments;
                    int nextThetaPhi = nextPhi + NSegments;

                    list.Add(thisPoint);
                    list.Add(nextTheta);
                    list.Add(nextThetaPhi);

                    list.Add(thisPoint);
                    list.Add(nextThetaPhi);
                    list.Add(nextPhi);
                }
            }

            // Bottom ring
            int lastThetaRing = maxTheta + 1;
            int lastIndex = NSegments * (NSegments - 1) + 1;
            for (int index = lastThetaRing; index < lastIndex; ++index)
            {
                list.Add(lastIndex);
                list.Add(index == lastIndex - 1 ? lastThetaRing : index + 1);
                list.Add(index);
            }

            return list;
        }

    }
}
