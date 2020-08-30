using DongUtility;
using MotionVisualizerControl;
using PhysicsUtility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Media;
using static WPFUtility.UtilityFunctions;

namespace Visualizer.FastestDescent
{
    class FastestDescentDriver
    {
        static internal void Level1()
        {
            // x = t, y=0, z = a+bt+ct^2 --> z = a + bx + cx^2
            // A = (-10,0,10) and B = (0,0,0)
            // 0 = a + 0 + 0 --> a = 0;
            // 10 = -10b + 100c --> 1 = -b + 10c
            // From trial and error, b: [-1,4], c: [0,.5] is a good testing range
            double bMin = -1;
            double bMax = 4;
            double minTime = 5;
            double[] finalParameters = new double[3];
            for (double i = bMin; i < bMax; i += .01)
            {
                double[] parameters = new double[] { 0, i, (1 + i) / 10 };
                double time = RunOnce(1, parameters);
                if (time < minTime)
                {
                    minTime = time;
                    finalParameters = parameters;
                }
            }
            Debug.WriteLine("Min time = " + minTime);
            Debug.WriteLine("a: " + finalParameters[0] + ", b: " + finalParameters[1] + ", c: " +
                finalParameters[2]);
        }
        static internal void Level2()
        {
            // (-10,0,10) to (-6,0,8) to (0,0,0)
            // x = t, y=0, z = a+bt+ct^2+dt^3 --> z = a + bx + cx^2 + dx^3 --> a = 0
            // 10 = -10b + 100c - 1000d --> 1 = -b + 10c - 100d --> 27 = -27b + 270c - 2700d
            // 8 = -6b + 36c -216d --> 4 = -3b + 18c - 108d --> 100 = -75b + 450c - 2700d
            // Optimize 73 = -48b + 180c
            double minTime = 10;
            double[] finalParameters = new double[4];
            for (double b = -10; b <= 10; b += 0.01)
            {
                double c = (73 + 48 * b) / 180;
                double d = (-1 - 1 * b + 10 * c) / 100;
                double[] parameters = new double[] { 0, b, c, d };
                double time = RunOnce(2, parameters);
                if (time < minTime)
                {
                    minTime = time;
                    finalParameters = parameters;
                }
            }
            Debug.WriteLine("Min Time: " + minTime);
            Debug.WriteLine("z = " + finalParameters[0] + " + " + finalParameters[1] + "x + "
                + finalParameters[2] + "x^2 " + finalParameters[3] + "x^3");

            VisualizeFastestDescent(2, finalParameters);
        }
        static internal void Level3()
        {
            //(-10, -10, 10) to point(-6, -2, 8) to the origin
            //Plane these points lie on: -3x+y-2z = 0
            //Rotate these points onto xz plane
            Rotation rotate = new Rotation();
            Vector axisRotation = Vector.Cross(new Vector(-3,1,-2), new Vector(0,1,0)); // Cross of normal of plane and xz plane
            double angleBetweenPlanes = Vector.AngleBetween(new Vector(-3,1,-2), new Vector(0,1,0));
            rotate.RotateArbitraryAxis(axisRotation, angleBetweenPlanes);
            Vector vector1 = new Vector(-10, -10, 10);
            Vector vector2 = new Vector(-6, -2, 8);
            Vector rotatedVector1 = rotate.ApplyRotation(vector1); //(-16.326901661786017, 0, 5.782065558809322)
            Vector rotatedVector2 = rotate.ApplyRotation(vector2); //(-7.2653803323572035, 0, 7.156413111761864)
            // x = t, y=0, z = a+bt+ct^2+dt^3 --> z = a + bx + cx^2 + dx^3 --> a = 0
            // 5.782065558809322 = (-16.326901661786017)b + (-16.326901661786017)^2c + (-16.326901661786017)^3d
            // 7.156413111761864 = (-7.2653803323572035)b + (-7.2653803323572035)^2c + (-7.2653803323572035)^3d

            // 5.782065558809322 = -16.326901661786017b + 266.567717873631c - 4352.224915929492d
            // 7.156413111761864 = -7.2653803323572035b + 52.78575137380287c - 383.50855985992456d

            // 2217.471635474633 = -6261.506543286165b + 102231.0015868629c
            // -31146.319453694497 = 31620.569306189118b - 229735.46233512426c
            // Optimize: -28928.847818219863 = 25359.062762902955b - 127504.46074826136c

            double minTime = 10;
            double[] finalParameters = new double[4];
            for (double b = -10; b <= 10; b += .05)
            {
                double c = (25359.062762902955 * b + 28928.847818219863) / 127504.46074826136;
                double d = (-16.326901661786017 * b + 266.567717873631 * c - 5.782065558809322) / 4352.224915929492;
                double[] parameters = new double[] { 0, b, c, d };
                double time = RunOnce(3, parameters);
                Debug.WriteLine(b);
                if (time < minTime)
                {
                    minTime = time;
                    finalParameters = parameters;
                }
            }
            Debug.WriteLine("Min Time: " + minTime);
            Debug.WriteLine("z = " + finalParameters[0] + " + " + finalParameters[1] + "x + "
                + finalParameters[2] + "x^2 " + "+" + finalParameters[3] + "x^3");

            VisualizeFastestDescent(3, finalParameters);
        }
        static internal void RunFastestDescent()
        {
            // You will want to do your optimization here,
            // calling RunOnce() many times

            //Level1();
            //Level2();
            Level3();
            // If you don't want to display your optimized parameters here,
            // comment this line out.

            // VisualizeFastestDescent(finalParameters);
        }

