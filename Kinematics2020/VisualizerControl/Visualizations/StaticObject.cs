using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace VisualizerControl.Visualizations
{
    class StaticObject : Object3D
    {
        public StaticObject(Shape3D shape, Material material) :
            base(shape, material)
        { }

        public override void Update()
        { }
    }
}
