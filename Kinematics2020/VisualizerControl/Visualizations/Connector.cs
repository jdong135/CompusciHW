using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using VisualizerControl.Shapes;

namespace VisualizerControl.Visualizations
{
    class Connector : Object3D
    {
        private Object3D part1;
        private Object3D part2;

        static private Color color = Colors.LimeGreen;
        private const double specularCoefficient = 1;

        static private Material material = new SpecularMaterial(new SolidColorBrush(color), specularCoefficient);

        public Connector(Object3D part1, Object3D part2) :
            base(new CaplessCylinder3D(), material)
        {
            this.part1 = part1;
            this.part2 = part2;

            const double connectorScale = .2;

            double xyScale = Math.Min(AverageScale(part1.Scale), AverageScale(part2.Scale)) * connectorScale;
            ScaleEvenly(xyScale);

            Update();
        }

        static private double AverageScale(Vector3D vec)
        {
            double average = 0;
            average += vec.X;
            average += vec.Y;
            average += vec.Z;
            return average / 3;
        }

        public override void Update()
        {
            // Scale
            var diff = part2.Position - part1.Position;
            var length = diff.Length;
            Scale = new Vector3D(Scale.X, Scale.Y, length / 2);

            // Translate
            var midpoint = (part1.Position + part2.Position) / 2;
            Position = midpoint;

            // Rotate
            double theta = Math.Acos(diff.Z / length);
            double phi = Math.Atan2(diff.Y, diff.X);

            AzimuthalAngle = phi;
            PolarAngle = theta;
        }
    }
}
