using DongUtility;
using PredatorPrey;
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

namespace Visualizer.PredatorPrey
{
    class PredatorPreyVisualization : IVisualization
    {
        private Arena arena;

        public PredatorPreyVisualization(Arena arena)
        {
            this.arena = arena;
        }

        public bool Continue { get; private set; } = true;

        public double Time => arena.Time;

        public VisualizerCommandSet Initialization()
        {
            var commandSet = new VisualizerCommandSet();

            for (int i = 0; i < arena.Animals.Count; ++i)
            {
                var animal = arena.Animals[i];
                commandSet.AddCommand(new AddObject(new ObjectPrototype(new Cube3D(), i == 0 ? Colors.Blue : Colors.Red), i));
                commandSet.AddCommand(new TransformObject(i, new Vector(animal.Position.X, animal.Position.Y, 0), new Vector(animal.Width, animal.Height, animal.Width)));
            }
            commandSet.AddCommand(new AddObject(new ObjectPrototype(new Cube3D(), Colors.SandyBrown), arena.Animals.Count));
            commandSet.AddCommand(new TransformObject(arena.Animals.Count, new Vector(0, 0, 0), new Vector(arena.Width, arena.Height, .01)));
            commandSet.AddCommand(new MoveCamera(new Vector(80, 80, 80)));

            return commandSet;
        }

        public VisualizerCommandSet Tick(double newTime)
        {
            Continue = arena.Update(newTime);

            var commandSet = new VisualizerCommandSet();

            for (int i = 0; i < arena.Animals.Count; ++i)
            {
                commandSet.AddCommand(new MoveObject(i, new Vector3D(arena.Animals[i].Position.X, arena.Animals[i].Position.Y, 0)));
            }

            return commandSet;
        }
    }
}
