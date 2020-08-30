﻿using DongUtility;
using System;
using System.Drawing;

namespace PhysicsUtility
{
    /// <summary>
    /// A triangle that can be used to make larger objects that things bounce off
    /// </summary>
    public class Triangle
    {
        /// <summary>
        /// The three points in the triangle, wound counterclockwise
        /// </summary>
        public Vector[] Points { get; } = new Vector[3];

        public Color Color { get; }

        public bool IsTransparent { get; }

        /// <summary>
        /// The three points in the rotated reference frame
        /// </summary>
        private readonly Vector[] rotatedPoints = new Vector[3];

        /// <summary>
        /// The normal vector to the plane of the triangle, with right-handed winding
        /// </summary>
        public Vector Normal { get; }
        /// <summary>
        /// A rotation to bring points from the original frame to the rotated reference frame in which the triangle lies in the xy plane
        /// </summary>
        private readonly Rotation rotationToZ;
        /// <summary>
        /// A rotation to bring points from the rotated reference frame back to the lab reference frame
        /// </summary>
        private readonly Rotation rotationFromZ;
        /// <summary>
        /// A translation to bring point 0 to the origin
        /// </summary>
        private Vector translateToZ;
        /// <summary>
        /// A reverse translation to bring point 0 back to its original position
        /// </summary>
        private Vector translateFromZ;

        /// <summary>
        /// Constructor. Points should be wound counterclockwise
        /// </summary>
        public Triangle(Vector point1, Vector point2, Vector point3, Color color, bool isTransparent = false)
        {
            if (point1 == point2 || point2 == point3 || point1 == point3)
                throw new ArgumentException("All three points of a triangle must be distinct!");

            Points[0] = point1;
            Points[1] = point2;
            Points[2] = point3;
            Color = color;
            IsTransparent = isTransparent;

            // Find normal
            Vector dir1 = Points[1] - Points[0];
            Vector dir2 = Points[2] - Points[0];
            Normal = Vector.Cross(dir1, dir2).UnitVector();

            // Create translations
            translateToZ = -Points[0];
            translateFromZ = Points[0];

            // Create rotations
            rotationFromZ = new Rotation();
            rotationFromZ.RotateYAxis(Normal.Polar);
            rotationFromZ.RotateZAxis(Normal.Azimuthal);
            rotationToZ = rotationFromZ.Inverse();

            // Find rotated points
            for (int i = 0; i < 3; ++i)
            {
                rotatedPoints[i] = TransformToZ(Points[i]);
            }
        }
        /// <summary>
        /// Returns the intersection of a given line and the triangle.
        /// Returns the null vector if they do not reach
        /// </summary>
        /// <param name="point">The starting point of the line</param>
        /// <param name="direction">The vector of the line's direction.  Magnitude dows not matter.</param>
        public Vector Intersection(Vector point, Vector direction)
        {
            // Thanks, Wikipedia!
            // https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
            double distance = Vector.Dot(Points[0] - point, Normal) / Vector.Dot(direction, Normal);
            return point + direction * distance;
        }

        /// <summary>
        /// Transforms a point to the rotated reference frame, in which the triangle lies in the xy plane
        /// </summary>
        private Vector TransformToZ(Vector input)
        {
            input += translateToZ;
            input = rotationToZ.ApplyRotation(input);
            return input;
        }

        /// <summary>
        /// Transforms a point from the rotated reference frame back to the lab frame
        /// </summary>
        private Vector TransformFromZ(Vector input)
        {
            input = rotationFromZ.ApplyRotation(input);
            input += translateFromZ;
            return input;
        }

        /// <summary>
        /// Determines if the particle passed through the triangle between the initial and final points given
        /// </summary>
        public bool PassedThrough(Vector initial, Vector final)
        {
            // Rotate so that plane of the Triangle is the xy plane
            Vector rotatedInitial = TransformToZ(initial);
            Vector rotatedFinal = TransformToZ(final);

            // Check if points are on opposite sides
            if (Math.Sign(rotatedInitial.Z) == Math.Sign(rotatedFinal.Z))
                return false;

            // Find where the line intersects the xy plane
            double xslope = (rotatedFinal.X - rotatedInitial.X) / (rotatedFinal.Z - rotatedInitial.Z);
            double yslope = (rotatedFinal.Y - rotatedInitial.Y) / (rotatedFinal.Z - rotatedInitial.Z);
            double zDiff = rotatedInitial.Z;
            Vector rotatedIntersection = new Vector(xslope * zDiff + rotatedInitial.X, yslope * zDiff + rotatedInitial.Y, 0);

            // Check to see if it is inside the triangle
            return InTriangle(rotatedPoints[0], rotatedPoints[1], rotatedPoints[2], rotatedIntersection);
        }

        /// <summary>
        /// Reflects the given vector about the triangle
        /// </summary>
        public Vector Reflect(Vector input)
        {
            Vector rotatedInput = TransformToZ(input);
            rotatedInput.Z = -rotatedInput.Z;
            return TransformFromZ(rotatedInput);
        }

        /// <summary>
        /// Calculates if a given point lies within the triangle formed by the three points given
        /// All points are assumed to lie in the xy plane
        /// </summary>
        static private bool InTriangle(Vector p1, Vector p2, Vector p3, Vector point)
        {
            return CheckSameSide(p1, p2, p3, point) && CheckSameSide(p2, p3, p1, point) && CheckSameSide(p3, p1, p2, point);
        }

        /// <summary>
        /// Returns whether a point is on the same side of a line as a reference point
        /// This assumes that all points lie in the xy plane - z components are ignored
        /// </summary>
        /// <param name="p1">One of the two points that define a line</param>
        /// <param name="p2">One of the two points that define a line</param>
        /// <param name="reference">The reference point</param>
        /// <param name="point">The point we are interested in</param>
        /// <returns>True if point and reference lie on the same side of the line defined by p1 and p2</returns>
        static private bool CheckSameSide(Vector p1, Vector p2, Vector reference, Vector point)
        {
            if (p1.X == p2.X)
            {
                double splitPoint = p1.X;
                double checkRef = reference.X - splitPoint;
                double checkPoint = point.X - splitPoint;

                return Math.Sign(checkRef) == Math.Sign(checkPoint);
            }
            else
            {
                double slope = (p2.Y - p1.Y) / (p2.X - p1.X);
                double intercept = p1.Y - slope * p1.X;
                double checkRef = slope * reference.X + intercept - reference.Y;
                double checkPoint = slope * point.X + intercept - point.Y;

                return Math.Sign(checkRef) == Math.Sign(checkPoint);
            }
        }
    }
}
