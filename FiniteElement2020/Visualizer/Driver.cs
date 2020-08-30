using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DongUtility;
using VisualizerControl.Shapes;
using VisualizerControl;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Visualizer
{
    class Driver
    {

        static internal void Run()
        {
            //Kinematics.KinematicsDriver.RunKinematics();
            FiniteElement.FiniteElementDriver.RunFiniteElement();
        }



    }

}
