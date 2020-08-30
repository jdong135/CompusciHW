using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizerControl
{
    /// <summary>
    /// An interface for a command issued to the visualizer
    /// </summary>
    public interface IVisualizerCommand
    {
        /// <summary>
        /// This executes the command
        /// </summary>
        /// <param name="viz">The visualizer that receives the command</param>
        void Do(Visualizer viz);
    }
}
