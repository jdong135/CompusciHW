using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer.Kinematics
{
    /// <summary>
    /// An interface to connect an engine to a KinematicsVisualization
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Increments the engine by one timestep
        /// </summary>
        /// <param name="newTime">The new time of the engine</param>
        /// <returns>True if the simulation should continue running</returns>
        bool Tick(double newTime);

        /// <summary>
        /// All projectiles in the engine
        /// </summary>
        List<IProjectile> Projectiles { get; }

        /// <summary>
        /// The current time of the engine
        /// </summary>
        double Time { get; }
    }
}
