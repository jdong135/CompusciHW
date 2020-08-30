using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizerControl
{
    /// <summary>
    /// A collection of VisualizerCommands representing one "turn" for the visualizer
    /// </summary>
    public class VisualizerCommandSet
    {
        private List<IVisualizerCommand> commands = new List<IVisualizerCommand>();

        /// <summary>
        /// Adds a new command to the CommandSet
        /// </summary>
        public void AddCommand(IVisualizerCommand command) => commands.Add(command);

        /// <summary>
        /// Executes all the commands in the set, in order
        /// </summary>
        /// <param name="visualizer">The Visualizer that is receiving the commands</param>
        public void ProcessAll(Visualizer visualizer)
        {
            foreach (var command in commands)
            {
                command.Do(visualizer);
            }
        }

        static public VisualizerCommandSet operator+(VisualizerCommandSet one, VisualizerCommandSet two)
        {
            var response = new VisualizerCommandSet();
            response.commands.AddRange(one.commands);
            response.commands.AddRange(two.commands);
            return response;
        }
    }
}
