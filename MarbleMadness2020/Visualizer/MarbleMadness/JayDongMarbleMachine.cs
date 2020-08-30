using DongUtility;
using PhysicsUtility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Visualizer.MarbleMadness
{
    public class JayDongMarbleMachine : MarbleMachine
    {
        public JayDongMarbleMachine()
        {
            var cadetBlue = WPFUtility.UtilityFunctions.ConvertColor(Colors.CadetBlue);
            var red = WPFUtility.UtilityFunctions.ConvertColor(Colors.Red);
            var purple = WPFUtility.UtilityFunctions.ConvertColor(Colors.Purple);
            var transparent = WPFUtility.UtilityFunctions.ConvertColor(Colors.Transparent);
            // x is towards you
            // y is left right
            // z is up down
            var surface1 = new Surface(.1, 1);
            var pinballShapes = new Surface(.5, .2);
            void CreateSpiral(Vector corner1, Vector corner2, double width, double height, System.Drawing.Color color)
            {
                var diffVectors = corner2 - corner1;
                // otherSide is the cross of unit vector z and the difference between the two corner vectors (adjacent)
                var otherSide = -Vector.Cross(diffVectors, new Vector(0, 0, 1)).UnitVector() * width;
                var heightVector = Vector.Cross(diffVectors, otherSide).UnitVector() * height;
                pinballShapes.AddQuad(corner1, corner2, corner2 + heightVector, corner1 + heightVector, color);
                pinballShapes.AddQuad(corner1 + otherSide, corner1 + otherSide + heightVector, corner1 + diffVectors + heightVector + otherSide, corner1 + diffVectors + otherSide, color);
                pinballShapes.AddQuad(corner1, corner2, corner2 + otherSide, corner1 + otherSide, red);
            }
            void CreateBox(Vector corner1, Vector corner2, double width, double height, Boolean tube, Boolean openEnd, System.Drawing.Color color)
            {
                if (tube)
                {
                    var diffVectors = corner2 - corner1;
                    // otherSide is the cross of unit vector z and the difference between the two corner vectors (adjacent)
                    var otherSide = -Vector.Cross(diffVectors, new Vector(0, 0, 1)).UnitVector() * width;
                    var heightVector = Vector.Cross(diffVectors, otherSide).UnitVector() * height;
                    pinballShapes.AddQuad(corner1, corner2, corner2 + heightVector, corner1 + heightVector, color);
                    pinballShapes.AddQuad(corner1, corner1 + heightVector, corner1 + otherSide + heightVector, corner1 + otherSide, color);
                    pinballShapes.AddQuad(corner1 + otherSide, corner1 + otherSide + heightVector, corner1 + diffVectors + heightVector + otherSide, corner1 + diffVectors + otherSide, color);
                    pinballShapes.AddQuad(corner1 + otherSide + diffVectors, corner1 + otherSide + diffVectors + heightVector, corner1 + diffVectors + heightVector, corner1 + diffVectors, color);
                }
                else
                {
                    var diffVectors = corner2 - corner1;
                    // otherSide is the cross of unit vector z and the difference between the two corner vectors (adjacent)
                    var otherSide = -Vector.Cross(diffVectors, new Vector(0, 0, 1)).UnitVector() * width;
                    pinballShapes.AddQuad(corner1, corner1 + otherSide, corner1 + diffVectors + otherSide, corner2, color);
                    var heightVector = Vector.Cross(diffVectors, otherSide).UnitVector() * height;
                    pinballShapes.AddQuad(corner1, corner2, corner2 + heightVector, corner1 + heightVector, color);
                    pinballShapes.AddQuad(corner1 + otherSide, corner1 + otherSide + heightVector, corner1 + diffVectors + heightVector + otherSide, corner1 + diffVectors + otherSide, color);
                    if(openEnd == false)
                    {
                        pinballShapes.AddQuad(corner1, corner1 + heightVector, corner1 + otherSide + heightVector, corner1 + otherSide, color);
                        pinballShapes.AddQuad(corner1 + otherSide + diffVectors, corner1 + otherSide + diffVectors + heightVector, corner1 + diffVectors + heightVector, corner1 + diffVectors, color);
                    }
                }
                
            }
            void CreateSurface(Vector corner1, Vector corner2, Vector corner3, System.Drawing.Color color)
            {
                Vector corner4 = corner1 - corner2 + corner3;
                surface1.AddQuad(corner1, corner2, corner3, corner4, color);
            }
            void Create3DTriangle(Vector corner1, Vector corner2, Vector corner3, double height, System.Drawing.Color color)
            {
                pinballShapes.AddTriangle(new Triangle(corner1, corner2, corner3, color));
                var heightVector = -Vector.Cross(corner1 - corner2, corner3-corner2).UnitVector() * height;
                CreateSurface(corner1 + heightVector, corner2 + heightVector, corner2, color);
                CreateSurface(corner2 + heightVector, corner3 + heightVector, corner3, color);
                CreateSurface(corner1 + heightVector, corner3 + heightVector, corner3, color);
            }
            void CreateCylinder(Vector center, Vector point, double radius, System.Drawing.Color color)
            {
                Vector displacement = center - point;
                Vector axis1 = Vector.Cross(displacement, new Vector(0, 0, 1)).UnitVector(); // On plane
                Vector axis2 = Vector.Cross(displacement, axis1).UnitVector(); 
                double halfSidelength = radius * 2 * Math.Sin(Math.PI / 8)/2;
                double apothem = 2*halfSidelength / (2*Math.Tan(Math.PI/8));
                Vector shortHorizontal = axis1 * halfSidelength;
                Vector shortVertical = axis2 * halfSidelength;
                Vector longHorizontal = axis1 * apothem;
                Vector longVertical = axis2 * apothem;
                CreateSurface(center + longHorizontal + shortVertical + displacement, center + longHorizontal - shortVertical + displacement, center + longHorizontal - shortVertical, color);
                CreateSurface(center + shortHorizontal + longVertical + displacement, center + longHorizontal + shortVertical + displacement, center + longHorizontal + shortVertical, color);
                CreateSurface(center - shortHorizontal + longVertical + displacement, center + shortHorizontal + longVertical + displacement, center + shortHorizontal + longVertical, color);
                CreateSurface(center - longHorizontal + shortVertical + displacement, center - shortHorizontal + longVertical + displacement, center - shortHorizontal + longVertical, color);
                CreateSurface(center - longHorizontal + shortVertical + displacement, center - longHorizontal - shortVertical + displacement, center - longHorizontal - shortVertical, color);
                CreateSurface(center - longVertical - shortHorizontal + displacement, center - longHorizontal - shortVertical + displacement, center - longHorizontal - shortVertical, color);
                CreateSurface(center - longVertical + shortHorizontal + displacement, center - longVertical - shortHorizontal + displacement, center - longVertical - shortHorizontal, color);
                CreateSurface(center - shortVertical + longHorizontal + displacement, center - longVertical + shortHorizontal + displacement, center - longVertical + shortHorizontal, color);
            }
            void CreateRamp(double radius, Vector rampCenter, Vector rampCenter2, double angleIncrement)
            {
                for (double angle = 0; angle < Math.PI / 2; angle += angleIncrement)
                {
                    var point1 = new Vector(radius * Math.Cos(angle), 0, -radius * Math.Sin(angle)) + rampCenter;
                    var point2 = new Vector(radius * Math.Cos(angle + angleIncrement), 0, -radius * Math.Sin(angle + angleIncrement)) + rampCenter;
                    pinballShapes.AddTriangle(new Triangle(rampCenter, point1, point2, red));
                    var point3 = new Vector(radius * Math.Cos(angle), 0, -radius * Math.Sin(angle)) + rampCenter2;
                    var point4 = new Vector(radius * Math.Cos(angle + angleIncrement), 0, -radius * Math.Sin(angle + angleIncrement)) + rampCenter2;
                    pinballShapes.AddTriangle(new Triangle(rampCenter2, point3, point4, red));
                    CreateSurface(point1, point2, point4, cadetBlue);
                }
            }

            //Create invisible walls: front, left, back, right
            surface1.AddQuad(new Vector(.5, .5, .5), new Vector(.5, .5, -.5), new Vector(.5, -.5, -.5), new Vector(.5, -.5, .5), transparent, true);
            surface1.AddQuad(new Vector(.5, -.5, .5), new Vector(.5, -.5, -.5), new Vector(-.5, -.5, -.5), new Vector(-.5, -.5, .5), transparent, true);
            surface1.AddQuad(new Vector(-.5, -.5, .5), new Vector(-.5, -.5, -.5), new Vector(-.5, .5, -.5), new Vector(-.5, .5, .5), transparent, true);
            surface1.AddQuad(new Vector(-.5, .5, .5), new Vector(-.5, .5, -.5), new Vector(.5, .5, -.5), new Vector(.5, .5, .5), transparent, true);

            //Top layer ball rolls on
            CreateSurface(new Vector(-.5, -.5, .4), new Vector(.4, -.5, .2), new Vector(.4, .5, .2), cadetBlue);
            CreateSurface(new Vector(.5, -.5, .2), new Vector(.5, -.05, .2), new Vector(.4, -.05, .2), cadetBlue);
            CreateSurface(new Vector(.5, .5, .2), new Vector(.5, .05, .2), new Vector(.4, .05, .2), cadetBlue); 

            //Two "circles"
            CreateCylinder(new Vector(0, 0.2, .4), new Vector(0.01, 0.2, .5), .08, purple);
            CreateCylinder(new Vector(-.1, -.1, .4), new Vector(-.09, -.1, .5), .08, purple);
            CreateCylinder(new Vector(.2, -.15, .35), new Vector(0.21, -.15, .45), .08, purple);
            CreateCylinder(new Vector(-.3, .3, .45), new Vector(-0.29, .3, .55), .08, purple);

            //Guide rails on top surface
            CreateSurface(new Vector(-.4,0,.54), new Vector(-.4,0,.19+.1777), new Vector(-.2,.2, .32), red);
            CreateSurface(new Vector(-.3, 0.45, .54), new Vector(-.3, 0.45, .35), new Vector(-.1, .2, .3), red);

            //Two "bumpers"
            Create3DTriangle(new Vector(0, -.5, .28), new Vector(.5, -.5, .18), new Vector(.5, -.05, .18), .1, red);
            Create3DTriangle(new Vector(.5, .5, .18), new Vector(0, .5, .28), new Vector(.5, .05, .18), .1, red);

            //Quarter circle ramp
            CreateRamp(.15, new Vector(.35, .05, .2), new Vector(.35, -.05, .2), Math.PI / 16);
            //Diagonal ramp from quarter circle
            CreateBox(new Vector(0.35, .05, .05), new Vector(0, -.05, -.1), .1, .1, false, true, red);

            //Store the last two points of the spiral
            Vector endPoint1 = new Vector();
            Vector endPoint2 = new Vector();
            for (double t = 3.6 * Math.PI; t > 3 * Math.PI; t -= Math.PI / 64)
            {
                double a = 1;
                double b = 1;
                double x = a * Math.Cos(t);
                double x1 = a * Math.Cos(t + Math.PI / 32);
                double y = b * Math.Sin(t);
                double y1 = b * Math.Sin(t + Math.PI / 32);
                double z = t / 10;
                Vector point1 = new Vector(x / 10, y / 10 - .05, z / 2 - .67);
                Vector point2 = new Vector(x1 / 10, y1 / 10 - .05, z / 2 - .67);
                endPoint1 = point1;
                endPoint2 = point2;
                CreateSpiral(point1, point2, .1, .2, purple);
            }
            CreateSurface(endPoint2, endPoint2 + new Vector(.1, 0,0), endPoint2 + new Vector(0, 0.2, -.1), cadetBlue);
            CreateSurface(endPoint2 + new Vector(.1, 0, 0), endPoint2 + new Vector(0, 0.2, -.1), endPoint1 + new Vector(0, 0.2, 0), red);
            CreateSurface(endPoint2 + new Vector(0,0,.1) , endPoint2, endPoint2 + new Vector(0-.1, 0.2, -.1), red);

            //Ending funnel
            Vector funnelCenter = new Vector(0, 0, -.5);
            for (double angle = 0; angle < 2 * Math.PI; angle += Math.PI / 8)
            {
                double radius = .5;
                double innerRadius = .02;

                double z = -.3;
                Vector point1 = new Vector(radius * Math.Cos(angle), radius * Math.Sin(angle), z);
                Vector point2 = new Vector(radius * Math.Cos(angle + Math.PI / 8), radius * Math.Sin(angle + Math.PI / 8), z);
                Vector point3 = new Vector(innerRadius * Math.Cos(angle), innerRadius * Math.Sin(angle), -.48);
                CreateSurface(point2, point1, point3, purple);
            }

            AddSurface(surface1);
            AddSurface(pinballShapes);
        }

        //Fill out beginning and ending
        protected override Vector Beginning => new Vector(-.4, 0.23, .5);

        protected override Vector Ending => new Vector(0, 0, -.5);
    }
}
