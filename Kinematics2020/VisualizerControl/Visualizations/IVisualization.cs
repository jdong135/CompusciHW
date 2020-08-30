using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace VisualizerControl.Visualizations
{
    public interface IVisualization
    {
        void Initialize();
        void SetVisualizer(Visualizer viz);
        List<Object3D> GetObjects();
        bool Tick(double newTime);
    }
}
