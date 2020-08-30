using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizerControl.Commands
{
    public class ClearAll : IVisualizerCommand
    {
        public void Do(Visualizer viz)
        {
            viz.Clear();
        }
    }
}
