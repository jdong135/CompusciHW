using DongUtility;
using PhysicsUtility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Visualizer.MarbleMadness
{
    public class Surface
    {
        public List<Triangle> Triangles { get; } = new List<Triangle>();
        public double Elasticity { get; }
        public double FrictionCoefficient { get; }

        public Surface(double elasticity, double frictionCoefficient)
        {
            Elasticity = Math.Clamp(elasticity, 0, 1);
            FrictionCoefficient = Math.Clamp(frictionCoefficient, 0, 2);
        }

        public void AddTriangle(Triangle triangle)
        {
            Triangles.Add(triangle);
        }

        public void AddQuad(Vector p1, Vector p2, Vector p3, Vector p4, Color color, bool isTransparent = false)
        {
            AddTriangle(new Triangle(p1, p2, p4, color, isTransparent));
            AddTriangle(new Triangle(p4, p2, p3, color, isTransparent));
        }

    }
}
