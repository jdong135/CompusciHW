using DongUtility;
using FiniteElement;
using GraphControl;
using MotionVisualizerControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace Visualizer.FiniteElement
{
    static internal class FiniteElementDriver
    {
        static private Color ConnectorColor = Colors.Green;

        static internal void RunFiniteElement()
        {
            var engine = new KinematicsEngine();
            engine.AddForce(new ConstantGravitationForce(engine, new Vector(0, 0, -9.8)));
            engine.AddForce(new GroundForce(engine));
            var ps = new YourParticleStructure();
            AddParticleStructure(ps, engine);

            var adapter = new EngineAdapter(engine);

            Sphere3D.NSegments = 40;
            var visualization = new KinematicsVisualization(adapter);
            
            visualization.Ground = true;

            visualization.ConnectorRadiusScale = .1;
            ProjectileAdapter.VisualSize = .1;

            AddConnectorsToVisualizer(ps, visualization);

            Timeline.MaximumPoints = 3000;

            var fullViz = new MotionVisualizer(visualization);
            //fullViz.BackgroundFile = "stars.jpg";

            //fullViz.Add3DGraph("Position", () => engine.Time, () => ConvertToVector3D(engine.Projectiles[0].Position), "Time (s)", "Position (m)");
            //fullViz.Add3DGraph("Velocity", () => engine.Time, () => ConvertToVector3D(engine.Projectiles[0].Velocity), "Time (s)", "Velocity (m/s)");
            //fullViz.Add3DGraph("Acceleration", () => engine.Time, () => ConvertToVector3D(engine.Projectiles[0].Acceleration), "Time (s)", "Acceleration (m/s^2)");
            //fullViz.Add3DGraph("Center of Mass", () => engine.Time, () => ConvertToVector3D(ps.CalcCOM()), "Time (s)", "Center of Mass (m)");
            //fullViz.Add3DGraph("Rotation", () => ps.ProjectileList[0].Position.Polar, () => ConvertToVector3D(ps.ProjectileList[0].Position), "Y Position (m)", "Position");

            //fullViz.AddSingleGraph("Rotation Motion", Colors.Teal, () => ps.ProjectileList[0].Position.Y, (() => ps.ProjectileList[0].Position.Z)
            //    , "Y Position", "Z Position");

            fullViz.Show();
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
