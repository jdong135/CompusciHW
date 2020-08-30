using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace VisualizerControl
{
    public class ObjectPrototype
    {
        internal Shape3D Shape { get; }
        internal MaterialPrototype MaterialPrototype { get; }

        internal Vector3D Position { get; }
        internal Vector3D Scale { get; }
        internal double AzimuthalAngle { get; }
        internal double PolarAngle { get; }

        public ObjectPrototype(Shape3D shape, Color color, bool specular = false) :
            this(shape, new BasicMaterial(color, specular))
        { }

        public ObjectPrototype(Shape3D shape, MaterialPrototype material) :
            this(shape, material, new Vector3D(0, 0, 0), new Vector3D(1, 1, 1), 0, 0)
        {}

        public ObjectPrototype(Shape3D shape, MaterialPrototype material, Vector3D position, Vector3D scale,
            double azimuthal = 0, double polar = 0)
        {
            Shape = shape;
            MaterialPrototype = material;
            Position = position;
            Scale = scale;
            AzimuthalAngle = azimuthal;
            PolarAngle = polar;
        }
    }

}
