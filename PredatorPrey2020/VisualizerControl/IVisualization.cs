using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizerControl
{
    /// <summary>
    /// Provides an interface for a kinematics engine or world class to work with the visualizer.
    /// This class should keep track of projectiles and forces and do all updates when told to.
    /// </summary>
    public interface IVisualization
    {
        /// <summary>
        /// Moves the time in the engine to newTime and updates all projectiles accordingly.
        /// </summary>
        /// <param name="newTime">The new time the engine will move to</param>
        /// <returns>The visualization commands that result from this operation</returns>
        VisualizerCommandSet Tick(double newTime);

        /// <summary>
        /// Commands to set up the initial visualization (before anything starts)
        /// </summary>
        /// <returns></returns>
        VisualizerCommandSet Initialization();

        /// <summary>
        /// A simple property to check whether the simulation should continue
        /// </summary>
        bool Continue { get; }

        /// <summary>
        /// The current time in the simulation
        /// </summary>
        double Time { get; }
    }
}
