using DongUtility;
using FiniteElement;
using GraphControl;
using MotionVisualizerControl;
using PhysicsUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace Visualizer.MarbleMadness
{
    static internal class MarbleMadnessDriver
    {
        static private Color ConnectorColor = Colors.Green;

        static internal void RunMarbleMadness()
        {
            var engine = new KinematicsEngine();
            engine.AddForce(new ConstantGravitationForce(engine, new Vector(0, 0, -9.8)));
            var ps = new YourParticleStructure();
            var surfaces = new JayDongMarbleMachine();
            AddParticleStructure(ps, engine);
            AddSurfaces(surfaces, engine);

            var adapter = new EngineAdapter(engine);

            Sphere3D.NSegments = 40;
            var visualization = new KinematicsVisualization(adapter);
            
            visualization.Box = true;

            visualization.ConnectorRadiusScale = .1;
            ProjectileAdapter.VisualSize = .01;

            AddConnectorsToVisualizer(ps, visualization);
            AddSurfacesToVisualizer(surfaces, visualization);

            Timeline.MaximumPoints = 3000;

            var fullViz = new MotionVisualizer(visualization);
            //fullViz.BackgroundFile = "stars.jpg";

            fullViz.Add3DGraph("Position", () => engine.Time, () => ConvertToVector3D(engine.Projectiles[0].Position), "Time (s)", "Position (m)");
            fullViz.Add3DGraph("Velocity", () => engine.Time, () => ConvertToVector3D(engine.Projectiles[0].Velocity), "Time (s)", "Velocity (m/s)");
            fullViz.Add3DGraph("Acceleration", () => engine.Time, () => ConvertToVector3D(engine.Projectiles[0].Acceleration), "Time (s)", "Acceleration (m/s^2)");

            //fullViz.AddSingleGraph("Speed", Colors.Teal, () => engine.Time, (() => engine.Projectiles[0].Velocity.Magnitude),
            //     "Time (s)", "Speed (m/s)");

            fullViz.Show();
        }

        private static void AddSurfacesToVisualizer(JayDongMarbleMachine surfaces, KinematicsVisualization visualization)
        {
            foreach (var surface in surfaces.Surfaces)
                foreach (var triangle in surface.Triangles)
                {
                    visualization.AddTriangle(triangle);
                }
        }

        private static void AddSurfaces(MarbleMachine surfaces, KinematicsEngine engine)
        {
            var surfaceForce = new SurfaceForce(engine);
            foreach (var surface in surfaces.Surfaces)
            {
                surfaceForce.AddSurface(surface);
            }
            engine.AddForce(surfaceForce);
        }

        static private Vector3D ConvertToVector3D(Vector vec)
        {
            return new Vector3D(vec.X, vec.Y, vec.Z);
        }

        static private void AddParticleStructure(ParticleStructure ps, KinematicsEngine engine)
        {
            // Add projectiles
            foreach (var projectile in ps.Projectiles)
            {
                engine.AddProjectile(projectile);
            }

            // Add connectors
            foreach (var connector in ps.Connectors)
            {
                // Remember to connect it both ways
                engine.AddForce(new FiniteElementConnectorForce(connector.Projectile1, connector.Projectile2, connector.SpringConstant, connector.UnstretchedLength));
                engine.AddForce(new FiniteElementConnectorForce(connector.Projectile2, connector.Projectile1, connector.SpringConstant, connector.UnstretchedLength));
            }
        }

        static private void AddConnectorsToVisualizer(ParticleStructure ps, KinematicsVisualization viz)
        {
            foreach (var connector in ps.Connectors)
            {
                var indices = ps.GetIndexOfProjectiles(connector);
                viz.AddTwoParticleConnector(indices.Item1, indices.Item2, ConnectorColor);
            }
        }

    }
}
