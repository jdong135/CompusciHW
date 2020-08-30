using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizerControl.Commands
{
    /// <summary>
    /// A command to remove an existing object, by index
    /// </summary>
    public class RemoveObject : IVisualizerCommand
    {
        private readonly int index;

        public RemoveObject(int index)
        {
            this.index = index;
        }

        public void Do(Visualizer viz) => viz.RemoveParticle(index);
    }
}
