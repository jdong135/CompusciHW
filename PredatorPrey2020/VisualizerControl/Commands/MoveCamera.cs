using DongUtility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using static WPFUtility.UtilityFunctions;

namespace VisualizerControl.Commands
{
    public class MoveCamera : IVisualizerCommand
    {
        private Vector position;
        public MoveCamera(Vector position)
        {
            this.position = position;
        }
        public void Do(Visualizer viz)
        {
            viz.Camera.Position = ConvertToPoint3D(ConvertVector(position));
        }
    }
}
