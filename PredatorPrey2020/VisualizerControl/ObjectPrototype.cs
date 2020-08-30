using DongUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;
using static WPFUtility.UtilityFunctions;

namespace VisualizerControl
{
    public class ObjectPrototype
    {
        internal Shape3D Shape { get; }
        internal MaterialPrototype MaterialPrototype { get; }

        internal Vector3D Position { get; }
        internal Vector3D Scale { get; }
        internal Matrix3D Rotation { get; }

        public ObjectPrototype(Shape3D shape, Color color, bool specular = false) :
            this(shape, new BasicMaterial(color, specular))
        { }

        public ObjectPrototype(Shape3D shape, MaterialPrototype material) :
            this(shape, material, new Vector3D(0, 0, 0), new Vector3D(1, 1, 1))
        {}

        public ObjectPrototype(Shape3D shape, MaterialPrototype material, Vector3D position, Vector3D scale) :
            this(shape, material, position, scale, Matrix3D.Identity)
        { }

        public ObjectPrototype(Shape3D shape, MaterialPrototype material, Vector3D position, Vector3D scale,
            Matrix3D rotation)
        {
            Shape = shape;
            MaterialPrototype = material;
            Position = position;
            Scale = scale;
            Rotation = rotation;
        }

        public ObjectPrototype(Shape3D shape, MaterialPrototype material, Vector position, Vector scale, Rotation rotation) :
            this(shape, material, ConvertVector(position), ConvertVector(scale), ConvertToMatrix3D(rotation.Matrix))
        { }

        public ObjectPrototype(Shape3D shape, MaterialPrototype material, Vector position, Vector scale) :
            this(shape, material, position, scale, DongUtility.Rotation.Identity)
        { }
    }

}