        static private double RunOnce(int level, params double[] parameters)
        {
            var engine = SetupEngine(level, parameters);
            const double timeStep = .01;
            while (engine.Tick(engine.Time + timeStep))
            { 
                //Avoid being stuck in a loop
                if(engine.Time > 6)
                {
                    return 6;
                }
            }
            return engine.Time;
        }

        static private Engine SetupEngine(int level, params double[] parameters)
        {
            // Initial position is set by the path, so it doesn't need to be set now
            var projectile = new Projectile(Vector.NullVector(), Vector.NullVector(), 1); // The mass does not matter
            if(level == 1)
            {
                var path = new Simple2DQuadratic(parameters);
                return new Engine(projectile, path);
            }
            else if(level == 2)
            {
                var path = new Simple2DCubic(parameters);
                return new Engine(projectile, path);
            }
            else if(level == 3)
            {
                var path = new Simple3DCubic(parameters);
                return new Engine(projectile, path);
            }
            //Default return
            return new Engine(projectile, new Simple2DQuadratic(parameters));
        }

        static internal void VisualizeFastestDescent(int level, params double[] parameters)
        {
            var engine = SetupEngine(level, parameters);

            var visualization = new DescentVisualization(engine)
            {
                PathThickness = .5,
                PathColor = Colors.IndianRed,
                ProjectileSize = 1,
                ProjectileColor = Colors.NavajoWhite
            };

            var fullViz = new MotionVisualizer(visualization);
            //fullViz.BackgroundFile = "stars.jpg";

            fullViz.Add3DGraph("Position", () => engine.Time, () => ConvertVector(engine.Projectile.Position), "Time (s)", "Position (m)");
            fullViz.AddText("Time", Colors.MidnightBlue, () => (Math.Round(engine.Time, 3)).ToString() + " s");

            fullViz.SlowDraw = true;
            fullViz.TimeIncrement = .01;

            fullViz.Show();
        }

        //private static void AddSurfacesToVisualizer(YOURNAMEMarbleMachine surfaces, KinematicsVisualization visualization)
        //{
        //    foreach (var surface in surfaces.Surfaces)
        //        foreach (var triangle in surface.Triangles)
        //        {
        //            visualization.AddTriangle(triangle);
        //        }
        //}

        //private static void AddSurfaces(MarbleMachine surfaces, KinematicsEngine engine)
        //{
        //    var surfaceForce = new SurfaceForce(engine);
        //    foreach (var surface in surfaces.Surfaces)
        //    {
        //        surfaceForce.AddSurface(surface);
        //    }
        //    engine.AddForce(surfaceForce);
        //}

        //static private Vector3D ConvertToVector3D(Vector vec)
        //{
        //    return new Vector3D(vec.X, vec.Y, vec.Z);
        //}

        //static private void AddParticleStructure(ParticleStructure ps, KinematicsEngine engine)
        //{
        //    // Add projectiles
        //    foreach (var projectile in ps.Projectiles)
        //    {
        //        engine.AddProjectile(projectile);
        //    }

        //    // Add connectors
        //    foreach (var connector in ps.Connectors)
        //    {
        //        // Remember to connect it both ways
        //        engine.AddForce(new FiniteElementConnectorForce(connector.Projectile1, connector.Projectile2, connector.SpringConstant, connector.UnstretchedLength));
        //        engine.AddForce(new FiniteElementConnectorForce(connector.Projectile2, connector.Projectile1, connector.SpringConstant, connector.UnstretchedLength));
        //    }
        //}

        //static private void AddConnectorsToVisualizer(ParticleStructure ps, KinematicsVisualization viz)
        //{
        //    foreach (var connector in ps.Connectors)
        //    {
        //        var indices = ps.GetIndexOfProjectiles(connector);
        //        viz.AddTwoParticleConnector(indices.Item1, indices.Item2, ConnectorColor);
        //    }
        //}
    }
}
