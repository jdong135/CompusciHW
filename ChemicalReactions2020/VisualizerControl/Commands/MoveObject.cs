using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Commands
{
    /// <summary>
    /// A command to move an existing object to a new position
    /// The object is accessed by index.
    /// </summary>
    public class MoveObject : IVisualizerCommand
    {
        private readonly Vector3D newPosition;
        private readonly int index;

        public MoveObject(int index, Vector3D newPosition)
        {
            this.newPosition = newPosition;
            this.index = index;
        }

        public MoveObject(int index, double x, double y, double z) :
            this(index, new Vector3D(x, y, z))
        {}

        public void Do(Visualizer viz)
        {
            Object3D obj = viz.RetrieveObject(index);
            obj.Position = newPosition;
        }
    }
}
