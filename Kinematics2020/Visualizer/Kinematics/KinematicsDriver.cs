using DongUtility;
using GraphControl;
using MotionVisualizerControl;
using Homework_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;
using Utility;

namespace Visualizer.Kinematics
{
    static internal class KinematicsDriver
    {
        
        static internal void RunKinematics()
        {
            // Level 1
            //var engine = new World(0.01, 0, -9.8);
            //Projectile projectile1 = new Projectile(new Utility.Vector(0, 0, 0), new Utility.Vector(4, 0, Math.PI / 4, false), new Utility.Vector(0, 0, -9.8), 5);
            //engine.AddProjectile(projectile1);

            // Level 2
            //var engine = new World(0.01, 0.4, -9.8);
            //Projectile projectile1 = new Projectile(new Utility.Vector(0, 0, 0), new Utility.Vector(4, 0, Math.PI / 4, false), new Utility.Vector(0, 0, -9.8), 5);
            //engine.AddProjectile(projectile1);

            // Level 3
            //var engine = new World(0.01, 0.4, -9.8);
            //Projectile projectile1 = new Projectile(new Utility.Vector(1, 1, 1), new Utility.Vector(-2, 1, 3), new Utility.Vector(0, 0, -9.8), 5);
            //Spring spring1 = new Spring(2, 5, new Utility.Vector(), projectile1.Position);
            //engine.AddProjectile(projectile1);
            //engine.AddSpring(spring1);

            // Challenge
            // This should connect to your World or Engine class from your kinematics project
            var engine = new World(0.01, 0.4, -9.8);
            Projectile projectile1 = new Projectile(new Utility.Vector(1, 1, 1), new Utility.Vector(-2, 1, 3), new Utility.Vector(0, 0, -9.8), 5);
            Projectile projectile2 = new Projectile(new Utility.Vector(2, 2, 2), new Utility.Vector(1, -3, 4), new Utility.Vector(0, 0, -9.8), 3);
            Spring spring1 = new Spring(2, 5, new Utility.Vector(), projectile1.Position);
            Spring spring2 = new Spring(2, 5, projectile1.Position, projectile2.Position);
            engine.AddProjectile(projectile1);
            engine.AddProjectile(projectile2);
            engine.AddSpring(spring1);
            engine.AddSpring(spring2);
            // This will work once you define an EngineAdapter that inherits from IEngine


            var adapter = new EngineAdapter(engine);

            Sphere3D.NSegments = 40;
            var visualization = new KinematicsVisualization(adapter);

            Timeline.MaximumPoints = 3000;

            var fullViz = new MotionVisualizer(visualization);

            // For Levels 1-3
            //fullViz.Add3DGraph("Position", () => engine.Time, () => ConvertToVector3D(engine.ProjectileList[0].Position), "Time (s)", "Position (m)");
            //fullViz.Add3DGraph("Velocity", () => engine.Time, () => ConvertToVector3D(engine.ProjectileList[0].Velocity), "Time (s)", "Velocity (m/s)");
            //fullViz.Add3DGraph("Acceleration", () => engine.Time, () => ConvertToVector3D(engine.ProjectileList[0].Acceleration), "Time (s)", "Acceleration (m/s^2)");

            // For the challenge
            fullViz.AddSingleGraph("Distance Between Projectiles", Colors.Teal, () => engine.Time, (() => (engine.ProjectileList[1].Position - engine.ProjectileList[0].Position).Magnitude)
            , "Time (s)", "Distance (m)");
            fullViz.Add3DGraph("CM Position", () => engine.Time, () => ConvertToVector3D(engine.CalcCOM(projectile1, projectile2)), "Time (s)", "CM Position (m)");
            fullViz.Add3DGraph("CM Velocity", () => engine.Time, () => ConvertToVector3D((engine.ProjectileList[0].Velocity*engine.ProjectileList[0].Mass + engine.ProjectileList[1].Velocity * engine.ProjectileList[1].Mass)/(engine.ProjectileList[0].Mass+engine.ProjectileList[1].Mass)), "Time (s)", "CM Velocity (m/s)");
            fullViz.Add3DGraph("CM Accleration", () => engine.Time, () => ConvertToVector3D((engine.ProjectileList[0].Acceleration * engine.ProjectileList[0].Mass + engine.ProjectileList[1].Acceleration * engine.ProjectileList[1].Mass) / (engine.ProjectileList[0].Mass + engine.ProjectileList[1].Mass)), "Time (s)", "CM Acceleration (m/s^2)");

            fullViz.Show();
        }

        static Vector3D ConvertToVector3D(Utility.Vector vec)
        {
            return new Vector3D(vec.X, vec.Y, vec.Z);
        }
    }
}
