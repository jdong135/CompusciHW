using PhysicsUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl;
using VisualizerControl.Commands;
using VisualizerControl.Shapes;
using WPFUtility;
using static WPFUtility.UtilityFunctions;

namespace Visualizer.FastestDescent
{
    /// <summary>
    /// A visualization for a kinematics engine
    /// </summary>
    class DescentVisualization : IVisualization
    {
        private readonly Engine engine;

        private int counter = 0;
        private int projectileIndex;

        public DescentVisualization(Engine engine)
        {
            this.engine = engine;
        }

        public bool Continue { get; private set; } = true;
        public double Time => engine.Time;

        public Color ProjectileColor { get; set; } = Colors.IndianRed;
        public double ProjectileSize { get; set; } = 1;
        public Color PathColor { get; set; } = Colors.NavajoWhite;
        public double PathThickness { get; set; } = .5;

        public VisualizerCommandSet Initialization()
        {
            var set = new VisualizerCommandSet();

            var projectile = new ObjectPrototype(new Sphere3D(), new BasicMaterial(ProjectileColor),
                ConvertVector(engine.Projectile.Position), new Vector3D(ProjectileSize, ProjectileSize, ProjectileSize));
            set.AddCommand(new AddObject(projectile, counter));
            projectileIndex = counter;
            ++counter;

            var shape = new FunctionShape3D(engine.Path)
            {
                CircleRadius = PathThickness
            };
            var path = new ObjectPrototype(shape, new BasicMaterial(PathColor));
            set.AddCommand(new AddObject(path, counter));

            return set;
        }

        public VisualizerCommandSet Tick(double newTime)
        {
            Continue = engine.Tick(newTime);
            
            var set = new VisualizerCommandSet();

            set.AddCommand(new MoveObject(projectileIndex, engine.Projectile.Position));

            return set;
        }
    }
}
